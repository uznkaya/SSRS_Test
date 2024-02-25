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
        public LoginPage()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, EventArgs e)
        {
            string personEmail = UsernameTextBox.Text;
            string personPassword = PasswordTextBox.Text;
            if(Person.SetPersonData(personEmail, personPassword))
            {
                int personAuthID = Person.GetPersonAuthID();
                if(personAuthID == 1)
                {
                    UserPage userPage = new UserPage();
                    userPage.Show();
                }
                else if (personAuthID == 2)
                {
                    AdminPage adminPage = new AdminPage();
                    adminPage.Show();
                }
                this.Hide();
            }
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
