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
		public static readonly MemoryCache<PartitionType[][]> CachedPartitionTypes = new MemoryCache<PartitionType[][]>(CacheStorageRefresher);

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
			User.PurgeAllCachedUsers();

			return new PartitionType(id, new Str(nameEn, namePtBr));
		}

		private static PartitionType[][] CacheStorageRefresher() {
			List<PartitionType> partitionTypes = new List<PartitionType>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($"SELECT id, name_en, name_ptbr FROM partition_type ORDER BY name{Str._FieldSuffix} ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							partitionTypes.Add(new PartitionType(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2))));
					}
				}
			}
			PartitionType[] arrayEn, arrayPtBr;
			Array.Sort(arrayEn = partitionTypes.ToArray(), (a, b) => a.Name.ValueEn.CompareTo(b.Name.ValueEn));
			Array.Sort(arrayPtBr = partitionTypes.ToArray(), (a, b) => a.Name.ValuePtBr.CompareTo(b.Name.ValuePtBr));
			// Ordered by language
			// Str.LanguagePtBr = 0
			// Str.LanguageEn = 1
			return new PartitionType[][] { arrayPtBr, arrayEn };
		}

		public static PartitionType[] GetAll() {
			PartitionType[][] cachedPartitionTypes = CachedPartitionTypes.StartReading();
			try {
				return cachedPartitionTypes?[Str.CurrentLanguage];
			} finally {
				CachedPartitionTypes.FinishReading();
			}
		}

		public static PartitionType GetById(int id) {
			// Since all partition type objects are cached in memory with all properties
			// set, and since there are not too many of those objects, it is faster
			// to look up for one of them here, instead of reading it from the database
			PartitionType[][] cachedPartitionTypes = CachedPartitionTypes.StartReading();
			try {
				PartitionType[] partitionTypes;
				if ((partitionTypes = cachedPartitionTypes?[0]) != null) {
					for (int i = partitionTypes.Length - 1; i >= 0; i--) {
						if (partitionTypes[i].Id == id)
							return partitionTypes[i];
					}
				}
				return null;
			} finally {
				CachedPartitionTypes.FinishReading();
			}
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
			User.PurgeAllCachedUsers();
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM partition_type WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}

			CachedPartitionTypes.Refresh();
			User.PurgeAllCachedUsers();
		}
	}
}
