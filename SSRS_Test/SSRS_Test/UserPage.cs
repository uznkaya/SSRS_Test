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
            MyAppointmentsPanel.Hide();
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
            MyAppointmentsPanel.Hide();
            SportPanel.Hide();
        }
        private void RandevuOluşturButton_Click(object sender, EventArgs e)
        {
            RandevuOlusturGroupBox.Show();
            AnaSayfaGroupBox.Hide();
            RandevularimGroupBox.Hide();
            AyarlarGroupBox.Hide();
            MyAppointmentsPanel.Hide();
            SportPanel.Hide();
        }
        private void RandevularımButton_Click(object sender, EventArgs e)
        {
            RandevularimGroupBox.Show();
            RandevuOlusturGroupBox.Hide();
            AnaSayfaGroupBox.Hide();
            AyarlarGroupBox.Hide();
            SportPanel.Hide();
            MyAppointmentsPanel.Hide();
            GetMyAppointments();
        }
        private void AyarlarButton_Click(object sender, EventArgs e)
        {
            AyarlarGroupBox.Show();
            RandevularimGroupBox.Hide();
            RandevuOlusturGroupBox.Hide();
            AnaSayfaGroupBox.Hide();
            MyAppointmentsPanel.Hide();
            SportPanel.Hide();
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
            SqlCommand getAppointments = new SqlCommand("SELECT appointmentHour, appointmentSportID, COALESCE(Sports.sportName,null) as SportName, COUNT(CreatedAppointments.createdappointmentAppID) as AppointmentCount, Sports.sportCapacity as SportCapacity, appointmentIsAvailable FROM Appointments LEFT JOIN Sports ON Appointments.appointmentSportID = Sports.sportID LEFT JOIN CreatedAppointments ON Appointments.appointmentID = CreatedAppointments.createdappointmentAppID WHERE appointmentDate = @date GROUP BY appointmentHour, appointmentSportID, COALESCE(Sports.sportName, null), Sports.sportCapacity, appointmentIsAvailable", Database.GetConnection());

            string inputDate = DateComboBox.Text;
            DateTime convertedDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", null);
            string outputConvertedDate = convertedDate.ToString("yyyy-MM-dd");

            getAppointments.Parameters.AddWithValue("@date", outputConvertedDate);
            SqlDataReader dataReader = getAppointments.ExecuteReader();

            int i = 0;
            while (dataReader.Read())
            {
                Button newButton = new Button();

                if (!dataReader.IsDBNull(1) && dataReader["appointmentIsAvailable"].ToString() == "True")
                {
                    newButton.Text = dataReader.GetTimeSpan(0).ToString() + "\n" + dataReader["SportName"].ToString() + "\n" + dataReader["AppointmentCount"].ToString() + "/" + dataReader["SportCapacity"];
                }
                else if (!dataReader.IsDBNull(1) && dataReader["appointmentIsAvailable"].ToString() == "False")
                {
                    newButton.Text = dataReader.GetTimeSpan(0).ToString() + "\n" + dataReader["SportName"].ToString() + "\n" + "FULL";
                    newButton.Enabled = false;
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

            SqlCommand checkAppointmentHaveCapacity = new SqlCommand("SELECT Appointments.appointmentID, Appointments.appointmentDate, Appointments.appointmentHour, Sports.sportID, Sports.sportName, Sports.SportCapacity, COUNT(CreatedAppointments.createdappointmentAppID) AS AppointmentCount FROM Appointments INNER JOIN Sports ON Appointments.appointmentSportID = Sports.sportID LEFT JOIN CreatedAppointments ON Appointments.appointmentID = CreatedAppointments.createdappointmentAppID WHERE Appointments.appointmentDate = @date AND Appointments.appointmentHour = @hour GROUP BY Appointments.appointmentID, Appointments.appointmentDate, Appointments.appointmentHour, Sports.sportID, Sports.sportName, Sports.SportCapacity HAVING COUNT(CreatedAppointments.createdappointmentAppID) < Sports.SportCapacity OR COUNT(CreatedAppointments.createdappointmentAppID) IS NULL", Database.GetConnection());
            checkAppointmentHaveCapacity.Parameters.AddWithValue("@hour", clickedButton.Tag.ToString());
            checkAppointmentHaveCapacity.Parameters.AddWithValue("@date", outputConvertedDate);
            SqlDataReader dataReader1 = checkAppointmentHaveCapacity.ExecuteReader();
            if (dataReader1.Read())
            {
                AppointmentData = dataReader1["appointmentID"].ToString();
                SportData = dataReader1["sportID"].ToString();
                Database.CloseConnection();
                CreateAppointment(AppointmentData, SportData);
                CheckCapacity(AppointmentData);
                SetAppointmentsHoursToDateComboBox();
            }
            dataReader1.Close();

            //Randevu alınmış yere kapasite dolmadı ise tekrar al olayı gelicek

            Database.CloseConnection();
        }

        private void CheckCapacity(string appointmentID)
        {

            Database.OpenConnection();
            SqlCommand checkCapacity = new SqlCommand("SELECT Appointments.appointmentID, Appointments.appointmentDate, Appointments.appointmentHour, Sports.sportID, Sports.sportName, Sports.SportCapacity, COUNT(CreatedAppointments.createdappointmentAppID) AS AppointmentCount FROM Appointments INNER JOIN Sports ON Appointments.appointmentSportID = Sports.sportID LEFT JOIN CreatedAppointments ON Appointments.appointmentID = CreatedAppointments.createdappointmentAppID WHERE Appointments.appointmentID = @id GROUP BY Appointments.appointmentID, Appointments.appointmentDate, Appointments.appointmentHour, Sports.sportID, Sports.sportName, Sports.SportCapacity HAVING COUNT(CreatedAppointments.createdappointmentAppID) = Sports.SportCapacity", Database.GetConnection());
            checkCapacity.Parameters.AddWithValue("@id", appointmentID);
            SqlDataReader dataReader = checkCapacity.ExecuteReader();
            if (dataReader.Read())
            {
                dataReader.Close();
                SqlCommand updateAppointments = new SqlCommand("update Appointments set appointmentIsAvailable = 0 where appointmentID = @id", Database.GetConnection());
                updateAppointments.Parameters.AddWithValue("@id", appointmentID);
                updateAppointments.ExecuteNonQuery();
            }
            Database.CloseConnection();

        }
        private void CreateAppointment(string appointmentID, string sportID)
        {
            Database.OpenConnection();
            SqlCommand createAppointment = new SqlCommand("insert into CreatedAppointments values(@appPersonID, @appAppID, @appSportID)", Database.GetConnection());
            createAppointment.Parameters.AddWithValue("@appPersonID", Person.GetPersonID());
            createAppointment.Parameters.AddWithValue("@appAppID", appointmentID);
            createAppointment.Parameters.AddWithValue("@appSportID", sportID); createAppointment.ExecuteNonQuery();
            MessageBox.Show("Successfully created appointment.");
            Database.CloseConnection();
        }
        private void CreateNewAppointment(string appointmentID, string sportID)
        {
            Database.OpenConnection();

            SqlCommand createAppointment = new SqlCommand("insert into CreatedAppointments values(@appPersonID, @appAppID, @appSportID)", Database.GetConnection());
            createAppointment.Parameters.AddWithValue("@appPersonID", Person.GetPersonID());
            createAppointment.Parameters.AddWithValue("@appAppID", appointmentID);

            if (SportData != null)
            {
                createAppointment.Parameters.AddWithValue("@appSportID", sportID);
                createAppointment.ExecuteNonQuery();

                SqlCommand updateAppointmentsCommand = new SqlCommand("update Appointments set appointmentSportID = @sportID where appointmentID = @appID", Database.GetConnection());
                updateAppointmentsCommand.Parameters.AddWithValue("@sportID", sportID);
                updateAppointmentsCommand.Parameters.AddWithValue("@appID", appointmentID);
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
            CheckCapacity(AppointmentData);
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

        private void GetMyAppointments()
        {
            Database.OpenConnection();
            SqlDataAdapter getMyAppointments = new SqlDataAdapter("SELECT CreatedAppointments.createdappointmentPersonID, Appointments.appointmentID, Appointments.appointmentDate, Appointments.appointmentHour, Sports.sportName, CONCAT((SELECT COUNT(CreatedAppointments.createdappointmentAppID) FROM CreatedAppointments WHERE CreatedAppointments.createdappointmentAppID = Appointments.appointmentID), '/', Sports.sportCapacity) AS AppointmentCapacity FROM Appointments INNER JOIN Sports ON Appointments.appointmentSportID = Sports.sportID LEFT JOIN CreatedAppointments ON Appointments.appointmentID = CreatedAppointments.createdappointmentAppID Where CreatedAppointments.createdappointmentPersonID = "+Convert.ToString(Person.GetPersonID())+" GROUP BY CreatedAppointments.createdappointmentPersonID, Appointments.appointmentID, Appointments.appointmentDate, Appointments.appointmentHour, Sports.sportID, Sports.sportName, Sports.sportCapacity; ", Database.GetConnection());
            DataTable table = new DataTable();
            getMyAppointments.Fill(table);
            AppointmentDataGridView.DataSource = table;
            Database.CloseConnection();
        }

        private void AppointmentDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            MyAppointmentsPanel.Show();
        }

        private void RemoveAppointmentButton_Click(object sender, EventArgs e)
        {
            Database.OpenConnection();
            SqlCommand removeAppointment = new SqlCommand("DELETE FROM CreatedAppointments Where CreatedAppointments.createdappointmentPersonID = @personid and CreatedAppointments.createdappointmentAppID = @appointmentid", Database.GetConnection());
            removeAppointment.Parameters.AddWithValue("@personid", Person.GetPersonID());
            removeAppointment.Parameters.AddWithValue("@appointmentid", AppointmentDataGridView.SelectedRows[0].Cells[1].Value);
            removeAppointment.ExecuteNonQuery();
            MessageBox.Show("Appointment successfully removed");
            Database.CloseConnection();
            UpdateAppointmentsStatus();
            SetAppointmentsHoursToDateComboBox();
            GetMyAppointments();

        }

        private void UpdateAppointmentsStatus()
        {
            Database.OpenConnection();
            SqlCommand updateAppointments = new SqlCommand("Update Appointments Set appointmentIsAvailable = 1, appointmentSportID = null WHERE appointmentID IN (SELECT Appointments.appointmentID FROM Appointments INNER JOIN Sports ON Appointments.appointmentSportID = Sports.sportID LEFT JOIN CreatedAppointments ON Appointments.appointmentID = CreatedAppointments.createdappointmentAppID GROUP BY Appointments.appointmentID HAVING COUNT(CreatedAppointments.createdappointmentAppID) = 0)",Database.GetConnection());
            updateAppointments.ExecuteNonQuery();
            Database.CloseConnection();
        }
    }
}
