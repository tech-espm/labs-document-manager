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
	public class PartitionType {
		public static readonly MemoryCache<PartitionType[]> CachedPartitionTypes = new MemoryCache<PartitionType[]>(CacheStorageRefresher);

		public int Id;
		public Str Name;

		private static void Validate(ref string nameEn, ref string namePtBr) {
			if (string.IsNullOrWhiteSpace(nameEn) ||
				string.IsNullOrWhiteSpace(namePtBr))
				throw new ValidationException(Str.InvalidName);
			if ((nameEn = nameEn.Trim().ToUpper()).Length > 64 ||
				(namePtBr = namePtBr.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);
		}

		public static PartitionType Create(string nameEn, string namePtBr) {
			Validate(ref nameEn, ref namePtBr);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO partition_type (name_en, name_ptbr) VALUES (@name_en, @name_ptbr)", conn)) {
					cmd.Parameters.AddWithValue("@name_en", nameEn);
					cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			CachedPartitionTypes.Refresh();

			return new PartitionType(id, new Str(nameEn, namePtBr));
		}

		private static PartitionType[] CacheStorageRefresher() {
			List<PartitionType> partitionTypes = new List<PartitionType>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($"SELECT id, name_en, name_ptbr FROM partition_type ORDER BY name{Str._FieldSuffix} ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							partitionTypes.Add(new PartitionType(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2))));
					}
				}
			}
			return partitionTypes.ToArray();
		}

		public static PartitionType[] GetAll() {
			PartitionType[] cachedPartitionTypes = CachedPartitionTypes.StartReading();
			try {
				return cachedPartitionTypes;
			} finally {
				CachedPartitionTypes.FinishReading();
			}
		}

		public static PartitionType GetById(int id) {
			PartitionType course = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr FROM partition_type WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							course = new PartitionType(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2)));
					}
				}
			}
			return course;
		}

		public PartitionType() {
		}

		private PartitionType(int id, Str name) {
			Id = id;
			Name = name;
		}

		public override string ToString() => Name.ToString();

		public void Update(string nameEn, string namePtBr) {
			Validate(ref nameEn, ref namePtBr);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE partition_type SET name_en = @name_en, name_ptbr = @name_ptbr WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@name_en", nameEn);
					cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
					Name = new Str(nameEn, namePtBr);
				}
			}

			CachedPartitionTypes.Refresh();
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM partition_type WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}

			CachedPartitionTypes.Refresh();
		}
	}
}
