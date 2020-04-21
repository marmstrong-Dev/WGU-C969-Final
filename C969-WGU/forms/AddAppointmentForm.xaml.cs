using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    /// <summary>
    /// Interaction logic for AddAppointmentForm.xaml
    /// </summary>
    public partial class AddAppointmentForm : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        Appointment addedAppointment = new Appointment();
        Consultant loggedConsultant_AA = new Consultant();

        public AddAppointmentForm(Consultant loggedUser)
        {
            InitializeComponent();
            loggedConsultant_AA = loggedUser;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimePickerInit();
            GenerateAppointmentID();
        }

        // Generate Customer ID
        private void GenerateAppointmentID()
        {
            string getNextIDQuery = $"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'U07042' AND TABLE_NAME = 'appointment';";
            int nextID = 0;

            dbCon.Open();

            MySqlCommand getNextIDCommand = new MySqlCommand(getNextIDQuery, dbCon);
            MySqlDataReader getNextIDReader = getNextIDCommand.ExecuteReader();

            if (getNextIDReader.Read())
            { nextID = getNextIDReader.GetInt32(0); }

            dbCon.Close();

            AppointmentIDLabel.Content = $"Appointment ID: { nextID }";
        }

        /*
        Date / Time Selection Methods 
        */

        // Initialize Combo Boxes
        private void TimePickerInit()
        {
            HrSelection_start.Items.Add("Hours");
            HrSelection_end.Items.Add("Hours");
            HrSelection_start.SelectedIndex = 0;
            HrSelection_end.SelectedIndex = 0;
            HourGenerator(HrSelection_start);
            HourGenerator(HrSelection_end);

            MinSelection_start.Items.Add("Minutes");
            MinSelection_end.Items.Add("Minutes");
            MinSelection_start.SelectedIndex = 0;
            MinSelection_end.SelectedIndex = 0;
            MinuteGenerator(MinSelection_start);
            MinuteGenerator(MinSelection_end);

            AM_PM_Selection_start.Items.Add("AM");
            AM_PM_Selection_start.Items.Add("PM");
            AM_PM_Selection_start.SelectedIndex = 0;

            AM_PM_Selection_end.Items.Add("AM");
            AM_PM_Selection_end.Items.Add("PM");
            AM_PM_Selection_end.SelectedIndex = 0;
        }

        // Populate Hours Selections
        private void HourGenerator(ComboBox genHours)
        {
            for (int i = 1; i <= 12; i++)
            { genHours.Items.Add(i); }
        }

        // Populate Minutes Selections
        private void MinuteGenerator(ComboBox genMinutes)
        {
            for (int i = 0; i < 60; i++)
            { genMinutes.Items.Add(i); }
        }

        // Lookup User ID and Customer ID
        private void CustomerIDLookup()
        {
            Validator idValidator = new Validator();

            if (idValidator.CheckIfExists(CustomerIDInput.Text, "customerId", "customer"))
            { 
                int custID = Int32.Parse(CustomerIDInput.Text);
                addedAppointment.AddAppointment(custID, loggedConsultant_AA.consultantID, loggedConsultant_AA.consultantName);

                Log addedAppointmentLog = new Log(loggedConsultant_AA.consultantName);
                addedAppointmentLog.AddRecord("Appointment", AppointmentNameInput.Text);

                Dashboard savedAddAppointment = new Dashboard(loggedConsultant_AA);
                savedAddAppointment.Show();

                this.Close();
            }
            else 
            { MessageBox.Show("Customer Does Not Exist"); }
        }

        // Build New Appointment
        private void BuildAppointment()
        {
            addedAppointment.appointmentName = AppointmentNameInput.Text;
            addedAppointment.appointmentDescription = AppointmentDescriptionInput.Text;
            addedAppointment.appointmentLocation = AppointmentLocationInput.Text;
            addedAppointment.appointmentContact = AppointmentContactInput.Text;
            addedAppointment.appointmentURL = AppointmentURLInput.Text;

            addedAppointment.startTime = BuildTimeStamp(AM_PM_Selection_start.Text, HrSelection_start.Text, MinSelection_start.Text);
            addedAppointment.endTime = BuildTimeStamp(AM_PM_Selection_end.Text, HrSelection_end.Text, MinSelection_end.Text);

            if (IOSelected.IsChecked == true)
            { addedAppointment.appointmentType = "In Office"; }
            else if (OSSelected.IsChecked == true)
            { addedAppointment.appointmentType = "Offsite"; }
            else if (TCSelected.IsChecked == true)
            { addedAppointment.appointmentType = "Teleconference"; }

            Validator timeValidator = new Validator();

            if (timeValidator.CheckForAppointmentConflicts(addedAppointment.startTime.ToUniversalTime(), addedAppointment.endTime.ToUniversalTime(), loggedConsultant_AA.consultantID) == true)
            { CustomerIDLookup(); }
            else
            { MessageBox.Show(timeValidator.formError); }
        }

        // Generate TimeStamp
        private DateTime BuildTimeStamp(string dayHalf, string hrSelection, string minSelection)
        {
            int hoursAdd;
            if (dayHalf == "PM")
            { hoursAdd = Int32.Parse(hrSelection) + 12; }
            else
            { hoursAdd = Int32.Parse(hrSelection); }

            DateTime addedTime = new DateTime
            (
                AppointmentDateInput.SelectedDate.Value.Year,
                AppointmentDateInput.SelectedDate.Value.Month,
                AppointmentDateInput.SelectedDate.Value.Day,
                hoursAdd,
                Int32.Parse(minSelection), 0
            );

            addedTime.ToUniversalTime();

            return addedTime;
        }
        
        /*
        Button Methods 
        */

        // Save New Appointment
        private void CustomerSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (HrSelection_start.Text == "Hours" || MinSelection_start.Text == "Minutes")
            {
                HrSelection_start.Text = "";
                MinSelection_start.Text = "";
            }

            if (HrSelection_end.Text == "Hours" || MinSelection_end.Text == "Minutes")
            {
                HrSelection_end.Text = "";
                MinSelection_end.Text = "";
            }
            
            string[] appointmentInput =
            {
                AppointmentNameInput.Text,
                AppointmentContactInput.Text,
                AppointmentLocationInput.Text,
                AppointmentDescriptionInput.Text,
                AppointmentURLInput.Text,
                AM_PM_Selection_start.Text, HrSelection_start.Text, MinSelection_start.Text,
                AM_PM_Selection_end.Text, HrSelection_end.Text, MinSelection_end.Text
            };

            Validator addAppointmentValidator = new Validator();
            
            if (addAppointmentValidator.CheckForNulls(appointmentInput))
            { BuildAppointment(); }
            else
            { MessageBox.Show(addAppointmentValidator.formError); }

        }

        // Cancel and Return to Dashboard
        private void CustomerCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard cancelAddAppointment = new Dashboard(loggedConsultant_AA);
            cancelAddAppointment.Show();

            this.Close();
        }
    }
}
