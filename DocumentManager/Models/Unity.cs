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
	public class Unity {
		public static readonly MemoryCache<Unity[][]> CachedUnits = new MemoryCache<Unity[][]>(CacheStorageRefresher);

		public int Id;
		public Str Name, ShortName;

		private static void Validate(ref string nameEn, ref string namePtBr, ref string shortNameEn, ref string shortNamePtBr) {
			if (string.IsNullOrWhiteSpace(nameEn) ||
				string.IsNullOrWhiteSpace(namePtBr))
				throw new ValidationException(Str.InvalidName);
			if ((nameEn = nameEn.Trim().ToUpper()).Length > 64 ||
				(namePtBr = namePtBr.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (string.IsNullOrWhiteSpace(shortNameEn) ||
				string.IsNullOrWhiteSpace(shortNamePtBr))
				throw new ValidationException(Str.InvalidShortName);
			if ((shortNameEn = shortNameEn.Trim().ToUpper()).Length > 16 ||
				(shortNamePtBr = shortNamePtBr.Trim().ToUpper()).Length > 16)
				throw new ValidationException(Str.ShortNameTooLong);
		}

		public static Unity Create(string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			Validate(ref nameEn, ref namePtBr, ref shortNameEn, ref shortNamePtBr);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO unity (name_en, name_ptbr, short_name_en, short_name_ptbr) VALUES (@name_en, @name_ptbr, @short_name_en, @short_name_ptbr)", conn)) {
					cmd.Parameters.AddWithValue("@name_en", nameEn);
					cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
					cmd.Parameters.AddWithValue("@short_name_en", shortNameEn);
					cmd.Parameters.AddWithValue("@short_name_ptbr", shortNamePtBr);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			CachedUnits.Refresh();

			return new Unity(id, new Str(nameEn, namePtBr), new Str(shortNameEn, shortNamePtBr));
		}

		private static Unity[][] CacheStorageRefresher() {
			List<Unity> units = new List<Unity>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($"SELECT id, name_en, name_ptbr, short_name_en, short_name_ptbr FROM unity ORDER BY name{Str._FieldSuffix} ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							units.Add(new Unity(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2)), new Str(reader.GetString(3), reader.GetString(4))));
					}
				}
			}
			Unity[] arrayEn, arrayPtBr;
			Array.Sort(arrayEn = units.ToArray(), (a, b) => a.Name.ValueEn.CompareTo(b.Name.ValueEn));
			Array.Sort(arrayPtBr = units.ToArray(), (a, b) => a.Name.ValuePtBr.CompareTo(b.Name.ValuePtBr));
			// Ordered by language
			// Str.LanguagePtBr = 0
			// Str.LanguageEn = 1
			return new Unity[][] { arrayPtBr, arrayEn };
		}

		public static Unity[] GetAll() {
			Unity[][] cachedUnits = CachedUnits.StartReading();
			try {
				return cachedUnits?[Str.CurrentLanguage];
			} finally {
				CachedUnits.FinishReading();
			}
		}

		public static Unity GetById(int id) {
			// Since all unity objects are cached in memory with all properties
			// set, and since there are not too many of those objects, it is faster
			// to look up for one of them here, instead of reading it from the database
			Unity[][] cachedUnits = CachedUnits.StartReading();
			try {
				Unity[] units;
				if ((units = cachedUnits?[0]) != null) {
					for (int i = units.Length - 1; i >= 0; i--) {
						if (units[i].Id == id)
							return units[i];
					}
				}
				return null;
			} finally {
				CachedUnits.FinishReading();
			}
		}

		public Unity() {
		}

		private Unity(int id, Str name, Str shortName) {
			Id = id;
			Name = name;
			ShortName = shortName;
		}

		public override string ToString() => Name.ToString();

		public void Update(string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			Validate(ref nameEn, ref namePtBr, ref shortNameEn, ref shortNamePtBr);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE unity SET name_en = @name_en, name_ptbr = @name_ptbr, short_name_en = @short_name_en, short_name_ptbr = @short_name_ptbr WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@name_en", nameEn);
					cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
					cmd.Parameters.AddWithValue("@short_name_en", shortNameEn);
					cmd.Parameters.AddWithValue("@short_name_ptbr", shortNamePtBr);
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
					Name = new Str(nameEn, namePtBr);
					ShortName = new Str(shortNameEn, shortNamePtBr);
				}
			}

			CachedUnits.Refresh();
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM unity WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}

			CachedUnits.Refresh();
		}
	}
}
