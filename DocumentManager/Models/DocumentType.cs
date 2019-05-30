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
		public static readonly MemoryCache<DocumentType[]> CachedDocumentTypes = new MemoryCache<DocumentType[]>(CacheStorageRefresher);

		public int Id;
		public Str Name;

		private static void Validate(ref string nameEn, ref string namePtBr) {
			if (string.IsNullOrWhiteSpace(nameEn))
				throw new ValidationException(Str.InvalidName);
			if ((nameEn = nameEn.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (string.IsNullOrWhiteSpace(namePtBr))
				throw new ValidationException(Str.InvalidName);
			if ((namePtBr = namePtBr.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);
		}

		public static DocumentType Create(string nameEn, string namePtBr) {
			Validate(ref nameEn, ref namePtBr);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO document_type (name_en, name_ptbr) VALUES (@name_en, @name_ptbr)", conn)) {
					cmd.Parameters.AddWithValue("@name_en", nameEn);
					cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			CachedDocumentTypes.Refresh();

			return new DocumentType(id, new Str(nameEn, namePtBr));
		}

		private static DocumentType[] CacheStorageRefresher() {
			List<DocumentType> documentTypes = new List<DocumentType>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr FROM document_type ORDER BY name_en ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							documentTypes.Add(new DocumentType(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2))));
					}
				}
			}
			return documentTypes.ToArray();
		}

		public static DocumentType[] GetAll() {
			DocumentType[] cachedDocumentTypes = CachedDocumentTypes.StartReading();
			try {
				return cachedDocumentTypes;
			} finally {
				CachedDocumentTypes.FinishReading();
			}
		}

		public static DocumentType GetById(int id) {
			DocumentType course = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr FROM document_type WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							course = new DocumentType(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2)));
					}
				}
			}
			return course;
		}

		public DocumentType() {
		}

		private DocumentType(int id, Str name) {
			Id = id;
			Name = name;
		}

		public override string ToString() => Name.ToString();

		public void Update(string nameEn, string namePtBr) {
			Validate(ref nameEn, ref namePtBr);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE document_type SET name_en = @name_en, name_ptbr = @name_ptbr WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@name_en", nameEn);
					cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
					Name = new Str(nameEn, namePtBr);
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
		}
	}
}
