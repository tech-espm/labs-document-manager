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
	public class Profile {
		private static readonly Dictionary<int, int[]> PermissionsByProfile = new Dictionary<int, int[]>(16);
		private static readonly CommonReadRareWriteLock Lock = new CommonReadRareWriteLock();

		public const int ADMIN_ID = 1;

		public int Id;
		public Str Name;
		public HashSet<int> Features;

		public static List<KeyValuePair<int, string>> GetAllFeatures() {
			List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>((int)Feature.Max + 1);

			list.Add(new KeyValuePair<int, string>((int)Feature.UnityCreate, $"{Str.Courses}: {Str.Creation}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.UnityList, $"{Str.Courses}: {Str.Listing}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.UnityEdit, $"{Str.Courses}: {Str.Edition}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.UnityDelete, $"{Str.Courses}: {Str.Deletion}"));

			list.Add(new KeyValuePair<int, string>((int)Feature.CourseCreate, $"{Str.Courses}: {Str.Creation}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.CourseList, $"{Str.Courses}: {Str.Listing}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.CourseEdit, $"{Str.Courses}: {Str.Edition}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.CourseDelete, $"{Str.Courses}: {Str.Deletion}"));

			list.Add(new KeyValuePair<int, string>((int)Feature.PartitionTypeCreate, $"{Str.PartitionTypes}: {Str.Creation}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.PartitionTypeList, $"{Str.PartitionTypes}: {Str.Listing}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.PartitionTypeEdit, $"{Str.PartitionTypes}: {Str.Edition}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.PartitionTypeDelete, $"{Str.PartitionTypes}: {Str.Deletion}"));

			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentTypeCreate, $"{Str.DocumentTypes}: {Str.Creation}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentTypeList, $"{Str.DocumentTypes}: {Str.Listing}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentTypeEdit, $"{Str.DocumentTypes}: {Str.Edition}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentTypeDelete, $"{Str.DocumentTypes}: {Str.Deletion}"));

			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentCreate, $"{Str.Documents}: {Str.Creation}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentList, $"{Str.Documents}: {Str.Listing}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentEdit, $"{Str.Documents}: {Str.Edition}"));
			list.Add(new KeyValuePair<int, string>((int)Feature.DocumentDelete, $"{Str.Documents}: {Str.Deletion}"));

			return list;
		}

		private static void Validate(ref string nameEn, ref string namePtBr, int[] features) {
			if (string.IsNullOrWhiteSpace(nameEn))
				throw new ValidationException(Str.InvalidName);
			if ((nameEn = nameEn.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (string.IsNullOrWhiteSpace(namePtBr))
				throw new ValidationException(Str.InvalidName);
			if ((namePtBr = namePtBr.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.NameTooLong);

			if (features != null) {
				for (int i = features.Length - 1; i >= 0; i--) {
					if (features[i] > (int)Feature.Max || features[i] < (int)Feature.Min)
						throw new ValidationException(Str.InvalidPermission);
				}
			}
		}

		public static Profile Create(string nameEn, string namePtBr, int[] features) {
			Validate(ref nameEn, ref namePtBr, features);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("INSERT INTO profile (name_en, name_ptbr) VALUES (@name_en, @name_ptbr)", conn, tran)) {
							cmd.Parameters.AddWithValue("@name_en", nameEn);
							cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
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

			return new Profile(id, new Str(nameEn, namePtBr));
		}

		public static List<Profile> GetAll() {
			List<Profile> profiles = new List<Profile>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr FROM profile ORDER BY name_en ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							profiles.Add(new Profile(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2))));
					}
				}
			}
			return profiles;
		}

		public static Profile GetById(int id, bool full) {
			Profile profile = null;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, name_en, name_ptbr FROM profile WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (reader.Read())
							profile = new Profile(reader.GetInt32(0), new Str(reader.GetString(1), reader.GetString(2)));
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

		private Profile(int id, Str name) {
			Id = id;
			Name = name;
		}

		public override string ToString() => Name.ToString();

		public void Update(string nameEn, string namePtBr, int[] features) {
			if (Id == ADMIN_ID)
				throw new ValidationException(Str.EditingProfileNotAllowed);

			Validate(ref nameEn, ref namePtBr, features);

			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlTransaction tran = conn.BeginTransaction()) {
					try {
						using (MySqlCommand cmd = new MySqlCommand("UPDATE profile SET name_en = @name_en, name_ptbr = @name_ptbr WHERE id = @id", conn, tran)) {
							cmd.Parameters.AddWithValue("@name_en", nameEn);
							cmd.Parameters.AddWithValue("@name_ptbr", namePtBr);
							cmd.Parameters.AddWithValue("@id", Id);
							cmd.ExecuteNonQuery();
							Name = new Str(nameEn, namePtBr);
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
				throw new ValidationException(Str.DeletingProfileNotAllowed);
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
