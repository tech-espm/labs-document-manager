using DocumentManager.Exceptions;
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
		public string Name;

		private static void Validate(ref string name) {
			if (string.IsNullOrWhiteSpace(name))
				throw new ValidationException("Nome inválido!");
			if ((name = name.Trim().ToUpper()).Length > 64)
				throw new ValidationException("Nome muito longo!");
		}

		public static PartitionType Create(string name) {
			Validate(ref name);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO partition_type (name) VALUES (@name)", conn)) {
					cmd.Parameters.AddWithValue("@name", name);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			CachedPartitionTypes.Refresh();

			return new PartitionType(id, name);
		}

		private static PartitionType[] CacheStorageRefresher() {
			List<PartitionType> partitionTypes = new List<PartitionType>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM partition_type ORDER BY name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							partitionTypes.Add(new PartitionType(reader.GetInt32(0), reader.GetString(1)));
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
			PartitionType partitionType = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM partition_type WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							partitionType = new PartitionType(reader.GetInt32(0), reader.GetString(1));
					}
				}
			}
			return partitionType;
		}

		public PartitionType() {
		}

		private PartitionType(int id, string name) {
			Id = id;
			Name = name;
		}

		public override string ToString() {
			return Name;
		}

		public void Update(string name) {
			Validate(ref name);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE partition_type SET name = @name WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@name", name);
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
					Name = name;
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
