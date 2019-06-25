using DocumentManager.Exceptions;
using DocumentManager.Localization;
using DocumentManager.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace DocumentManager.Models {
	public class DocumentType {
		public static readonly MemoryCache<DocumentType[][]> CachedDocumentTypes = new MemoryCache<DocumentType[][]>(CacheStorageRefresher);

		public int Id;
		public Str Name;
		public int[] DefaultTagIds;

		private static void Validate(ref string nameEn, ref string namePtBr, int[] newTagIds) {
			if (string.IsNullOrWhiteSpace(nameEn) ||
				string.IsNullOrWhiteSpace(namePtBr))
				throw new ValidationException(Str.InvalidName);
			if ((nameEn = nameEn.Trim().ToUpper()).Length > 64 ||
				(namePtBr = namePtBr.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (newTagIds != null) {
				for (int i = newTagIds.Length - 1; i >= 0; i--) {
					if (newTagIds[i] <= 0)
						throw new ValidationException(Str.InvalidTag);
				}
				for (int i = newTagIds.Length - 1; i > 0; i--) {
					int t = newTagIds[i];
					for (int j = 0; j < i; j++) {
						if (t == newTagIds[j])
							throw new ValidationException(Str.RepeatedTags);
					}
				}
			}
		}

		public static void SyncValues(MySqlConnection conn, MySqlTransaction tran, int documentTypeId, int[] existingTagIds, int[] newTagIds) {
			bool recreate = false;

			if (existingTagIds == null || existingTagIds.Length == 0 ||
				newTagIds == null || newTagIds.Length == 0 ||
				existingTagIds.Length != newTagIds.Length) {
				recreate = true;
			} else {
				for (int i = 0; i < existingTagIds.Length; i++) {
					if (existingTagIds[i] != newTagIds[i]) {
						recreate = true;
						break;
					}
				}
			}

			if (recreate) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM document_type_default_tags WHERE document_type_id = @id", conn, tran)) {
					cmd.Parameters.AddWithValue("@id", documentTypeId);
					cmd.ExecuteNonQuery();
				}

				if (newTagIds != null && newTagIds.Length > 0) {
					using (MySqlCommand cmd = new MySqlCommand("INSERT INTO document_type_default_tags (document_type_id, tag_id, position) VALUES (@document_type_id, @tag_id, @position)", conn, tran)) {
						cmd.Parameters.AddWithValue("@document_type_id", documentTypeId);
						cmd.Parameters.AddWithValue("@tag_id", 0);
						cmd.Parameters.AddWithValue("@position", 0);
						for (int i = 0; i < newTagIds.Length; i++) {
							cmd.Parameters[1].Value = newTagIds[i];
							cmd.Parameters[2].Value = i;
							cmd.ExecuteNonQuery();
						}
					}
				}
			}
		}

		public static DocumentType Create(string nameEn, string namePtBr, int[] defaultTagIds) {
			Validate(ref nameEn, ref namePtBr, defaultTagIds);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("INSERT INTO document_type (name_en, name_ptbr) VALUES (@name_en, @name_ptbr)", conn)) {
							cmd.Parameters.AddWithValue("@name_en", nameEn);
							cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
							cmd.ExecuteNonQuery();
						}
						using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
							id = (int)(ulong)cmd.ExecuteScalar();
						}
						SyncValues(conn, tran, id, null, defaultTagIds);
					} catch {
						tran.Rollback();
						throw;
					}
					tran.Commit();
				}
			}

			CachedDocumentTypes.Refresh();

			return new DocumentType(id, new Str(nameEn, namePtBr)) {
				DefaultTagIds = defaultTagIds
			};
		}

		private static DocumentType[][] CacheStorageRefresher() {
			List<DocumentType> documentTypes = new List<DocumentType>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($"SELECT d.id, d.name_en, d.name_ptbr, t.tag_id FROM document_type d LEFT JOIN document_type_default_tags t ON t.document_type_id = d.id ORDER BY d.id ASC, t.position ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						DocumentType lastDocumentType = new DocumentType();
						List<int> defaultTagIds = new List<int>();
						while (reader.Read()) {
							int id = reader.GetInt32(0);
							if (id != lastDocumentType.Id) {
								if (lastDocumentType.Id > 0)
									lastDocumentType.DefaultTagIds = defaultTagIds.ToArray();
								defaultTagIds.Clear();
								lastDocumentType = new DocumentType(id, new Str(reader.GetString(1), reader.GetString(2)));
								documentTypes.Add(lastDocumentType);
							}
							if (!reader.IsDBNull(3))
								defaultTagIds.Add(reader.GetInt32(3));
						}
						if (lastDocumentType.Id > 0)
							lastDocumentType.DefaultTagIds = defaultTagIds.ToArray();
					}
				}
			}
			DocumentType[] arrayEn, arrayPtBr;
			Array.Sort(arrayEn = documentTypes.ToArray(), (a, b) => a.Name.ValueEn.CompareTo(b.Name.ValueEn));
			Array.Sort(arrayPtBr = documentTypes.ToArray(), (a, b) => a.Name.ValuePtBr.CompareTo(b.Name.ValuePtBr));
			// Ordered by language
			// Str.LanguagePtBr = 0
			// Str.LanguageEn = 1
			return new DocumentType[][] { arrayPtBr, arrayEn };
		}

		public static DocumentType[] GetAll() {
			DocumentType[][] cachedDocumentTypes = CachedDocumentTypes.StartReading();
			try {
				return cachedDocumentTypes?[Str.CurrentLanguage];
			} finally {
				CachedDocumentTypes.FinishReading();
			}
		}

		public static DocumentType GetById(int id) {
			// Since all document type objects are cached in memory with all properties
			// set, and since there are not too many of those objects, it is faster
			// to look up for one of them here, instead of reading it from the database
			DocumentType[][] cachedDocumentTypes = CachedDocumentTypes.StartReading();
			try {
				DocumentType[] documentTypes;
				if ((documentTypes = cachedDocumentTypes?[0]) != null) {
					for (int i = documentTypes.Length - 1; i >= 0; i--) {
						if (documentTypes[i].Id == id)
							return documentTypes[i];
					}
				}
				return null;
			} finally {
				CachedDocumentTypes.FinishReading();
			}
		}

		public DocumentType() {
		}

		private DocumentType(int id, Str name) {
			Id = id;
			Name = name;
		}

		public override string ToString() => Name.ToString();

		public void Update(string nameEn, string namePtBr, int[] defaultTagIds) {
			Validate(ref nameEn, ref namePtBr, defaultTagIds);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("UPDATE document_type SET name_en = @name_en, name_ptbr = @name_ptbr WHERE id = @id", conn)) {
							cmd.Parameters.AddWithValue("@name_en", nameEn);
							cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
							cmd.Parameters.AddWithValue("@id", Id);
							cmd.ExecuteNonQuery();
							Name = new Str(nameEn, namePtBr);
						}
						SyncValues(conn, tran, Id, DefaultTagIds, defaultTagIds);
						DefaultTagIds = defaultTagIds;
					} catch {
						tran.Rollback();
						throw;
					}
					tran.Commit();
				}
			}

			CachedDocumentTypes.Refresh();
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM document_type WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}

			CachedDocumentTypes.Refresh();
			User.PurgeAllCachedUsers();
		}
	}
}
