using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSRS_Test
{
    public partial class LoginPage : Form
    {
        public static string connectionString = "Data Source=DESKTOP-3HOFD4J\\SQLEXPRESS;Initial Catalog=SSRS;Integrated Security=True";
        public static SqlConnection connection = new SqlConnection(connectionString);
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand getUserData = new SqlCommand("select * from Person where personEmail=@username and personPassword=@password", connection);
            getUserData.Parameters.AddWithValue("@username", UsernameTextBox.Text);
            getUserData.Parameters.AddWithValue("@password", PasswordTextBox.Text);
            SqlDataReader userDataReader = getUserData.ExecuteReader();
            if (userDataReader.Read())
            {
                int AuthID = Convert.ToInt32(userDataReader["personAuthID"]);
                if (AuthID == 1)
                {
                    UserPage userPage = new UserPage();
                    userPage.Show();
                }
                else if (AuthID == 2)
                {
                    AdminPage adminPage = new AdminPage();
                    adminPage.Show();
                }
            }
            else
            {
                MessageBox.Show("Login failed. Try Again.");
            }
            connection.Close();
        }

        private void RegisterLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            RegisterPage registerPage = new RegisterPage();
            registerPage.ShowDialog();
            this.Show();
        }
    }
}
