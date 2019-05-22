using DocumentManager.Exceptions;
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
		public string Name, ShortName;

		private static void Validate(ref string name, ref string shortName) {
			if (string.IsNullOrWhiteSpace(name))
				throw new ValidationException("Nome inválido!");
			if ((name = name.Trim().ToUpper()).Length > 64)
				throw new ValidationException("Nome muito longo!");

			if (string.IsNullOrWhiteSpace(shortName))
				throw new ValidationException("Apelido inválido!");
			if ((shortName = shortName.Trim().ToUpper()).Length > 16)
				throw new ValidationException("Apelido muito longo!");
		}

		public static Course Create(string name, string shortName) {
			Validate(ref name, ref shortName);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO course (name, short_name) VALUES (@name, @short_name)", conn)) {
					cmd.Parameters.AddWithValue("@name", name);
					cmd.Parameters.AddWithValue("@short_name", shortName);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			return new Course(id, name, shortName);
		}

		public static List<Course> GetAll() {
			List<Course> courses = new List<Course>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name, short_name FROM course ORDER BY name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							courses.Add(new Course(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
					}
				}
			}
			return courses;
		}

		public static Course GetById(int id) {
			Course course = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name, short_name FROM course WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							course = new Course(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
					}
				}
			}
			return course;
		}

		public Course() {
		}

		private Course(int id, string name, string shortName) {
			Id = id;
			Name = name;
			ShortName = shortName;
		}

		public override string ToString() {
			return Name;
		}

		public void Update(string name, string shortName) {
			Validate(ref name, ref shortName);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE course SET name = @name, short_name = @short_name WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@name", name);
					cmd.Parameters.AddWithValue("@short_name", shortName);
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
					Name = name;
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
