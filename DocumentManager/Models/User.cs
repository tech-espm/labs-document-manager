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

namespace DocumentManager.Models {
	public class User {
		public enum ValidationResult {
			OK = 1,
			NotLoggedIn = 0,
			LoggedInButNoPermission = -1
		}

		private static readonly Dictionary<int, User> UsersById = new Dictionary<int, User>(16);
		private static readonly CommonReadRareWriteLock Lock = new CommonReadRareWriteLock();

		private const string DefaultPassword = "k2yew1ZGIN3Qe2NHA0KS4lI2+VadNr43PdXfBVstWTEE:q6dfK7fYm5SH/86x/MfkYPaU5K34yBr8UZ52Ga6USVeh";
		private const int ProfileHash = unchecked((int)0x92A43546);
		private const int IdHash = unchecked((int)0xE3340833);

		// PASSWORD LENGTH MUST BE <= 20!!
		public int Id;
		public string UserName, FullName;
		public int ProfileId, PictureVersion;
		public bool Active;
		public string ProfileName;
		public long TokenLow, TokenHigh;
		private int[] Features;

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
			int id, profileId, pictureVersion;
			bool active;
			string fullName, hash;
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT id, full_name, password, profile_id, picture_version, active FROM user WHERE user_name = @user_name", conn)) {
					cmd.Parameters.AddWithValue("@user_name", userName);
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						if (!reader.Read())
							return null;
						id = reader.GetInt32(0);
						fullName = reader.GetString(1);
						hash = reader.GetString(2);
						profileId = reader.GetInt32(3);
						pictureVersion = reader.GetInt32(4);
						active = reader.GetBoolean(5);
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
					User user = new User(id, userName, fullName, profileId, pictureVersion, active, tokenLow, tokenHigh);
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
					using (MySqlCommand cmd = new MySqlCommand("SELECT user_name, full_name, profile_id, picture_version, token_low, token_high FROM user WHERE id = @id AND active = 1", conn)) {
						cmd.Parameters.AddWithValue("@id", id);
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							if (!reader.Read() ||
								reader.GetInt64(4) != tokenLow ||
								reader.GetInt64(5) != tokenHigh)
								return null;
							user = new User(id, reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), true, tokenLow, tokenHigh);
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
				Secure = true // Must be set to false if we stop using HTTPS
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

		private static void Validate(string userName, string fullName) {
			if (string.IsNullOrWhiteSpace(userName) || userName.IndexOf('=') >= 0)
				throw new ValidationException("Usuário inválido!");
			if ((userName = userName.Trim().ToLower()).Length > 32)
				throw new ValidationException("Usuário muito longo!");
			if (string.IsNullOrWhiteSpace(fullName) || fullName.IndexOf('=') >= 0)
				throw new ValidationException("Nome completo inválido!");
			if ((fullName = fullName.Trim().ToUpper()).Length > 64)
				throw new ValidationException("Nome completo muito longo!");
		}

		public static User Create(string userName, string fullName, int profileId) {
			Validate(userName, fullName);

			int id;

			using (MySqlConnection conn = Sql.OpenConnection()) {
				if (profileId != Profile.ADMIN_ID) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT 1 FROM profile WHERE id = @profile_id LIMIT 1", conn)) {
						cmd.Parameters.AddWithValue("@profile_id", profileId);
						object o = cmd.ExecuteScalar();
						if (o == null || o == DBNull.Value)
							throw new ValidationException("Perfil não encontrado!");
					}
				}
				using (MySqlCommand cmd = new MySqlCommand("INSERT INTO user (user_name, full_name, password, profile_id, picture_version, active, token_low, token_high) VALUES (@user_name, @full_name, @password, @profile_id, 0, 1, 0, 0)", conn)) {
					cmd.Parameters.AddWithValue("@user_name", userName);
					cmd.Parameters.AddWithValue("@full_name", fullName);
					cmd.Parameters.AddWithValue("@password", DefaultPassword);
					cmd.Parameters.AddWithValue("@profile_id", profileId);
					cmd.ExecuteNonQuery();
				}
				using (MySqlCommand cmd = new MySqlCommand("SELECT last_insert_id()", conn)) {
					id = (int)(ulong)cmd.ExecuteScalar();
				}
			}

			return new User(id, userName, fullName, profileId, 0, true, 0, 0);
		}

		public static IEnumerable<User> GetAllWithProfileName() {
			List<User> users = new List<User>();
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT u.id, u.user_name, u.full_name, u.profile_id, p.name, u.active, u.picture_version FROM user u LEFT JOIN profile ON p.id = u.profile_id ORDER BY p.name ASC, u.user_name ASC", conn)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							users.Add(new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(6), reader.GetBoolean(5), 0, 0) {
								ProfileName = (reader.IsDBNull(4) ? "SEM PERFIL" : reader.GetString(4))
							});
					}
				}
			}
			return users;
		}

		public void Activate(int id) {
			if (Id == id)
				throw new ValidationException("Um usuário não pode ativar a si próprio!");
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
				throw new ValidationException("Um usuário não pode desativar a si próprio!");
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
				throw new ValidationException("Um usuário não pode redefinir sua própria senha!");
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
				throw new ValidationException("Um usuário não pode definir seu próprio perfil!");
			using (MySqlConnection conn = Sql.OpenConnection()) {
				using (MySqlCommand cmd = new MySqlCommand("SELECT 1 FROM profile WHERE id = @profile_id LIMIT 1", conn)) {
					cmd.Parameters.AddWithValue("@profile_id", profileId);
					object o = cmd.ExecuteScalar();
					if (o == null || o == DBNull.Value)
						throw new ValidationException("Perfil não encontrado!");
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

		private User(int id, string userName, string fullName, int profileId, int pictureVersion, bool active, long tokenLow, long tokenHigh) {
			Id = id;
			UserName = userName;
			FullName = fullName;
			ProfileId = profileId;
			PictureVersion = pictureVersion;
			Active = active;
			TokenLow = tokenLow;
			TokenHigh = tokenHigh;
		}

		public override string ToString() {
			return UserName;
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
						throw new ValidationException("Imagem com formato inválido!");

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
					throw new ValidationException("Arquivo de imagem inválido!");
				}

				try {
					File.WriteAllBytes(Storage.UserProfilePicture(Id), buffer);
				} catch {
					throw new ValidationException("Falha na gravação da foto do perfil!");
				}

				Interlocked.Increment(ref PictureVersion);
			}
		}

		public void EditProfile(HttpContext context, string fullName, string picture, string pass, string newPass1, string newPass2) {
			Validate(UserName, fullName);

			if ((pass != null && pass.Length != 0) || (newPass1 != null && newPass1.Length != 0) || (newPass2 != null && newPass2.Length != 0)) {
				if (pass == null) pass = "";
				if (newPass1 == null) newPass1 = "";
				if (newPass2 == null) newPass2 = "";
				if (pass.Length == 0 || newPass1.Length == 0 || newPass2.Length == 0 || newPass1 != newPass2)
					throw new ValidationException("Senha inválida!");
				using (MySqlConnection conn = Sql.OpenConnection()) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT password FROM user WHERE id = @id", conn)) {
						cmd.Parameters.AddWithValue("@id", Id);
						using (MySqlDataReader reader = cmd.ExecuteReader()) {
							if (!reader.Read() || !PasswordHash.ValidatePassword(pass, reader.GetString(0)))
								throw new ValidationException("Senha atual não confere!");
						}
					}

					SaveNewPicture(picture);

					using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET full_name = @full_name, password = @password, picture_version = @picture_version, token_low = @token_low, token_high = @token_high WHERE id = @id", conn)) {
						//https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx
						byte[] buffer = Guid.NewGuid().ToByteArray();
						long tokenLow = BitConverter.ToInt64(buffer, 0);
						long tokenHigh = BitConverter.ToInt64(buffer, 8);
						cmd.Parameters.AddWithValue("@full_name", fullName);
						cmd.Parameters.AddWithValue("@password", PasswordHash.CreateHash(newPass1));
						cmd.Parameters.AddWithValue("@picture_version", PictureVersion);
						cmd.Parameters.AddWithValue("@token_low", tokenLow);
						cmd.Parameters.AddWithValue("@token_high", tokenLow);
						cmd.Parameters.AddWithValue("@id", Id);
						cmd.ExecuteNonQuery();
						FullName = fullName;
						TokenLow = tokenLow;
						TokenHigh = tokenHigh;
					}
				}
			} else {
				using (MySqlConnection conn = Sql.OpenConnection()) {

					SaveNewPicture(picture);

					using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET full_name = @full_name, picture_version = @picture_version WHERE id = @id", conn)) {
						cmd.Parameters.AddWithValue("@full_name", fullName);
						cmd.Parameters.AddWithValue("@picture_version", PictureVersion);
						cmd.Parameters.AddWithValue("@id", Id);
						cmd.ExecuteNonQuery();
						FullName = fullName;
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
