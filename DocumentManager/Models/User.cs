using DocumentManager.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using DocumentManager.Exceptions;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Localization;

namespace DocumentManager.Models {
	public class User {
		public enum ValidationResult {
			OK = 1,
			NotLoggedIn = 0,
			LoggedInButNoPermission = -1
		}

		[Flags]
		public enum FeaturePermissionResult {
			None = 0,
			Create = 1,
			Edit = 2,
			Download = 4,
			Delete = 8
		}

		public class DocumentTypePermission {

			public int ID { get; set; }
			public int UserID { get; set; }
			public int UnityID { get; set; }
			public int CourseID { get; set; }
			public int DocumentTypeId { get; set; }
			public FeaturePermissionResult FeaturePermission { get; set; }
			public DocumentType DocumentType { get; set; }

			public bool Delete(int id) {


				using (MySqlConnection conn = Sql.OpenConnection()) {
					StringBuilder query = new StringBuilder();
					query.Append("DELETE FROM user_permission_document_type ");
					query.Append("WHERE id = @id ");


					using (MySqlCommand cmd = new MySqlCommand(query.ToString(), conn)) {
						cmd.Parameters.AddWithValue("@id", id);
						int rows = cmd.ExecuteNonQuery();
						return rows > 0;

					}

				}

			}


			public bool Add(int UserId, int UnityId, int CourseId, int DocumentTypeId, int FeaturePermissionId) {


				using (MySqlConnection conn = Sql.OpenConnection()) {
					StringBuilder query = new StringBuilder();
					query.Append("INSERT INTO user_permission_document_type ");
					query.Append("        (user_id, ");
					query.Append("         unity_id, ");
					query.Append("         course_id, ");
					query.Append("         document_type_id, ");
					query.Append("         feature_permission_id) ");
					query.Append("VALUES ( @user_id, ");
					query.Append("        @unity_id, ");
					query.Append("        @course_id, ");
					query.Append("        @document_type_id, ");
					query.Append("        @feature_permission_id)");

					using (MySqlCommand cmd = new MySqlCommand(query.ToString(), conn)) {
						cmd.Parameters.AddWithValue("@user_id", UserId);
						cmd.Parameters.AddWithValue("@unity_id", UnityId);
						cmd.Parameters.AddWithValue("@course_id", CourseId);
						cmd.Parameters.AddWithValue("@document_type_id", DocumentTypeId);
						cmd.Parameters.AddWithValue("@feature_permission_id", FeaturePermissionId);
						int rows = cmd.ExecuteNonQuery();

						return rows > 0;

					}

				}

			}

			public static List<DocumentTypePermission> GetPermissions(int UserID) {
				List<DocumentTypePermission> documentTypePermissions = new List<DocumentTypePermission>();
				using (MySqlConnection conn = Sql.OpenConnection()) {

					StringBuilder query = new StringBuilder();
					query.Append("SELECT updt.id,");
					query.Append("            updt.user_id, ");
					query.Append("            updt.unity_id, ");
					query.Append("            updt.course_id, ");
					query.Append("            updt.document_type_id, ");
					query.Append("            updt.feature_permission_id, ");
					query.Append("            dt.name_en, ");
					query.Append("            dt.name_ptbr");
					query.Append("       FROM user_permission_document_type updt");
					query.Append(" INNER JOIN document_type dt");
					query.Append("         ON updt.document_type_id = dt.id");
					query.Append("      WHERE updt.user_id = " + UserID);

					using (MySqlCommand cmd = new MySqlCommand(query.ToString(), conn)) {
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							while (reader.Read())
								documentTypePermissions.Add(new DocumentTypePermission {
									ID = reader.GetInt32(0),
									UserID = reader.GetInt32(1),
									UnityID = reader.GetInt32(2),
									CourseID = reader.GetInt32(3),
									DocumentTypeId = reader.GetInt32(4),
									FeaturePermission = (FeaturePermissionResult)reader.GetInt32(5),
									DocumentType = new DocumentType {
										Id = reader.GetInt32(4),
										Name = new Str(reader.GetString(6), reader.GetString(7))
									}
								});

						}
					}
				}
				return documentTypePermissions;
			}


			public override string ToString() {
				return DocumentType.ToString();
			}
		}

		public class PartitionTypePermission {
			public int ID { get; set; }
			public int UserID { get; set; }
			public int UnityID { get; set; }
			public int CourseID { get; set; }
			public int PartitionTypeID { get; set; }
			public FeaturePermissionResult FeaturePermission { get; set; }
			public PartitionType PartitionType { get; set; }

			public bool Delete(int id) {


				using (MySqlConnection conn = Sql.OpenConnection()) {
					StringBuilder query = new StringBuilder();
					query.Append("DELETE FROM user_permission_partition_type ");
					query.Append("WHERE id = @id ");


					using (MySqlCommand cmd = new MySqlCommand(query.ToString(), conn)) {
						cmd.Parameters.AddWithValue("@id", id);
						int rows = cmd.ExecuteNonQuery();
						return rows > 0;

					}

				}

			}


			public bool Add(int UserId, int UnityId, int CourseId, int PartitionTypeId, int FeaturePermissionId) {


				using (MySqlConnection conn = Sql.OpenConnection()) {
					StringBuilder query = new StringBuilder();
					query.Append("INSERT INTO user_permission_partition_type ");
					query.Append("        (user_id, ");
					query.Append("         unity_id, ");
					query.Append("         course_id, ");
					query.Append("         partition_type_id, ");
					query.Append("         feature_permission_id) ");
					query.Append("VALUES ( @user_id, ");
					query.Append("        @unity_id, ");
					query.Append("        @course_id, ");
					query.Append("        @partition_type_id, ");
					query.Append("        @feature_permission_id)");

					using (MySqlCommand cmd = new MySqlCommand(query.ToString(), conn)) {
						cmd.Parameters.AddWithValue("@user_id", UserId);
						cmd.Parameters.AddWithValue("@unity_id", UnityId);
						cmd.Parameters.AddWithValue("@course_id", CourseId);
						cmd.Parameters.AddWithValue("@partition_type_id", PartitionTypeId);
						cmd.Parameters.AddWithValue("@feature_permission_id", FeaturePermissionId);
						int rows = cmd.ExecuteNonQuery();

						return rows > 0;

					}

				}

			}


			public static List<PartitionTypePermission> GetPermissions(int UserID) {
				List<PartitionTypePermission> partitionTypePermissions = new List<PartitionTypePermission>();
				using (MySqlConnection conn = Sql.OpenConnection()) {

					StringBuilder query = new StringBuilder();
					query.Append("SELECT uppt.id,");
					query.Append("            uppt.user_id, ");
					query.Append("            uppt.unity_id, ");
					query.Append("            uppt.course_id, ");
					query.Append("            uppt.partition_type_id, ");
					query.Append("            uppt.feature_permission_id, ");
					query.Append("            pt.name_en, ");
					query.Append("            pt.name_ptbr");
					query.Append("       FROM user_permission_partition_type uppt");
					query.Append(" INNER JOIN partition_type pt");
					query.Append("         ON uppt.partition_type_id = pt.id");
					query.Append("      WHERE uppt.user_id = " + UserID);

					using (MySqlCommand cmd = new MySqlCommand(query.ToString(), conn)) {
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							while (reader.Read())
								partitionTypePermissions.Add(new PartitionTypePermission {
									ID = reader.GetInt32(0),
									UserID = reader.GetInt32(1),
									UnityID = reader.GetInt32(2),
									CourseID = reader.GetInt32(3),
									PartitionTypeID = reader.GetInt32(4),
									FeaturePermission = (FeaturePermissionResult)reader.GetInt32(5),
									PartitionType = new PartitionType {
										Id = reader.GetInt32(4),
										Name = new Str(reader.GetString(6), reader.GetString(7))
									}
								});

						}
					}
				}
				return partitionTypePermissions;
			}

			public override string ToString() {
				return PartitionType.ToString();
			}
		}

		public class CoursePermission {
			public readonly Course Course;
			public readonly PartitionTypePermission[] PartitionTypes;
			public readonly DocumentTypePermission[] DocumentTypes;

			public CoursePermission(Course course, PartitionTypePermission[] partitionTypes, DocumentTypePermission[] documentTypes) {
				Course = course;
				PartitionTypes = partitionTypes;
				DocumentTypes = documentTypes;
			}

			public override string ToString() {
				return Course.ToString();
			}
		}

		public class UnityPermission {
			public readonly Unity Unity;
			public readonly CoursePermission[] Courses;

			public UnityPermission(Unity unity, CoursePermission[] courses) {
				Unity = unity;
				Courses = courses;
			}

			public override string ToString() {
				return Unity.ToString();
			}
		}

		private static readonly Dictionary<int, User> UsersById = new Dictionary<int, User>(16);
		private static readonly CommonReadRareWriteLock Lock = new CommonReadRareWriteLock();

		private const string DefaultPassword = "k2yew1ZGIN3Qe2NHA0KS4lI2+VadNr43PdXfBVstWTEE:q6dfK7fYm5SH/86x/MfkYPaU5K34yBr8UZ52Ga6USVeh";
		private const int ProfileHash = unchecked((int)0x92A43546);
		private const int IdHash = unchecked((int)0xE3340833);

		public int Id;
		public string UserName, FullName;
		public int ProfileId, LanguageId, PictureVersion;
		public bool Active;
		public string ProfileName;
		public long TokenLow, TokenHigh;
		private int[] Features;
		private UnityPermission[] UnityPermissions;

		private string Serialize() {
			byte[] buffer = new byte[24];
			int tmp;
			tmp = (int)TokenLow;
			buffer[0] = (byte)tmp;
			buffer[1] = (byte)(tmp >> 8);
			buffer[2] = (byte)(tmp >> 16);
			buffer[3] = (byte)(tmp >> 24);
			tmp = (int)(TokenLow >> 32);
			buffer[4] = (byte)tmp;
			buffer[5] = (byte)(tmp >> 8);
			buffer[6] = (byte)(tmp >> 16);
			buffer[7] = (byte)(tmp >> 24);
			tmp = (Id ^ IdHash);
			buffer[8] = (byte)tmp;
			buffer[9] = (byte)(tmp >> 8);
			buffer[10] = (byte)(tmp >> 16);
			buffer[11] = (byte)(tmp >> 24);
			tmp = (ProfileId ^ ProfileHash);
			buffer[12] = (byte)tmp;
			buffer[13] = (byte)(tmp >> 8);
			buffer[14] = (byte)(tmp >> 16);
			buffer[15] = (byte)(tmp >> 24);
			tmp = (int)TokenHigh;
			buffer[16] = (byte)tmp;
			buffer[17] = (byte)(tmp >> 8);
			buffer[18] = (byte)(tmp >> 16);
			buffer[19] = (byte)(tmp >> 24);
			tmp = (int)(TokenHigh >> 32);
			buffer[20] = (byte)tmp;
			buffer[21] = (byte)(tmp >> 8);
			buffer[22] = (byte)(tmp >> 16);
			buffer[23] = (byte)(tmp >> 24);
			return Convert.ToBase64String(buffer);
		}

		private static bool Deserialize(string user, out int id, out long tokenLow, out long tokenHigh) {
			byte[] buffer;
			try {
				if (string.IsNullOrWhiteSpace(user)) {
					id = 0;
					tokenLow = 0;
					tokenHigh = 0;
					return false;
				}
				buffer = Convert.FromBase64String(user);
				if (buffer.Length < 24) {
					id = 0;
					tokenLow = 0;
					tokenHigh = 0;
					return false;
				}
			} catch {
				id = 0;
				tokenLow = 0;
				tokenHigh = 0;
				return false;
			}
			id = IdHash ^ BitConverter.ToInt32(buffer, 8); // (buffer[8] | (buffer[9] << 8) | (buffer[10] << 16) | (buffer[11] << 24));
			tokenLow = BitConverter.ToInt64(buffer, 0); // ((long)(buffer[4] | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24)) << 32)
														//| (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
			tokenHigh = BitConverter.ToInt64(buffer, 16); // ((long)(buffer[20] | (buffer[21] << 8) | (buffer[22] << 16) | (buffer[23] << 24)) << 32)
														  //| (uint)(buffer[16] | (buffer[17] << 8) | (buffer[18] << 16) | (buffer[19] << 24));
			return true;
		}

		public static User Login(HttpContext context, string userName, string password) {
			int id, profileId, languageId, pictureVersion;
			bool active;
			string fullName, hash;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, full_name, password, profile_id, language_id, picture_version, active FROM user WHERE user_name = @user_name", conn)) {
					cmd.Parameters.AddWithValue("@user_name", userName);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (!reader.Read())
							return null;
						id = reader.GetInt32(0);
						fullName = reader.GetString(1);
						hash = reader.GetString(2);
						profileId = reader.GetInt32(3);
						languageId = reader.GetInt32(4);
						pictureVersion = reader.GetInt32(5);
						active = reader.GetBoolean(6);
					}
				}
				if (active && PasswordHash.ValidatePassword(password, hash)) {
					//https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx
					byte[] buffer = Guid.NewGuid().ToByteArray();
					long tokenLow = BitConverter.ToInt64(buffer, 0);
					long tokenHigh = BitConverter.ToInt64(buffer, 8);
					using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET token_low = @token_low, token_high = @token_high WHERE id = @id", conn)) {
						cmd.Parameters.AddWithValue("@token_low", tokenLow);
						cmd.Parameters.AddWithValue("@token_high", tokenHigh);
						cmd.Parameters.AddWithValue("@id", id);
						cmd.ExecuteNonQuery();
					}
					User user = new User(id, userName, fullName, profileId, languageId, pictureVersion, active, tokenLow, tokenHigh);
					user.SendToClient(context);
					return user;
				}
			}
			return null;
		}

		public static User GetFromClient(HttpContext context) {
			string cookie = context.Request.Cookies["user"];

			if (string.IsNullOrEmpty(cookie) ||
				!Deserialize(cookie, out int id, out long tokenLow, out long tokenHigh) ||
				id <= 0 ||
				tokenLow == 0 ||
				tokenHigh == 0)
				return null;

			User user;
			try {
				Lock.EnterReadLock();
				UsersById.TryGetValue(id, out user);
			} finally {
				Lock.ExitReadLock();
			}

			if (user == null) {
				using (MySqlConnection conn = Sql.OpenConnection()) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT user_name, full_name, profile_id, language_id, picture_version, token_low, token_high FROM user WHERE id = @id AND active = 1", conn)) {
						cmd.Parameters.AddWithValue("@id", id);
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							if (!reader.Read() ||
								reader.GetInt64(5) != tokenLow ||
								reader.GetInt64(6) != tokenHigh)
								return null;
							user = new User(id, reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), true, tokenLow, tokenHigh);
						}
					}
				}
				try {
					Lock.EnterWriteLock();
					UsersById[id] = user;
				} finally {
					Lock.ExitWriteLock();
				}
			} else if (user.TokenLow != tokenLow || user.TokenHigh != tokenHigh) {
				return null;
			}

			return user;
		}

		private void SendToClient(HttpContext context) {
			context.Response.Cookies.Append("user", Serialize(), new CookieOptions() {
				Expires = DateTime.UtcNow.AddYears(1),
				HttpOnly = false,
				Path = "/",
				Secure = false // Must be set to false if we stop using HTTPS
			});

			try {
				Lock.EnterWriteLock();
				UsersById[Id] = this;
			} finally {
				Lock.ExitWriteLock();
			}
		}

		private void RemoveFromClient(HttpContext context) {
			context.Response.Cookies.Delete("user");

			try {
				Lock.EnterWriteLock();
				UsersById.Remove(Id);
			} finally {
				Lock.ExitWriteLock();
			}
		}

		internal static void PurgeAllCachedUsers() {
			try {
				Lock.EnterWriteLock();
				UsersById.Clear();
			} finally {
				Lock.ExitWriteLock();
			}
		}

		private static void Validate(ref string userName, ref string fullName) {
			if (userName != "admin") {
				if (string.IsNullOrWhiteSpace(userName) || userName.IndexOf('=') >= 0 || userName.IndexOf('@') <= 0)
					throw new ValidationException(Str.InvalidUserName);
				if ((userName = userName.Trim().ToLower()).Length > 64)
					throw new ValidationException(Str.UserNameTooLong);
				if (userName.Length < 10)
					throw new ValidationException(Str.UserNameTooShort);
			}
			if (string.IsNullOrWhiteSpace(fullName) || fullName.IndexOf('=') >= 0)
				throw new ValidationException(Str.InvalidFullName);
			if ((fullName = fullName.Trim().ToUpper()).Length > 64)
				throw new ValidationException(Str.FullNameTooLong);
			if (fullName.Length < 3)
				throw new ValidationException(Str.FullNameTooShort);
		}

		public static User Create(string userName, string fullName, int profileId, int languageId) {
			Validate(ref userName, ref fullName);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				if (profileId != Profile.ADMIN_ID) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT 1 FROM profile WHERE id = @profile_id LIMIT 1", conn)) {
						cmd.Parameters.AddWithValue("@profile_id", profileId);
						object o = cmd.ExecuteScalar();
						if (o == null || o == DBNull.Value)
							throw new ValidationException(Str.ProfileNotFound);
					}
				}
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO user (user_name, full_name, password, profile_id, language_id, picture_version, active, token_low, token_high) VALUES (@user_name, @full_name, @password, @profile_id, @language_id, 0, 1, 0, 0)", conn)) {
					cmd.Parameters.AddWithValue("@user_name", userName);
					cmd.Parameters.AddWithValue("@full_name", fullName);
					cmd.Parameters.AddWithValue("@password", DefaultPassword);
					cmd.Parameters.AddWithValue("@profile_id", profileId);
					cmd.Parameters.AddWithValue("@language_id", languageId);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			return new User(id, userName, fullName, profileId, languageId, 0, true, 0, 0);
		}

		public static List<User> GetAllWithProfileName() {
			List<User> users = new List<User>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand($"SELECT u.id, u.user_name, u.full_name, u.profile_id, u.language_id, p.name{Str._FieldSuffix}, u.active, u.picture_version FROM user u LEFT JOIN profile p ON p.id = u.profile_id ORDER BY p.name{Str._FieldSuffix} ASC, u.user_name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						string noProfile = Str.NO_PROFILE;
						while (reader.Read())
							users.Add(new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(7), reader.GetBoolean(6), 0, 0) {
								ProfileName = (reader.IsDBNull(5) ? noProfile : reader.GetString(5))
							});
					}
				}
			}
			return users;
		}

		public static IActionResult Picture(HttpContext context, int id) {
			string file = Storage.UserProfilePicture(id);
			string mime = "image/jpeg";
			if (!File.Exists(file)) {
				file = Path.Combine(Storage.WWWRoot, "images", "user.png");
				mime = "image/png";
			}

			Microsoft.Net.Http.Headers.EntityTagHeaderValue etag = Storage.GenerateFullETag(file, out DateTime lastModified);

			Microsoft.AspNetCore.Http.Headers.RequestHeaders requestHeaders = context.Request.GetTypedHeaders();
			Microsoft.AspNetCore.Http.Headers.ResponseHeaders responseHeaders = context.Response.GetTypedHeaders();

			if ((requestHeaders.IfNoneMatch != null && requestHeaders.IfNoneMatch.Contains(etag)) ||
				(requestHeaders.IfModifiedSince != null && requestHeaders.IfModifiedSince >= lastModified)) {
				responseHeaders.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue();
				return new StatusCodeResult(StatusCodes.Status304NotModified);
			}

			responseHeaders.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue() {
				Public = true,
				NoCache = false,
				NoStore = false,
				MaxAge = TimeSpan.FromDays(30)
			};
			responseHeaders.ETag = etag;
			responseHeaders.LastModified = lastModified;

			return new FileContentResult(File.ReadAllBytes(file), mime);
		}

		public void Activate(int id) {
			if (Id == id)
				throw new ValidationException(Str.UsersCannotActivateThemselves);
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET active = 1, token_low = 0, token_high = 0 WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					cmd.ExecuteNonQuery();
				}
			}
			try {
				Lock.EnterWriteLock();
				UsersById.Remove(id);
			} finally {
				Lock.ExitWriteLock();
			}
		}

		public void Deactivate(int id) {
			if (Id == id)
				throw new ValidationException(Str.UsersCannotDeactivateThemselves);
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET active = 0, token_low = 0, token_high = 0 WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", id);
					cmd.ExecuteNonQuery();
				}
			}
			try {
				Lock.EnterWriteLock();
				UsersById.Remove(id);
			} finally {
				Lock.ExitWriteLock();
			}
		}

		public void ResetPassword(int id) {
			if (Id == id)
				throw new ValidationException(Str.UsersCannotResetTheirPassword);
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET password = @password, token_low = 0, token_high = 0 WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@password", DefaultPassword);
					cmd.Parameters.AddWithValue("@id", id);
					cmd.ExecuteNonQuery();
				}
			}
			try {
				Lock.EnterWriteLock();
				UsersById.Remove(id);
			} finally {
				Lock.ExitWriteLock();
			}
		}

		public void SetProfile(int id, int profileId) {
			if (Id == id)
				throw new ValidationException(Str.UsersCannotChangeTheirProfile);
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT 1 FROM profile WHERE id = @profile_id LIMIT 1", conn)) {
					cmd.Parameters.AddWithValue("@profile_id", profileId);
					object o = cmd.ExecuteScalar();
					if (o == null || o == DBNull.Value)
						throw new ValidationException(Str.ProfileNotFound);
				}
				using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET profile_id = @profile_id, token_low = 0, token_high = 0 WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@profile_id", profileId);
					cmd.Parameters.AddWithValue("@id", id);
					cmd.ExecuteNonQuery();
				}
			}
			try {
				Lock.EnterWriteLock();
				UsersById.Remove(id);
			} finally {
				Lock.ExitWriteLock();
			}
		}

		public User() {
			//for JSON
		}

		private User(int id, string userName, string fullName, int profileId, int languageId, int pictureVersion, bool active, long tokenLow, long tokenHigh) {
			Id = id;
			UserName = userName;
			FullName = fullName;
			ProfileId = profileId;
			LanguageId = languageId;
			PictureVersion = pictureVersion;
			Active = active;
			TokenLow = tokenLow;
			TokenHigh = tokenHigh;
		}

		public override string ToString() {
			return UserName;
		}

		public UnityPermission[] Permissions {
			get {
				// TODO: Placeholder
				if (UnityPermissions == null) {

					//PartitionTypePermission[] partitionTypePermissionsArray = PartitionTypePermission.GetPermissions(this.Id).ToArray();

					//DocumentTypePermission[] documentTypePermissionsArray = DocumentTypePermission.GetPermissions(this.Id).ToArray();

					PartitionType[] partitionTypes = PartitionType.GetAll();
					List<PartitionTypePermission> partitionTypePermissions = new List<PartitionTypePermission>();
					foreach (PartitionType partitionType in partitionTypes)
						partitionTypePermissions.Add(new PartitionTypePermission() {
							PartitionType = partitionType
						});
					PartitionTypePermission[] partitionTypePermissionsArray = partitionTypePermissions.ToArray();

					DocumentType[] documentTypes = DocumentType.GetAll();
					List<DocumentTypePermission> documentTypePermissions = new List<DocumentTypePermission>();
					foreach (DocumentType documentType in documentTypes)
						documentTypePermissions.Add(new DocumentTypePermission() {
							DocumentType = documentType
						});
					DocumentTypePermission[] documentTypePermissionsArray = documentTypePermissions.ToArray();

					Course[] courses = Course.GetAll();
					List<CoursePermission> coursePermissions = new List<CoursePermission>();
					foreach (Course course in courses)
						coursePermissions.Add(new CoursePermission(course, partitionTypePermissionsArray, documentTypePermissionsArray));
					CoursePermission[] coursePermissionsArray = coursePermissions.ToArray();

					Unity[] units = Unity.GetAll();
					List<UnityPermission> unityPermissions = new List<UnityPermission>();
					foreach (Unity unity in units)
						unityPermissions.Add(new UnityPermission(unity, coursePermissionsArray));

					UnityPermissions = unityPermissions.ToArray();
				}

				return UnityPermissions;
			}
		}

		public bool HasPermission(int unity) {
			// TODO: Placeholder
			return true;
		}

		public bool HasPermission(int unity, int course) {
			// TODO: Placeholder
			return true;
		}

		public bool HasPermission(int unity, int course, int partitionType, int documentType) {
			// TODO: Placeholder
			return true;
		}

		public bool HasFeature(Feature requestedFeature) {
			if (ProfileId == Profile.ADMIN_ID || requestedFeature == Feature.None)
				return true;
			if (Features == null)
				Features = Profile.GetCachedPermissionsByProfileId(ProfileId);
			int[] features = Features;
			if (features == null || features.Length == 0)
				return false;
			int r = (int)requestedFeature, s = 0, e = features.Length - 1, a;
			while (s <= e) {
				a = (s + e) >> 1;
				int f = features[a];
				if (r == f)
					return true;
				if (r < f)
					e = a - 1;
				else
					s = a + 1;
			}
			return false;
		}

		public ValidationResult Validate(Feature requestedFeature) {
			if (ProfileId != Profile.ADMIN_ID &&
				requestedFeature != Feature.None &&
				!HasFeature(requestedFeature))
				return ValidationResult.LoggedInButNoPermission;
			return ValidationResult.OK;
		}

		private void SaveNewPicture(string picture) {
			if (!string.IsNullOrWhiteSpace(picture)) {
				byte[] buffer;

				try {
					if (picture.StartsWith("data:image/jpeg;base64,"))
						picture = picture.Substring(23);
					else if (picture.StartsWith("data:image/png;base64,"))
						picture = picture.Substring(22);
					else
						throw new ValidationException(Str.InvalidImageFormat);

					buffer = Convert.FromBase64String(picture);

					using (MemoryStream stream = new MemoryStream(buffer, false)) {
						using (Bitmap bmp = Image.FromStream(stream) as Bitmap) {
							if (bmp.Width == bmp.Height && bmp.Width <= 150) {
								buffer = ImageManipulation.SaveJpeg(bmp, 90);
							} else {
								using (Bitmap outBmp = new Bitmap(150, 150)) {
									using (Graphics g = Graphics.FromImage(outBmp)) {
										int s = (bmp.Width <= bmp.Height ? bmp.Width : bmp.Height);
										g.DrawImage(bmp, new Rectangle(0, 0, 150, 150), (bmp.Width - s) >> 1, (bmp.Height - s) >> 1, s, s, GraphicsUnit.Pixel);
									}
									buffer = ImageManipulation.SaveJpeg(outBmp, 90);
								}
							}
						}
					}
				} catch {
					throw new ValidationException(Str.InvalidImageFile);
				}

				try {
					File.WriteAllBytes(Storage.UserProfilePicture(Id), buffer);
				} catch {
					throw new ValidationException(Str.ErrorSavingProfileImage);
				}

				Interlocked.Increment(ref PictureVersion);
			}
		}

		public void EditProfile(HttpContext context, string fullName, string picture, int languageId, string password, string newPassword, string newPassword2) {
			Validate(ref UserName, ref fullName);

			if ((password != null && password.Length != 0) || (newPassword != null && newPassword.Length != 0) || (newPassword2 != null && newPassword2.Length != 0)) {
				if (password == null) password = "";
				if (newPassword == null) newPassword = "";
				if (newPassword2 == null) newPassword2 = "";
				if (password.Length == 0 || newPassword.Length == 0 || newPassword2.Length == 0 || newPassword != newPassword2 || newPassword.Length > 20)
					throw new ValidationException(Str.InvalidPassword);
				using (MySqlConnection conn = Sql.OpenConnection()) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT password FROM user WHERE id = @id", conn)) {
						cmd.Parameters.AddWithValue("@id", Id);
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							if (!reader.Read() || !PasswordHash.ValidatePassword(password, reader.GetString(0)))
								throw new ValidationException(Str.CurrentPasswordDoesNotMatch);
						}
					}

					SaveNewPicture(picture);

					using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET full_name = @full_name, password = @password, language_id = @language_id, picture_version = @picture_version, token_low = @token_low, token_high = @token_high WHERE id = @id", conn)) {
						//https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx
						byte[] buffer = Guid.NewGuid().ToByteArray();
						long tokenLow = BitConverter.ToInt64(buffer, 0);
						long tokenHigh = BitConverter.ToInt64(buffer, 8);
						cmd.Parameters.AddWithValue("@full_name", fullName);
						cmd.Parameters.AddWithValue("@password", PasswordHash.CreateHash(newPassword));
						cmd.Parameters.AddWithValue("@language_id", languageId);
						cmd.Parameters.AddWithValue("@picture_version", PictureVersion);
						cmd.Parameters.AddWithValue("@token_low", tokenLow);
						cmd.Parameters.AddWithValue("@token_high", tokenHigh);
						cmd.Parameters.AddWithValue("@id", Id);
						cmd.ExecuteNonQuery();
						FullName = fullName;
						LanguageId = languageId;
						TokenLow = tokenLow;
						TokenHigh = tokenHigh;
					}
				}
			} else {
				using (MySqlConnection conn = Sql.OpenConnection()) {

					SaveNewPicture(picture);

					using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET full_name = @full_name, language_id = @language_id, picture_version = @picture_version WHERE id = @id", conn)) {
						cmd.Parameters.AddWithValue("@full_name", fullName);
						cmd.Parameters.AddWithValue("@language_id", languageId);
						cmd.Parameters.AddWithValue("@picture_version", PictureVersion);
						cmd.Parameters.AddWithValue("@id", Id);
						cmd.ExecuteNonQuery();
						FullName = fullName;
						LanguageId = languageId;
					}
				}
			}
			SendToClient(context);
		}

		public void Logout(HttpContext context) {
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET token_low = 0, token_high = 0 WHERE id = @id", conn)) {
					cmd.Parameters.AddWithValue("@id", Id);
					cmd.ExecuteNonQuery();
				}
			}
			RemoveFromClient(context);
		}
	}
}
