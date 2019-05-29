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
		public int Id;
		public Str Name, ShortName;

		private static void Validate(ref string nameEn, ref string namePtBr, ref string shortNameEn, ref string shortNamePtBr) {
			if (string.IsNullOrWhiteSpace(nameEn))
				throw new ValidationException(Str.InvalidName);
			if ((nameEn = nameEn.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (string.IsNullOrWhiteSpace(namePtBr))
				throw new ValidationException(Str.InvalidName);
			if ((namePtBr = namePtBr.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (string.IsNullOrWhiteSpace(shortNameEn))
				throw new ValidationException(Str.InvalidShortName);
			if ((shortNameEn = shortNameEn.Trim().ToUpper()).Length > 16)
				throw new ValidationException(Str.ShortNameTooLong);

			if (string.IsNullOrWhiteSpace(shortNamePtBr))
				throw new ValidationException(Str.InvalidShortName);
			if ((shortNamePtBr = shortNamePtBr.Trim().ToUpper()).Length > 16)
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

			return new Course(id, new Str(nameEn, namePtBr), new Str(shortNameEn, shortNamePtBr));
		}

		public static List<Course> GetAll() {
			List<Course> courses = new List<Course>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr, short_name_en, short_name_ptbr FROM course ORDER BY name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							courses.Add(new Course(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2)), new Str(reader.GetString(3), reader.GetString(4))));
					}
				}
			}
			return courses;
		}

		public static Course GetById(int id) {
			Course course = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr, short_name_en, short_name_ptbr FROM course WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							course = new Course(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2)), new Str(reader.GetString(3), reader.GetString(4)));
					}
				}
			}
			return course;
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
		}

		public void Delete() {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM course WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}
