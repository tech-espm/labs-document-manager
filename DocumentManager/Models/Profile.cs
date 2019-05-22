﻿using DocumentManager.Exceptions;
using DocumentManager.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace DocumentManager.Models {
	public class Profile {
		private static readonly Dictionary<int, int[]> PermissionsByProfile = new Dictionary<int, int[]>(16);
		private static readonly CommonReadRareWriteLock Lock = new CommonReadRareWriteLock();

		public const int ADMIN_ID = 1;

		public int Id;
		public string Name;
		public HashSet<int> Features;

		public static List<KeyValuePair<int, string>> GetAllFeatures() {
			List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>((int)Feature.Max + 1);

			list.Add(new KeyValuePair<int, string>((int)Feature.CourseCreate, "Cursos: Criação"));
			list.Add(new KeyValuePair<int, string>((int)Feature.CourseList, "Cursos: Listagem"));
			list.Add(new KeyValuePair<int, string>((int)Feature.CourseEdit, "Cursos: Edição"));
			list.Add(new KeyValuePair<int, string>((int)Feature.CourseDelete, "Cursos: Exclusão"));

			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentCreate, "Documentos: Criação"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentList, "Documentos: Listagem"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentEdit, "Documentos: Edição"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentDelete, "Documentos: Exclusão"));

			return list;
		}

		private static void Validate(ref string name, int[] features) {
			if (string.IsNullOrWhiteSpace(name))
				throw new ValidationException("Nome inválido!");
			if ((name = name.Trim().ToUpper()).Length > 64)
				throw new ValidationException("Nome muito longo!");

			if (features != null) {
				for (int i = features.Length - 1; i >= 0; i--) {
					if (features[i] > (int)Feature.Max || features[i] < (int)Feature.Min)
						throw new ValidationException("Permissão inválida!");
				}
			}
		}

		public static Profile Create(string name, int[] features) {
			Validate(ref name, features);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("INSERT INTO profile (name) VALUES (@name)", conn, tran)) {
							cmd.Parameters.AddWithValue("@name", name);
							cmd.ExecuteNonQuery();
						}
						using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn, tran)) {
							id = (int)(ulong)cmd.ExecuteScalar();
						}
						if (features != null && features.Length > 0) {
							using (MySqlCommand cmd = new MySqlCommand("INSERT INTO profile_feature (profile_id, feature_id) VALUES (@profile_id, @feature_id)", conn, tran)) {
								cmd.Parameters.AddWithValue("@profile_id", id);
								cmd.Parameters.Add("@feature_id", MySqlDbType.Int32);
								for (int i = 0; i < features.Length; i++) {
									cmd.Parameters[1].Value = features[i];
									cmd.ExecuteNonQuery();
								}
							}
						}
					} catch {
						tran.Rollback();
						throw;
					}
					tran.Commit();
				}
			}

			return new Profile(id, name);
		}

		public static List<Profile> GetAll() {
			List<Profile> profiles = new List<Profile>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM profile ORDER BY name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							profiles.Add(new Profile(reader.GetInt32(0), reader.GetString(1)));
					}
				}
			}
			return profiles;
		}

		public static Profile GetById(int id, bool full) {
			Profile profile = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM profile WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							profile = new Profile(reader.GetInt32(0), reader.GetString(1));
					}
				}
				if (full && profile != null) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT feature_id FROM profile_feature WHERE profile_id = @profile_id ORDER BY feature_id ASC", conn)) {
						cmd.Parameters.AddWithValue("@profile_id", id);
						profile.Features = new HashSet<int>();
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							while (reader.Read())
								profile.Features.Add(reader.GetInt32(0));
						}
					}
				}
			}
			return profile;
		}

		private static void PurgeCachedPermissionsByProfileId(int profileId) {
			try {
				Lock.EnterWriteLock();
				PermissionsByProfile.Remove(profileId);
			} finally {
				Lock.ExitWriteLock();
			}
		}

		public static int[] GetCachedPermissionsByProfileId(int profileId) {
			int[] list;
			try {
				Lock.EnterReadLock();
				PermissionsByProfile.TryGetValue(profileId, out list);
			} finally {
				Lock.ExitReadLock();
			}
			if (list == null) {
				try {
					Lock.EnterWriteLock();
					if (!PermissionsByProfile.TryGetValue(profileId, out list)) {
						List<int> features = new List<int>();
						using (MySqlConnection conn = Sql.OpenConnection()) {
							using (MySqlCommand cmd = new MySqlCommand("SELECT feature_id FROM profile_feature WHERE profile_id = @profile_id ORDER BY feature_id ASC", conn)) {
								cmd.Parameters.AddWithValue("@profile_id", profileId);
								using (MySqlDataReader reader = cmd.ExecuteReader()) {
									while (reader.Read())
										features.Add(reader.GetInt32(0));
								}
							}
						}
						PermissionsByProfile[profileId] = (list = features.ToArray());
					}
				} finally {
					Lock.ExitWriteLock();
				}
			}
			return list;
		}

		public Profile() {
		}

		private Profile(int id, string name) {
			Id = id;
			Name = name;
		}

		public override string ToString() {
			return Name;
		}

		public void Update(string name, int[] features) {
			if (Id == ADMIN_ID)
				throw new ValidationException("Não é permitido editar o perfil \"ADMINISTRADOR\"!");

			Validate(ref name, features);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("UPDATE profile SET name = @name WHERE id = @id", conn, tran)) {
							cmd.Parameters.AddWithValue("@name", name);
							cmd.Parameters.AddWithValue("@id", Id);
							cmd.ExecuteNonQuery();
							Name = name;
						}
						using (MySqlCommand cmd = new MySqlCommand("DELETE FROM profile_feature WHERE profile_id = @profile_id", conn, tran)) {
							cmd.Parameters.AddWithValue("@profile_id", Id);
							cmd.ExecuteNonQuery();
						}
						if (features != null && features.Length > 0) {
							using (MySqlCommand cmd = new MySqlCommand("INSERT INTO profile_feature (profile_id, feature_id) VALUES (@profile_id, @feature_id)", conn, tran)) {
								cmd.Parameters.AddWithValue("@profile_id", Id);
								cmd.Parameters.Add("@feature_id", MySqlDbType.Int32);
								for (int i = 0; i < features.Length; i++) {
									cmd.Parameters[1].Value = features[i];
									cmd.ExecuteNonQuery();
								}
							}
						}
					} catch {
						tran.Rollback();
						throw;
					}
					tran.Commit();
				}
			}

			Menu.PurgeCachedMenusByProfileId(Id);
			PurgeCachedPermissionsByProfileId(Id);
			User.PurgeAllCachedUsers();
		}

		public void Delete() {
			if (Id == ADMIN_ID)
				throw new ValidationException("Não é permitido excluir o perfil \"ADMINISTRADOR\"!");
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("DELETE FROM profile WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}
			Menu.PurgeCachedMenusByProfileId(Id);
			PurgeCachedPermissionsByProfileId(Id);
			User.PurgeAllCachedUsers();
		}
	}
}
