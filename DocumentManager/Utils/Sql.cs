using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DocumentManager.Utils {
	public class Sql {
		public static SqlConnection OpenConnection() {
			// Add the correct connection string
			SqlConnection conn = new SqlConnection("fake connection string");
			conn.Open();
			return conn;
		}
	}
}
