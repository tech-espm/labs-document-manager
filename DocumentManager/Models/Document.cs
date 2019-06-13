using DocumentManager.Exceptions;
using DocumentManager.Localization;
using DocumentManager.Utils;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace DocumentManager.Models {
	public class Document {
		public const int MinFileSizeInBytes = 20;
		public const int MaxFileSizeInMiB = 25;
		public const int MaxFileSizeInBytes = MaxFileSizeInMiB << 20;

		// We must use properties in order to make [FromForm] work with multipart/form-data
		// ([FromBody] works OK with plain fields)
		public class Data {
			public int Id { get; set; }
			public int Size { get; set; }
			public int Unity { get; set; }
			public int Course { get; set; }
			public int PartitionType { get; set; }
			public int DocumentType { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string Extension { get; set; }
			public int[] TagIds { get; set; }
			public int[] TagValues { get; set; }
		}

		public int Id, Size;
		public string Name, Description, Extension, CreationTime;
		public IdNamePair Unity, Course, PartitionType, DocumentType;
		public List<IdValuePair> Tags;

		public string SafeDownloadName => Storage.SafeFileName(Name.ToLower()) + "." + Extension.ToLower();

		private static void Validate(Data data) {
			if (data == null)
				throw new ValidationException(Str.InvalidDocumentData);

			if (string.IsNullOrWhiteSpace(data.Name))
				throw new ValidationException(Str.InvalidName);
			if ((data.Name = data.Name.Trim().ToUpper()).Length > 128)
				throw new ValidationException(Str.NameTooLong);

			if ((data.Description = (data.Description ?? "").Trim().ToUpper()).Length > 255)
				throw new ValidationException(Str.DescriptionTooLong);

			if (string.IsNullOrWhiteSpace(data.Extension))
				throw new ValidationException(Str.InvalidFileExtension);
			if ((data.Extension = data.Extension.Trim().ToUpper()).Length > 10)
				throw new ValidationException(Str.FileExtensionTooLong);

			if (data.Size < MinFileSizeInBytes)
				throw new ValidationException(Str.FileSizeTooSmall);

			if (data.Size > MaxFileSizeInBytes)
				throw new ValidationException(Str.FileSizeTooLarge);

			if (data.Unity <= 0)
				throw new ValidationException(Str.InvalidUnity);

			if (data.Course <= 0)
				throw new ValidationException(Str.InvalidCourse);

			if (data.PartitionType <= 0)
				throw new ValidationException(Str.InvalidPartitionType);

			if (data.DocumentType <= 0)
				throw new ValidationException(Str.InvalidDocumentType);

			if (data.TagIds == null)
				data.TagIds = new int[0];
			if (data.TagValues == null)
				data.TagValues = new int[0];
			if (data.TagIds.Length != data.TagValues.Length)
				throw new ValidationException(Str.InvalidTags);
			for (int i = data.TagIds.Length - 1; i >= 0; i--) {
				if (data.TagIds[i] <= 0 || data.TagValues[i] <= 0)
					throw new ValidationException(Str.InvalidTags);
			}
		}

		public static Document Create(Data data, IFormFile file, User user) {
			Validate(data);

			if (file == null)
				throw new ValidationException(Str.NoFilesWereAdded);
			if (file.Length != data.Size)
				throw new ValidationException(Str.InvalidFileSize);

			int id;
			string path = null;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("INSERT INTO document (name, description, extension, size, unity_id, course_id, partition_type_id, document_type_id, creation_user_id, creation_time) VALUES (@name, @description, @extension, @size, @unity_id, @course_id, @partition_type_id, @document_type_id, @creation_user_id, now())", conn, tran)) {
							cmd.Parameters.AddWithValue("@name", data.Name);
							cmd.Parameters.AddWithValue("@description", data.Description);
							cmd.Parameters.AddWithValue("@extension", data.Extension);
							cmd.Parameters.AddWithValue("@size", data.Size);
							cmd.Parameters.AddWithValue("@unity_id", data.Unity);
							cmd.Parameters.AddWithValue("@course_id", data.Course);
							cmd.Parameters.AddWithValue("@partition_type_id", data.PartitionType);
							cmd.Parameters.AddWithValue("@document_type_id", data.DocumentType);
							cmd.Parameters.AddWithValue("@creation_user_id", user.Id);
							cmd.ExecuteNonQuery();
						}
						using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn, tran)) {
							id = (int)(ulong)cmd.ExecuteScalar();
						}

						// TODO: create tags

						path = Storage.Document(id, data.Extension);

						using (System.IO.FileStream stream = System.IO.File.Open(path, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.None)) {
							file.CopyTo(stream);
						}

						tran.Commit();
					} catch {
						if (path != null && System.IO.File.Exists(path)) {
							try {
								System.IO.File.Delete(path);
							} catch {
								// Just ignore...
							}
						}
						tran.Rollback();
						throw;
					}
				}
			}

			return new Document(id, data.Name, data.Description, data.Extension, data.Size, null, null, null, null, null);
		}

		public static List<Document> GetAll() {
			List<Document> documents = new List<Document>();
			string dateTimeFormat = Str._DateTimeFormat;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($@"
SELECT d.id, d.name, d.description, d.extension, d.size, d.creation_time,
d.unity_id, un.name{Str._FieldSuffix},
d.course_id, co.name{Str._FieldSuffix},
d.partition_type_id, pt.name{Str._FieldSuffix},
d.document_type_id, dt.name{Str._FieldSuffix}
FROM document d
INNER JOIN unity un ON un.id = d.unity_id
INNER JOIN course co ON co.id = d.course_id
INNER JOIN partition_type pt ON pt.id = d.partition_type_id
INNER JOIN document_type dt ON dt.id = d.document_type_id
ORDER BY un.name{Str._FieldSuffix} ASC, co.name{Str._FieldSuffix} ASC, pt.name{Str._FieldSuffix} ASC, dt.name{Str._FieldSuffix} ASC, d.name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							documents.Add(
								new Document(
									reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetDateTime(5).ToString(dateTimeFormat),
									new IdNamePair(reader.GetInt32(6), reader.GetString(7)),
									new IdNamePair(reader.GetInt32(8), reader.GetString(9)),
									new IdNamePair(reader.GetInt32(10), reader.GetString(11)),
									new IdNamePair(reader.GetInt32(12), reader.GetString(13))
								)
							);
					}
				}
			}
			return documents;
		}

		public static Document GetById(int id, bool full) {
			Document document = null;
			string dateTimeFormat = Str._DateTimeFormat;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($@"
SELECT d.id, d.name, d.description, d.extension, d.size, d.creation_time,
d.unity_id, un.name{Str._FieldSuffix},
d.course_id, co.name{Str._FieldSuffix},
d.partition_type_id, pt.name{Str._FieldSuffix},
d.document_type_id, dt.name{Str._FieldSuffix}
FROM document d
INNER JOIN unity un ON un.id = d.unity_id
INNER JOIN course co ON co.id = d.course_id
INNER JOIN partition_type pt ON pt.id = d.partition_type_id
INNER JOIN document_type dt ON dt.id = d.document_type_id
WHERE d.id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							document = new Document(
								reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetDateTime(5).ToString(dateTimeFormat),
								new IdNamePair(reader.GetInt32(6), reader.GetString(7)),
								new IdNamePair(reader.GetInt32(8), reader.GetString(9)),
								new IdNamePair(reader.GetInt32(10), reader.GetString(11)),
								new IdNamePair(reader.GetInt32(12), reader.GetString(13))
							);
					}

					if (full && document != null) {
						// TODO: load tags
					}
				}
			}
			return document;
		}

		public Document() {
		}

		private Document(int id, string name, string description, string extension, int size, string creationTime, IdNamePair unity, IdNamePair course, IdNamePair partitionType, IdNamePair documentType) {
			Id = id;
			Name = name;
			Description = description;
			Extension = extension;
			Size = size;
			CreationTime = creationTime;
			Unity = unity;
			Course = course;
			PartitionType = partitionType;
			DocumentType = documentType;
		}

		public override string ToString() => Name;

		public void Update(Data data) {
			if (data != null) {
				data.Extension = Extension;
				data.Size = Size;
			}
			Validate(data);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE document SET name = @name, description = @description, unity_id = @unity_id, course_id = @course_id, partition_type_id = @partition_type_id, document_type_id = @document_type_id WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@name", data.Name);
					cmd.Parameters.AddWithValue("@description", data.Description);
					cmd.Parameters.AddWithValue("@unity_id", data.Unity);
					cmd.Parameters.AddWithValue("@course_id", data.Course);
					cmd.Parameters.AddWithValue("@partition_type_id", data.PartitionType);
					cmd.Parameters.AddWithValue("@document_type_id", data.DocumentType);
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();

					Name = data.Name;
					Description = data.Description;
					Unity.Id = data.Unity;
					Course.Id = data.Course;
					PartitionType.Id = data.PartitionType;
					DocumentType.Id = data.DocumentType;
				}
			}
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("DELETE FROM document WHERE id = @id", conn, tran)) {
							cmd.Parameters.AddWithValue("@id", Id);
							cmd.ExecuteNonQuery();
						}

						string path = Storage.Document(Id, Extension);
						if (System.IO.File.Exists(path))
							System.IO.File.Delete(path);

						tran.Commit();
					} catch {
						tran.Rollback();
						throw;
					}
				}
			}
		}
	}
}
