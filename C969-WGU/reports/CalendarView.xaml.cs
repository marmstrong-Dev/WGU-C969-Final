using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Net;
using MySqlX.XDevAPI.Relational;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

namespace C969_Final
{
    /// <summary>
    /// Interaction logic for CalendarView.xaml
    /// </summary>
    public partial class CalendarView : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        DataTable calendarViewTbl = new DataTable();
        Consultant viewingConsultant_CalView = new Consultant();
        public delegate bool CalHaving(int monthNum);

        // Constructor
        public CalendarView(Consultant viewer)
        {
            InitializeComponent();
            viewingConsultant_CalView = viewer;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        
        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        { BuildDateFilter(); }

        // Fill Date Filter Categories
        private void BuildDateFilter()
        {
            string[] filterCategories =
            {
                "All Upcoming Appointments",
                "Appointments This Week",
                "Appointments This Month"
            };

            foreach (string filter in filterCategories)
            { TimeFilterSelector.Items.Add(filter); }

            TimeFilterSelector.SelectedIndex = 0;
        }

        // Appointments Table Builder
        private void CalendarViewBuilder()
        {
            int localMod = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
            string calendarViewQuery = $"SELECT appointmentId, title, description, customerName, userName, location, contact, type, url, " +
                                       $"CONVERT_TZ(start, '+0:00', '{ localMod }:00'), CONVERT_TZ(end, '+0:00', '{ localMod }:00'), CONVERT_TZ(appointment.createDate, '+0:00', '{ localMod }:00'), CONVERT_TZ(appointment.lastUpdate, '+0:00', '{ localMod }:00'), appointment.lastUpdateBy " +
                                       $"FROM appointment " +
                                       $"LEFT JOIN customer ON appointment.customerId = customer.customerId " +
                                       $"LEFT JOIN user ON appointment.userId = user.userId " +
                                       $"WHERE start > utc_timestamp;";

            dbCon.Open();

            MySqlDataAdapter calendarViewAdapter = new MySqlDataAdapter();
            calendarViewAdapter.SelectCommand = new MySqlCommand(calendarViewQuery, dbCon);
            calendarViewAdapter.Fill(calendarViewTbl);

            CalendarReportTbl.ItemsSource = calendarViewTbl.DefaultView;

            dbCon.Close();

            CalendarReportTbl.Columns[0].Header = "Appointment ID";
            CalendarReportTbl.Columns[1].Header = "Title";
            CalendarReportTbl.Columns[2].Header = "Description";
            CalendarReportTbl.Columns[3].Header = "Customer Name";
            CalendarReportTbl.Columns[4].Header = "Consultant Name";
            CalendarReportTbl.Columns[5].Header = "Location";
            CalendarReportTbl.Columns[6].Header = "Contact";
            CalendarReportTbl.Columns[7].Header = "Appointment Type";
            CalendarReportTbl.Columns[8].Header = "URL";
            CalendarReportTbl.Columns[9].Header = "Start Time";
            CalendarReportTbl.Columns[10].Header = "End Time";
            CalendarReportTbl.Columns[11].Header = "Created Date";
            CalendarReportTbl.Columns[12].Header = "Last Update";
            CalendarReportTbl.Columns[13].Header = "Updated By";

            LabelFiller("All");
        }

        // Filter By Appointments In The Next Week
        private void DateFilter(int daysToFilter)
        {
            calendarViewTbl.Clear();
            CalendarViewBuilder();

            for (int i = 0; i < calendarViewTbl.Rows.Count; i++)
            {
                try
                {
                    DateTime candidateDate = DateTime.Parse(calendarViewTbl.Rows[i].ItemArray[9].ToString());

                    if (candidateDate >= DateTime.Now.AddDays(daysToFilter))
                    { calendarViewTbl.Rows[i].Delete(); }
                }
                catch 
                { MessageBox.Show("No Appointments Found"); }
            }

            CalendarReportTbl.ItemsSource = calendarViewTbl.DefaultView;
            LabelFiller(DateTime.Now.AddDays(daysToFilter).ToShortDateString());
        }

        // On Filter Selection Change
        private void TimeFilterSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filteredInput = TimeFilterSelector.SelectedItem.ToString();

            switch (filteredInput)
            {
                case "Appointments This Week":
                    DateFilter(7);
                    break;

                case "Appointments This Month":
                    DateFilter(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day);
                    break;

                case "All Upcoming Appointments":
                    calendarViewTbl.Clear();
                    CalendarViewBuilder();
                    break;
            }
        }

        // Label Modifier
        private void LabelFiller(string dateRange)
        {
            AppointmentCounterLabel.Content = $"Total Appointments: { CalendarReportTbl.Items.Count - 1 }";
            DateRangeLabel.Content = $"Dates Showing: { DateTime.Now.ToShortDateString() }  -  { dateRange }";
        }

        // Exit and Return To Dashboard
        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard exitAppointmentView = new Dashboard(viewingConsultant_CalView);
            exitAppointmentView.Show();

            this.Close();
        }
    }
}
