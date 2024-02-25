using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSRS_Test
{
    internal class Person
    {
        private static int personID, personAuthID;
        private static string personName, personSurname, personEmail, personPassword;
        public static bool SetPersonData(string email, string password)
        {
            Database.OpenConnection();
            SqlCommand getUserData = new SqlCommand("select * from Person where personEmail=@email and personPassword=@password",Database.GetConnection());
            getUserData.Parameters.AddWithValue("@email", email);
            getUserData.Parameters.AddWithValue("@password", password);
            SqlDataReader dataReader = getUserData.ExecuteReader();
            if (dataReader.Read())
            {
                personID = Convert.ToInt32(dataReader["personID"]);
                personName = dataReader["personName"].ToString();
                personSurname = dataReader["personSurname"].ToString();
                personEmail = dataReader["personEmail"].ToString();
                personPassword = dataReader["personPassword"].ToString();
                personAuthID = Convert.ToInt32(dataReader["personAuthID"]);
                Database.CloseConnection();
                return true;
            }
            else
            {
                MessageBox.Show("Failed. Try Again");
                Database.CloseConnection();
                return false;
            }
        }
        public static int GetPersonID()
        {
            return personID;
        }
        public static int GetPersonAuthID()
        {
            return personAuthID;
        }
        public static string GetPersonName()
        {
            return personName;
        }
        public static string GetPersonSurname()
        {
            return personSurname;
        }
        public static string GetPersonEmail()
        {
            return personEmail;
        }
        public static string GetPersonPassword()
        {
            return personPassword;
        }
    }
}
