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
        private string AppointmentData { get; set; }
        private string SportData { get; set; }
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
            SportPanel.Hide();
            NameSurnameLabel.Text = "Hoşgeldin " + Person.GetPersonName() + " " + Person.GetPersonSurname();
            SetAppointmentHoursToDateComboBox();
            SetSportsToComboBox();
        }
        private void SetAppointmentHoursToDateComboBox()
        {
            DateComboBox.Items.Clear();
            Database.OpenConnection();
            SqlCommand getDatesCommand = new SqlCommand("select distinct appointmentDate from appointments", Database.GetConnection());
            SqlDataReader dataReader = getDatesCommand.ExecuteReader();
            while (dataReader.Read())
            {
                DateComboBox.Items.Add(dataReader.GetDateTime(0).ToString("dd-MM-yyyy"));
            }
            Database.CloseConnection();
        }
        private void SetSportsToComboBox()
        {
            SportsComboBox.Items.Clear();
            Database.OpenConnection();
            SqlCommand getSportsCommand = new SqlCommand("select distinct sportName from Sports", Database.GetConnection());
            SqlDataReader dataReader = getSportsCommand.ExecuteReader();
            while (dataReader.Read())
            {
                SportsComboBox.Items.Add(dataReader["sportName"].ToString());
            }
            Database.CloseConnection();
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
        private void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            Database.OpenConnection();
            SqlCommand changePasswordCommand = new SqlCommand("update Person set personPassword = @newpassword where personID = @personID", Database.GetConnection());
            changePasswordCommand.Parameters.AddWithValue("@personID", Person.GetPersonID());
            changePasswordCommand.Parameters.AddWithValue("@newpassword", NewPasswordTextBox.Text);
            if (OldPasswordTextBox.Text == Person.GetPersonPassword())
            {
                changePasswordCommand.ExecuteNonQuery();
                MessageBox.Show("Successfully changed password");
            }
            else
            {
                MessageBox.Show("Check your passwords");
            }
            Database.CloseConnection();
        }
        private void DateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAppointmentsHoursToDateComboBox();
        }
        private void SetAppointmentsHoursToDateComboBox()
        {
            AppointmentPanel.Controls.Clear();
            int startX = 10, startY = 10;
            int buttonWidth = 100, buttonHeight = 50;
            int buttonSpacing = 10;

            Database.OpenConnection();
            //SqlCommand getAppointments = new SqlCommand("SELECT appointmentHour, appointmentSportID, COALESCE(Sports.sportName,null) as SportName from Appointments LEFT JOIN Sports ON Appointments.appointmentSportID = Sports.sportID WHERE appointmentDate = @date", Database.GetConnection());
            SqlCommand getAppointments = new SqlCommand("SELECT appointmentHour, appointmentSportID, COALESCE(Sports.sportName,null) as SportName, COUNT(CreatedAppointments.createdappointmentAppID) as AppointmentCount, Sports.sportCapacity as SportCapacity FROM Appointments LEFT JOIN Sports ON Appointments.appointmentSportID = Sports.sportID LEFT JOIN CreatedAppointments ON Appointments.appointmentID = CreatedAppointments.createdappointmentAppID WHERE appointmentDate = @date GROUP BY appointmentHour, appointmentSportID, COALESCE(Sports.sportName, null), Sports.sportCapacity", Database.GetConnection());

            string inputDate = DateComboBox.Text;
            DateTime convertedDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", null);
            string outputConvertedDate = convertedDate.ToString("yyyy-MM-dd");

            getAppointments.Parameters.AddWithValue("@date", outputConvertedDate);
            SqlDataReader dataReader = getAppointments.ExecuteReader();

            int i = 0;
            while (dataReader.Read())
            {
                Button newButton = new Button();

                if (!dataReader.IsDBNull(1))
                {
                    newButton.Text = dataReader.GetTimeSpan(0).ToString() + "\n" + dataReader["SportName"].ToString() + "\n" + dataReader["AppointmentCount"].ToString() + "/" + dataReader["SportCapacity"];
                }
                else
                {
                    newButton.Text = dataReader.GetTimeSpan(0).ToString();
                }
                newButton.Tag = dataReader.GetTimeSpan(0).ToString();
                newButton.Width = buttonWidth;
                newButton.Height = buttonHeight;
                AppointmentPanel.Controls.Add(newButton);
                newButton.Location = new System.Drawing.Point(startX + i * (buttonWidth + buttonSpacing), startY);
                newButton.Click += new EventHandler(Button_Click);
                i++;
            }
            Database.CloseConnection();
        }
        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            Database.OpenConnection();
            SqlCommand checkAppointment = new SqlCommand("select * from Appointments where appointmentIsAvailable = 1 and appointmentSportID is null and appointmentDate = @date and appointmentHour = @hour", Database.GetConnection());

            string inputDate = DateComboBox.Text;
            DateTime convertedDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", null);
            string outputConvertedDate = convertedDate.ToString("yyyy-MM-dd");

            checkAppointment.Parameters.AddWithValue("@date", outputConvertedDate);
            checkAppointment.Parameters.AddWithValue("@hour", clickedButton.Tag.ToString());
            SqlDataReader dataReader = checkAppointment.ExecuteReader();
            if (dataReader.Read())
            {
                AppointmentData = dataReader["appointmentID"].ToString();
                SportPanel.Show();
            }
            else
            {
                SportPanel.Hide();
            }
            dataReader.Close();

            //Randevu alınmış yere kapasite dolmadı ise tekrar al olayı gelicek
            Database.CloseConnection();
        }
        private void CreateNewAppointment(string appointmetID, string sportID)
        {
            Database.OpenConnection();

            SqlCommand createAppointment = new SqlCommand("insert into CreatedAppointments values(@appPersonID, @appAppID, @appSportID)", Database.GetConnection());
            createAppointment.Parameters.AddWithValue("@appPersonID", Person.GetPersonID());
            createAppointment.Parameters.AddWithValue("@appAppID", appointmetID);

            if (SportData != null)
            {
                createAppointment.Parameters.AddWithValue("@appSportID", sportID);
                createAppointment.ExecuteNonQuery();

                SqlCommand updateAppointmentsCommand = new SqlCommand("update Appointments set appointmentSportID = @sportID where appointmentID = @appID", Database.GetConnection());
                updateAppointmentsCommand.Parameters.AddWithValue("@sportID", sportID);
                updateAppointmentsCommand.Parameters.AddWithValue("@appID", appointmetID);
                updateAppointmentsCommand.ExecuteNonQuery();
                MessageBox.Show("Successfully created appointment.");
            }
            else
            {
                MessageBox.Show("Not found in the sports database.");
            }
            Database.CloseConnection();
        }
        private void SportBackButton_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SportPanel.Hide();
        }
        private void CreateNewAppointmentButton_Click(object sender, EventArgs e)
        {
            CreateNewAppointment(AppointmentData, SportData);
            SetAppointmentsHoursToDateComboBox();
        }
        private void SportsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Database.OpenConnection();
            SqlCommand getSportID = new SqlCommand("select sportID from Sports where sportName = @spor", Database.GetConnection());
            getSportID.Parameters.AddWithValue("@spor", SportsComboBox.Text);
            SqlDataReader dataReader = getSportID.ExecuteReader();
            if (dataReader.Read())
            {
                SportData = dataReader["SportID"].ToString();
            }
            Database.CloseConnection();
        }
    }
}
