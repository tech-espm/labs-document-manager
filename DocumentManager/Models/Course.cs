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
	public class Course {
		public static readonly MemoryCache<Course[][]> CachedCourses = new MemoryCache<Course[][]>(CacheStorageRefresher);

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

		public static Course Create(string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			Validate(ref nameEn, ref namePtBr, ref shortNameEn, ref shortNamePtBr);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO course (name_en, name_ptbr, short_name_en, short_name_ptbr) VALUES (@name_en, @name_ptbr, @short_name_en, @short_name_ptbr)", conn)) {
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

			CachedCourses.Refresh();

			return new Course(id, new Str(nameEn, namePtBr), new Str(shortNameEn, shortNamePtBr));
		}

		private static Course[][] CacheStorageRefresher() {
			List<Course> courses = new List<Course>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($"SELECT id, name_en, name_ptbr, short_name_en, short_name_ptbr FROM course", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							courses.Add(new Course(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2)), new Str(reader.GetString(3), reader.GetString(4))));
					}
				}
			}
			Course[] arrayEn, arrayPtBr;
			Array.Sort(arrayEn = courses.ToArray(), (a, b) => a.Name.ValueEn.CompareTo(b.Name.ValueEn));
			Array.Sort(arrayPtBr = courses.ToArray(), (a, b) => a.Name.ValuePtBr.CompareTo(b.Name.ValuePtBr));
			// Ordered by language
			// Str.LanguagePtBr = 0
			// Str.LanguageEn = 1
			return new Course[][] { arrayPtBr, arrayEn };
		}

		public static Course[] GetAll() {
			Course[][] cachedCourses = CachedCourses.StartReading();
			try {
				return cachedCourses?[Str.CurrentLanguage];
			} finally {
				CachedCourses.FinishReading();
			}
		}

		public static Course GetById(int id) {
			// Since all course objects are cached in memory with all properties
			// set, and since there are not too many of those objects, it is faster
			// to look up for one of them here, instead of reading it from the database
			Course[][] cachedCourses = CachedCourses.StartReading();
			try {
				Course[] courses;
				if ((courses = cachedCourses?[0]) != null) {
					for (int i = courses.Length - 1; i >= 0; i--) {
						if (courses[i].Id == id)
							return courses[i];
					}
				}
				return null;
			} finally {
				CachedCourses.FinishReading();
			}
		}

		public Course() {
		}

		private Course(int id, Str name, Str shortName) {
			Id = id;
			Name = name;
			ShortName = shortName;
		}

		public override string ToString() => Name.ToString();

		public void Update(string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			Validate(ref nameEn, ref namePtBr, ref shortNameEn, ref shortNamePtBr);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE course SET name_en = @name_en, name_ptbr = @name_ptbr, short_name_en = @short_name_en, short_name_ptbr = @short_name_ptbr WHERE id = @id", conn)) {
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

			CachedCourses.Refresh();
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM course WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}

			CachedCourses.Refresh();
			User.PurgeAllCachedUsers();
		}
	}
}
