using DocumentManager.Exceptions;
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
		public string Name;

		private static void Validate(ref string name) {
			if (string.IsNullOrWhiteSpace(name))
				throw new ValidationException("Nome inválido!");
			if ((name = name.Trim().ToUpper()).Length > 64)
				throw new ValidationException("Nome muito longo!");
		}

		public static DocumentType Create(string name) {
			Validate(ref name);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO document_type (name) VALUES (@name)", conn)) {
					cmd.Parameters.AddWithValue("@name", name);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			CachedDocumentTypes.Refresh();

			return new DocumentType(id, name);
		}

		private static DocumentType[] CacheStorageRefresher() {
			List<DocumentType> documentTypes = new List<DocumentType>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM document_type ORDER BY name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							documentTypes.Add(new DocumentType(reader.GetInt32(0), reader.GetString(1)));
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
			DocumentType documentType = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM document_type WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							documentType = new DocumentType(reader.GetInt32(0), reader.GetString(1));
					}
				}
			}
			return documentType;
		}

		public DocumentType() {
		}

		private DocumentType(int id, string name) {
			Id = id;
			Name = name;
		}

		public override string ToString() {
			return Name;
		}

		public void Update(string name) {
			Validate(ref name);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE document_type SET name = @name WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@name", name);
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
					Name = name;
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
