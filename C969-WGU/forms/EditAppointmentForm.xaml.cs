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

namespace C969_Final
{
    /// <summary>
    /// Interaction logic for EditAppointmentForm.xaml
    /// </summary>
    public partial class EditAppointmentForm : Window
    {
        Appointment workingAppointment = new Appointment();
        Consultant loggedConsultant_EA = new Consultant();

        // Constructor
        public EditAppointmentForm(int selectionID, Consultant editingUser)
        {
            InitializeComponent();
            workingAppointment.appointmentID = selectionID;
            loggedConsultant_EA = editingUser;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FormFillAppointment();
            TimePickerInit_edit();
        }

        // Initialize Combo Boxes
        private void TimePickerInit_edit()
        {
            int hourSelector_start = 0;
            int hourSelector_end = 0;

            AM_PM_Selection_start_edit.Items.Add("AM");
            AM_PM_Selection_start_edit.Items.Add("PM");
            if (workingAppointment.startTime.Hour >= 12)
            {
                AM_PM_Selection_start_edit.SelectedIndex = 1;
                if (workingAppointment.startTime.Hour == 12)
                { hourSelector_start = workingAppointment.startTime.Hour; }
                else
                { hourSelector_start = workingAppointment.startTime.Hour - 12; }
            }
            else 
            {
                hourSelector_start = workingAppointment.startTime.Hour;
                AM_PM_Selection_start_edit.SelectedIndex = 0;
            }

            AM_PM_Selection_end_edit.Items.Add("AM");
            AM_PM_Selection_end_edit.Items.Add("PM");
            if (workingAppointment.endTime.Hour >= 12)
            {
                AM_PM_Selection_end_edit.SelectedIndex = 1;
                if (workingAppointment.endTime.Hour == 12)
                { hourSelector_end = workingAppointment.endTime.Hour; }
                else
                { hourSelector_end = workingAppointment.endTime.Hour - 12; }
            }
            else
            {
                hourSelector_end = workingAppointment.endTime.Hour;
                AM_PM_Selection_end_edit.SelectedIndex = 0; 
            }
          
            HrSelection_start_edit.Items.Add("Hours");
            HrSelection_end_edit.Items.Add("Hours");
            HrSelection_start_edit.SelectedIndex = hourSelector_start;
            HrSelection_end_edit.SelectedIndex = hourSelector_end;
            HourGenerator_edit(HrSelection_start_edit);
            HourGenerator_edit(HrSelection_end_edit);

            MinSelection_start_edit.Items.Add("Minutes");
            MinSelection_end_edit.Items.Add("Minutes");
            MinSelection_start_edit.SelectedIndex = workingAppointment.startTime.Minute + 1;
            MinSelection_end_edit.SelectedIndex = workingAppointment.endTime.Minute + 1;
            MinuteGenerator_edit(MinSelection_start_edit);
            MinuteGenerator_edit(MinSelection_end_edit);
        }

        // Populate Hours Selections
        private void HourGenerator_edit(ComboBox genHours)
        {
            for (int i = 1; i <= 12; i++)
            { genHours.Items.Add(i); }
        }

        // Populate Minutes Selections
        private void MinuteGenerator_edit(ComboBox genMinutes)
        {
            for (int i = 0; i < 60; i++)
            { genMinutes.Items.Add(i); }
        }

        // Populate Fields
        private void FormFillAppointment()
        {
            int custID = workingAppointment.LookupAppointment();

            CustomerIDInput_edit.Text = custID.ToString();
            AppointmentNameInput_edit.Text = workingAppointment.appointmentName;
            AppointmentDescriptionInput_edit.Text = workingAppointment.appointmentDescription;
            AppointmentLocationInput_edit.Text = workingAppointment.appointmentLocation;
            AppointmentContactInput_edit.Text = workingAppointment.appointmentContact;
            AppointmentURLInput_edit.Text = workingAppointment.appointmentURL;
            AppointmentDateInput_edit.SelectedDate = workingAppointment.startTime.ToLocalTime();

            if (workingAppointment.appointmentType == "Offsite")
            { OSSelected_edit.IsChecked = true; }
            else if (workingAppointment.appointmentType == "Teleconference")
            { TCSelected_edit.IsChecked = true; }

            AppointmentIDLabel_edit.Content = $"Appointment ID: { workingAppointment.appointmentID }";
        }

        // Lookup User ID and Customer ID
        private void CustomerIDLookup_edit()
        {
            Validator idValidator_edit = new Validator();

            if (idValidator_edit.CheckIfExists(CustomerIDInput_edit.Text, "customerId", "customer"))
            {
                int custID = Int32.Parse(CustomerIDInput_edit.Text);
                workingAppointment.EditAppointment(custID, loggedConsultant_EA.consultantID, loggedConsultant_EA.consultantName);

                Log editedAppointmentLog = new Log(loggedConsultant_EA.consultantName);
                editedAppointmentLog.EditRecord("Appointment", AppointmentNameInput_edit.Text);

                Dashboard savedAddAppointment = new Dashboard(loggedConsultant_EA);
                savedAddAppointment.Show();

                this.Close();
            }
            else
            { MessageBox.Show("Customer Does Not Exist"); }
        }

        // Build Edited Appointment
        private void BuildAppointment_edit()
        {
            workingAppointment.appointmentName = AppointmentNameInput_edit.Text;
            workingAppointment.appointmentDescription = AppointmentDescriptionInput_edit.Text;
            workingAppointment.appointmentLocation = AppointmentLocationInput_edit.Text;
            workingAppointment.appointmentContact = AppointmentContactInput_edit.Text;
            workingAppointment.appointmentURL = AppointmentURLInput_edit.Text;

            workingAppointment.startTime = BuildTimeStamp_edit(AM_PM_Selection_start_edit.Text, HrSelection_start_edit.Text, MinSelection_start_edit.Text);
            workingAppointment.endTime = BuildTimeStamp_edit(AM_PM_Selection_end_edit.Text, HrSelection_end_edit.Text, MinSelection_end_edit.Text);

            if (IOSelected_edit.IsChecked == true)
            { workingAppointment.appointmentType = "In Office"; }
            else if (OSSelected_edit.IsChecked == true)
            { workingAppointment.appointmentType = "Offsite"; }
            else if (TCSelected_edit.IsChecked == true)
            { workingAppointment.appointmentType = "Teleconference"; }

            Validator timeValidator_edit = new Validator();
            if (timeValidator_edit.CheckForAppointmentConflicts(workingAppointment.startTime.ToUniversalTime(), workingAppointment.endTime.ToUniversalTime(), loggedConsultant_EA.consultantID) == true)
            { CustomerIDLookup_edit(); }
            else 
            {
                if (workingAppointment.appointmentID != timeValidator_edit.idResult)
                { MessageBox.Show(timeValidator_edit.formError); }
                else 
                { CustomerIDLookup_edit(); }                     
            }
        }

        // Generate Edited TimeStamp
        private DateTime BuildTimeStamp_edit(string dayHalf, string hrSelection, string minSelection)
        {
            int hoursAdd;
            if (dayHalf == "PM" && Int32.Parse(hrSelection) != 12)
            { hoursAdd = Int32.Parse(hrSelection) + 12; }
            else
            { hoursAdd = Int32.Parse(hrSelection); }

            DateTime editedTime = new DateTime
            (
                AppointmentDateInput_edit.SelectedDate.Value.Year,
                AppointmentDateInput_edit.SelectedDate.Value.Month,
                AppointmentDateInput_edit.SelectedDate.Value.Day,
                hoursAdd,
                Int32.Parse(minSelection), 0
            );

            return editedTime;
        }

        /*
        Button Methods 
        */

        // Save Changes to Appointment
        private void AppointmentSaveBtn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (HrSelection_start_edit.Text == "Hours" || MinSelection_start_edit.Text == "Minutes")
            {
                HrSelection_start_edit.Text = "";
                MinSelection_start_edit.Text = "";
            }

            if (HrSelection_end_edit.Text == "Hours" || MinSelection_end_edit.Text == "Minutes")
            {
                HrSelection_end_edit.Text = "";
                MinSelection_end_edit.Text = "";
            }

            string[] appointmentInput_edit =
            {
                AppointmentNameInput_edit.Text,
                AppointmentContactInput_edit.Text,
                AppointmentLocationInput_edit.Text,
                AppointmentDescriptionInput_edit.Text,
                AppointmentURLInput_edit.Text,
                AM_PM_Selection_start_edit.Text, HrSelection_start_edit.Text, MinSelection_start_edit.Text,
                AM_PM_Selection_end_edit.Text, HrSelection_end_edit.Text, MinSelection_end_edit.Text
            };

            Validator editAppointmentValidator = new Validator();

            if (editAppointmentValidator.CheckForNulls(appointmentInput_edit))
            { BuildAppointment_edit(); }
            else
            { MessageBox.Show(editAppointmentValidator.formError); }
        }

        // Cancel Changes and Return to Dashboard
        private void AppointmentCancelBtn_edit_Click(object sender, RoutedEventArgs e)
        {
            Dashboard cancelEditAppointment = new Dashboard(loggedConsultant_EA);
            cancelEditAppointment.Show();

            this.Close();
        }
    }
}
