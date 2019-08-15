using System;
using MySql.Data.MySqlClient;

namespace DocumentManager.Utils {
	public static class Sql {
		private static readonly string ConnStr = AppSetting.GetAppSetting().MySQL;

		public static MySqlConnection OpenConnection() {
			MySqlConnection conn = new MySqlConnection(ConnStr);
			conn.Open();
			return conn;
		}
	}
}
