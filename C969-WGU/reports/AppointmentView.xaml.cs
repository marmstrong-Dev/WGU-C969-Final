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
    /// Interaction logic for AppointmentView.xaml
    /// </summary>
    public partial class AppointmentView : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        DataTable appointmentsViewTbl = new DataTable();
        Consultant viewingConsultant_AV;

        // Constructor
        public AppointmentView(Consultant viewer)
        {
            InitializeComponent();
            viewingConsultant_AV = viewer;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MonthPickerInit();
            AppointmentsViewBuilder();

            IOLabel.Content = $"Total In Office:  { TypeCounters("In House") }";
            OSLabel.Content = $"Total Offsite:  { TypeCounters("Offsite") }";
            TCLabel.Content = $"Total Teleconference:  { TypeCounters("Teleconference") }";

            FilterOffInput.IsChecked = true;
        }

        // Sets Year Placeholder Text
        private void YearPicker_GotFocus(object sender, RoutedEventArgs e)
        { 
            if (YearPicker.Text == "YYYY")
            { YearPicker.Text = ""; }
        }

        private void YearPicker_LostFocus(object sender, RoutedEventArgs e)
        {
            if (YearPicker.Text == "")
            { YearPicker.Text = "YYYY"; }
        }

        // Populates Month Picker List
        private void MonthPickerInit()
        {
            string[] monthList =
            {
                "January", "February", "March",
                "April", "May", "June",
                "July", "August", "September",
                "October", "November", "December"
            };

            MonthPicker.Items.Add("Month");
            MonthPicker.SelectedIndex = 0;

            for (int i = 0; i < monthList.Length; i++)
            { MonthPicker.Items.Add(monthList[i]); }
        }

        // Appointments Table Builder
        private void AppointmentsViewBuilder()
        {
            string appointmentViewQuery = @"SELECT appointmentId, title, description, customerName, userName, location, contact, type, start, end, url, appointment.createDate, appointment.lastUpdate, appointment.lastUpdateBy
                                            FROM appointment
                                            LEFT JOIN customer
                                            ON appointment.customerId = customer.customerId
                                            LEFT JOIN user
                                            ON appointment.userId = user.userId;";

            dbCon.Open();

            MySqlDataAdapter appointmentsViewAdapter = new MySqlDataAdapter();
            appointmentsViewAdapter.SelectCommand = new MySqlCommand(appointmentViewQuery, dbCon);
            appointmentsViewAdapter.Fill(appointmentsViewTbl);

            AppointmentReportTbl.ItemsSource = appointmentsViewTbl.DefaultView;

            dbCon.Close();
        }

        // Populate Type Counters
        private int TypeCounters(string appointmentType)
        {
            int typeCounter = 0;
            
            for (int i = 0; i < appointmentsViewTbl.Rows.Count; i++)
            {
                if (appointmentsViewTbl.Rows[i].ItemArray[7].ToString() == appointmentType)
                { typeCounter++; }
            }

            return typeCounter;
        }

        // Month / Week Filters
        private void MonthWeekChange()
        {
        }

        /*
        Table Filter Events
        */

        private void IOFilterInput_Checked(object sender, RoutedEventArgs e)
        { TableTypeFilters("In Office"); }

        private void OSFilterInput_Checked(object sender, RoutedEventArgs e)
        { TableTypeFilters("Offsite"); }

        private void TCFilterInput_Checked(object sender, RoutedEventArgs e)
        { TableTypeFilters("Teleconference"); }

        private void FilterOffInput_Checked(object sender, RoutedEventArgs e)
        {
            appointmentsViewTbl.Clear();
            AppointmentsViewBuilder();
        }

        // Method to Filter Based on Type
        private void TableTypeFilters(string filterType)
        {
            appointmentsViewTbl.Clear();
            AppointmentsViewBuilder();

            for (int i = 0; i < appointmentsViewTbl.Rows.Count; i++)
            {
                if (appointmentsViewTbl.Rows[i].ItemArray[7].ToString() != filterType)
                { appointmentsViewTbl.Rows[i].Delete(); }
            }

            AppointmentReportTbl.ItemsSource = appointmentsViewTbl.DefaultView;
        }

        // Date Filter Button
        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            appointmentsViewTbl.Clear();
            AppointmentsViewBuilder();

            for (int i = 0; i < appointmentsViewTbl.Rows.Count; i++)
            {
                DateTime filterDT = DateTime.Parse(appointmentsViewTbl.Rows[i].ItemArray[8].ToString());

                if (filterDT.Month != MonthPicker.SelectedIndex)
                { appointmentsViewTbl.Rows[i].Delete(); }
                else if (filterDT.Year != Int32.Parse(YearPicker.Text))
                { appointmentsViewTbl.Rows[i].Delete(); }
            }

            SelectedDateLabel.Content = $"Date: { MonthPicker.SelectedItem } { YearPicker.Text }";

            AppointmentReportTbl.ItemsSource = appointmentsViewTbl.DefaultView;
        }

        // Reset Date Filters
        private void ResetFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            appointmentsViewTbl.Clear();
            AppointmentsViewBuilder();
            SelectedDateLabel.Content = "Date: ";
        }

        // Exit and Return to Dashboard
        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard exitAppointmentView = new Dashboard(viewingConsultant_AV);
            exitAppointmentView.Show();

            this.Close();
        }
    }
}
