﻿using DocumentManager.Exceptions;
using DocumentManager.Localization;
using DocumentManager.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace DocumentManager.Models {
	public class Tag {
		public static readonly MemoryCache<Tag[]> CachedTags = new MemoryCache<Tag[]>(CacheStorageRefresher);

		public class Value {
			public int Id, Position;
			public Str Name;
		}

		public int Id;
		public Str Name;
		public List<Value> Values;

		private static void Validate(ref string nameEn, ref string namePtBr, Value[] existingValues, Value[] newValues, out List<Value> valuesToCreate, out List<Value> valuesToUpdate, out List<Value> valuesToDelete) {
			if (string.IsNullOrWhiteSpace(nameEn) ||
				string.IsNullOrWhiteSpace(namePtBr))
				throw new ValidationException(Str.InvalidName);
			if ((nameEn = nameEn.Trim().ToUpper()).Length > 64 ||
				(namePtBr = namePtBr.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (newValues != null) {
				for (int i = newValues.Length - 1; i >= 0; i--) {
					Value newValue = newValues[i];
					if (newValue == null ||
						newValue.Name == null ||
						newValue.Name.IsNullOrWhiteSpace)
						throw new ValidationException(Str.InvalidValue);
					newValue.Name.Normalize();
					if (newValue.Name.ValueEn.Length > 64 ||
						newValue.Name.ValuePtBr.Length > 64)
						throw new ValidationException(Str.ValueTooLong);
				}
			}

			valuesToCreate = new List<Value>();
			valuesToUpdate = new List<Value>();
			valuesToDelete = new List<Value>();

			if (newValues == null || newValues.Length == 0) {
				// Delete all values
				if (existingValues != null && existingValues.Length > 0)
					valuesToDelete.AddRange(existingValues);
			} else if (existingValues == null || existingValues.Length == 0) {
				// Create all values
				valuesToCreate.AddRange(newValues);
			} else {
				// Merge (This is O(n²), but since n is too small, the
				// performance difference is not noticeable)
				for (int i = existingValues.Length - 1; i >= 0; i--) {
					Value existingValue = existingValues[i];
					for (int j = newValues.Length - 1; j >= 0; j--) {
						Value newValue = newValues[j];
						if (newValue == null)
							continue;
						if (existingValue.Id == newValue.Id) {
							existingValues[i] = null;
							newValues[j] = null;
							if (existingValue.Position != newValue.Position ||
								!existingValue.Name.Equals(newValue.Name))
								valuesToUpdate.Add(newValue);
							break;
						}
					}
				}

				// Maybe the user has deleted and recreated the value, so it would appear
				// inside newValues with an id of 0, but the same name as another
				// existing value (in which case, we should just ignore this new value)
				for (int i = newValues.Length - 1; i >= 0; i--) {
					Value newValue = newValues[i];
					if (newValue == null || newValue.Id != 0)
						continue;
					for (int j = existingValues.Length - 1; j >= 0; j--) {
						Value existingValue = existingValues[j];
						if (existingValue == null)
							continue;
						if (existingValue.Name.Equals(newValue.Name)) {
							existingValues[i] = null;
							newValues[j] = null;
							if (existingValue.Position != newValue.Position) {
								newValue.Id = existingValue.Id;
								valuesToUpdate.Add(newValue);
							}
							break;
						}
					}
				}

				// All remaining values inside newValues must be created,
				// whereas all values still inside existingValues must be deleted
				for (int i = newValues.Length - 1; i >= 0; i--) {
					Value newValue = newValues[i];
					if (newValue != null)
						valuesToCreate.Add(newValue);
				}

				for (int i = existingValues.Length - 1; i >= 0; i--) {
					Value existingValue = existingValues[i];
					if (existingValue != null)
						valuesToDelete.Add(existingValue);
				}
			}
		}

		public static void SyncValuesInDatabase(MySqlConnection conn, MySqlTransaction tran, int tagId, List<Value> valuesToCreate, List<Value> valuesToUpdate, List<Value> valuesToDelete) {
			if (valuesToDelete.Count > 0) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM tag_value WHERE id = @id", conn, tran)) {
					cmd.Parameters.AddWithValue("@id", 0);
					for (int i = valuesToDelete.Count - 1; i >= 0; i--) {
						cmd.Parameters[0].Value = valuesToDelete[i].Id;
						cmd.ExecuteNonQuery();
					}
				}
			}

			if (valuesToUpdate.Count > 0) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE tag_value SET position = @position, name_en = @name_en, name_ptbr = @name_ptbr WHERE id = @id", conn, tran)) {
					cmd.Parameters.AddWithValue("@position", 0);
					cmd.Parameters.AddWithValue("@name_en", "");
					cmd.Parameters.AddWithValue("@name_ptbr", "");
					cmd.Parameters.AddWithValue("@id", 0);
					for (int i = valuesToUpdate.Count - 1; i >= 0; i--) {
						Value valueToUpdate = valuesToUpdate[i];
						cmd.Parameters[0].Value = valueToUpdate.Position;
						cmd.Parameters[1].Value = valueToUpdate.Name.ValueEn;
						cmd.Parameters[2].Value = valueToUpdate.Name.ValuePtBr;
						cmd.Parameters[3].Value = valueToUpdate.Id;
						cmd.ExecuteNonQuery();
					}
				}
			}

			if (valuesToCreate.Count > 0) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO tag_value (tag_id, position, name_en, name_ptbr) VALUES (@tag_id, @position, @name_en, @name_ptbr)", conn, tran)) {
					cmd.Parameters.AddWithValue("@tag_id", tagId);
					cmd.Parameters.AddWithValue("@position", 0);
					cmd.Parameters.AddWithValue("@name_en", "");
					cmd.Parameters.AddWithValue("@name_ptbr", "");
					for (int i = valuesToCreate.Count - 1; i >= 0; i--) {
						Value valueToCreate = valuesToCreate[i];
						cmd.Parameters[1].Value = valueToCreate.Position;
						cmd.Parameters[2].Value = valueToCreate.Name.ValueEn;
						cmd.Parameters[3].Value = valueToCreate.Name.ValuePtBr;
						cmd.ExecuteNonQuery();
						using (MySqlCommand cmd2 = new MySqlCommand("SELECT last_insert_id()", conn, tran)) {
							valueToCreate.Id = (int)(ulong)cmd2.ExecuteScalar();
						}
					}
				}
			}
		}

		public static Tag Create(string nameEn, string namePtBr, Value[] values) {
			Validate(ref nameEn, ref namePtBr, null, values, out List<Value> valuesToCreate, out List<Value> valuesToUpdate, out List<Value> valuesToDelete);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("INSERT INTO tag (name_en, name_ptbr) VALUES (@name_en, @name_ptbr)", conn, tran)) {
							cmd.Parameters.AddWithValue("@name_en", nameEn);
							cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
							cmd.ExecuteNonQuery();
						}
						using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn, tran)) {
							id = (int)(ulong)cmd.ExecuteScalar();
						}
						SyncValuesInDatabase(conn, tran, id, valuesToCreate, valuesToUpdate, valuesToDelete);
					} catch {
						tran.Rollback();
						throw;
					}
					tran.Commit();
				}
			}

			CachedTags.Refresh();

			valuesToCreate.Sort((a, b) => a.Position - b.Position);

			return new Tag(id, new Str(nameEn, namePtBr)) {
				Values = valuesToCreate
			};
		}

		private static Tag[] CacheStorageRefresher() {
			List<Tag> tags = new List<Tag>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($"SELECT t.id, t.name_en, t.name_ptbr, v.id, v.position, v.name_en, v.name_ptbr FROM tag t INNER JOIN tag_value v ON v.tag_id = t.id ORDER BY t.name{Str._FieldSuffix} ASC, v.position ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						Tag lastTag = new Tag();
						while (reader.Read()) {
							int id = reader.GetInt32(0);
							if (id != lastTag.Id) {
								lastTag = new Tag(id, new Str(reader.GetString(1), reader.GetString(2))) {
									Values = new List<Value>()
								};
								tags.Add(lastTag);
							}
							lastTag.Values.Add(new Value() {
								Id = reader.GetInt32(3),
								Position = reader.GetInt32(4),
								Name = new Str(reader.GetString(5), reader.GetString(6))
							});
						}
					}
				}
			}
			return tags.ToArray();
		}

		public static Tag[] GetAll() {
			Tag[] cachedTags = CachedTags.StartReading();
			try {
				return cachedTags;
			} finally {
				CachedTags.FinishReading();
			}
		}

		public static Tag GetById(int id) {
			Tag tag = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr FROM tag WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							tag = new Tag(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2))) {
								Values = new List<Value>()
							};
					}
				}
				if (tag != null) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT id, position, name_en, name_ptbr FROM tag_value WHERE tag_id = @tag_id ORDER BY position ASC", conn)) {
						cmd.Parameters.AddWithValue("@tag_id", id);
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							while (reader.Read())
								tag.Values.Add(new Value() {
									Id = reader.GetInt32(0),
									Position = reader.GetInt32(1),
									Name = new Str(reader.GetString(2), reader.GetString(3))
								});
						}
					}
				}
			}
			return tag;
		}

		public Tag() {
		}

		private Tag(int id, Str name) {
			Id = id;
			Name = name;
		}

		public override string ToString() => Name.ToString();

		public void Update(string nameEn, string namePtBr, Value[] values) {
			// ToArray() because Validate() changes the array
			Validate(ref nameEn, ref namePtBr, Values.ToArray(), values, out List<Value> valuesToCreate, out List<Value> valuesToUpdate, out List<Value> valuesToDelete);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("UPDATE tag SET name_en = @name_en, name_ptbr = @name_ptbr WHERE id = @id", conn, tran)) {
							cmd.Parameters.AddWithValue("@name_en", nameEn);
							cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
							cmd.Parameters.AddWithValue("@id", Id);
							cmd.ExecuteNonQuery();
							Name = new Str(nameEn, namePtBr);
						}
						SyncValuesInDatabase(conn, tran, Id, valuesToCreate, valuesToUpdate, valuesToDelete);
						Values = valuesToCreate;
						Values.AddRange(valuesToUpdate);
						Values.Sort((a, b) => a.Position - b.Position);
					} catch {
						tran.Rollback();
						throw;
					}
					tran.Commit();
				}
			}

			CachedTags.Refresh();
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM tag WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}

			CachedTags.Refresh();
		}
	}
}
