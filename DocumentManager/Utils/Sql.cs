using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace DocumentManager.Utils
{
    public static class Sql
    {
      

        public static MySqlConnection OpenConnection()
        {

            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.Production.json", optional: true)
              .AddJsonFile($"appsettings.Development.json", optional: true)
              .AddEnvironmentVariables();
 
            var Configuration = builder.Build();

            var appSetting = Configuration.Get<AppSetting>();

            // Add the correct connection string
            MySqlConnection conn = new MySqlConnection(appSetting.MySQL);
            conn.Open();
            return conn;
        }
    }
}
