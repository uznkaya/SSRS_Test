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
    public partial class RegisterPage : Form
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void BackLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            CreatePerson();
        }
        private void CreatePerson()
        {
            Database.OpenConnection();
            SqlCommand insertCommand = new SqlCommand("insert into Person values(@personName,@personSurname,@personEmail,@personPassword,1)", Database.GetConnection());
            insertCommand.Parameters.AddWithValue("@personName", NameTextBox.Text);
            insertCommand.Parameters.AddWithValue("@personSurname", SurnameTextBox.Text);
            insertCommand.Parameters.AddWithValue("@personEmail", EmailTextBox.Text);
            insertCommand.Parameters.AddWithValue("@personPassword", PasswordTextBox.Text);
            insertCommand.ExecuteNonQuery();
            Database.CloseConnection();
            this.Close();
        }
    }
}
