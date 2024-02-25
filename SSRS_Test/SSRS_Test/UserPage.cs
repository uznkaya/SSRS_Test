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
    public partial class UserPage : Form
    {
        public UserPage()
        {
            InitializeComponent();
        }
        private void UserPage_Load(object sender, EventArgs e)
        {
            AnaSayfaGroupBox.Show();
            RandevuOlusturGroupBox.Hide();
            RandevularimGroupBox.Hide();
            AyarlarGroupBox.Hide();
        }

        private void AnaSayfaButton_Click(object sender, EventArgs e)
        {
            AnaSayfaGroupBox.Show();
            RandevuOlusturGroupBox.Hide();
            RandevularimGroupBox.Hide();
            AyarlarGroupBox.Hide();
        }

        private void RandevuOluşturButton_Click(object sender, EventArgs e)
        {
            RandevuOlusturGroupBox.Show();
            AnaSayfaGroupBox.Hide();
            RandevularimGroupBox.Hide();
            AyarlarGroupBox.Hide();
        }

        private void RandevularımButton_Click(object sender, EventArgs e)
        {
            RandevularimGroupBox.Show();
            RandevuOlusturGroupBox.Hide();
            AnaSayfaGroupBox.Hide();
            AyarlarGroupBox.Hide();
        }

        private void AyarlarButton_Click(object sender, EventArgs e)
        {
            AyarlarGroupBox.Show();
            RandevularimGroupBox.Hide();
            RandevuOlusturGroupBox.Hide();
            AnaSayfaGroupBox.Hide();
        }
        private void CikisYapButton_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
        }
    }
}
