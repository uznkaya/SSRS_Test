using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRS_Test
{
    internal class Database
    {
        private static string connectionString = "Data Source=DESKTOP-3HOFD4J\\SQLEXPRESS;Initial Catalog=SSRS;Integrated Security=True";
        private static SqlConnection sqlConnection = new SqlConnection(connectionString);
        public static void OpenConnection()
        {
            sqlConnection.Open();
        }
        public static void CloseConnection()
        {
            sqlConnection.Close();
        }
        public static SqlConnection GetConnection()
        {
            return sqlConnection;
        }
    }
}
