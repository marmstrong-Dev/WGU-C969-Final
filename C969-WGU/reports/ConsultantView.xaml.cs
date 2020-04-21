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
    /// Interaction logic for ConsultantView.xaml
    /// </summary>
    public partial class ConsultantView : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        DataTable consultantViewTbl = new DataTable();
        Consultant loggedUser;
        private List<string> consultantsNames = new List<string>();

        // Consultant
        public ConsultantView(Consultant viewer)
        {
            InitializeComponent();
            loggedUser = viewer;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        { FillConsultantsList(); }

        // Runs on Consultant Selection Change
        private void ConsultantsListInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { FillLabel(ConsultantsListInput.SelectedItem.ToString()); }

        // Populate Consultants List
        private void FillConsultantsList()
        {
            string selectConsultantsQuery = "SELECT userName FROM user;";

            dbCon.Open();

            MySqlCommand selectConsultantsCommand = new MySqlCommand(selectConsultantsQuery, dbCon);
            MySqlDataReader selectConsultantReader = selectConsultantsCommand.ExecuteReader();

            while (selectConsultantReader.Read())
            { consultantsNames.Add(selectConsultantReader.GetString(0)); }

            dbCon.Close();

            for (int i = 0; i < consultantsNames.Count; i++)
            { ConsultantsListInput.Items.Add(consultantsNames[i]); }
        }

        // Fill Label Fields
        private void FillLabel(string selectionName)
        {
            Consultant selectedConsultant = new Consultant();
            selectedConsultant.consultantName = selectionName;

            ConsultantIDLabel.Content = $"Consultant ID: { selectedConsultant.LookupConsultant() }";
            ConsultantNameLabel.Content = $"Name: { selectedConsultant.consultantName }";
            
            if (selectedConsultant.activeConsultant == true)
            { ConsultantStatus.Content = "Status: Active"; }
            else
            { ConsultantStatus.Content = "Status: Inactive"; }

            FillConsultantTable(selectedConsultant.consultantID);
        }

        // Populate Consultant Table Based on Selection
        private void FillConsultantTable(int selectionID)
        {
            int localMod = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
            consultantViewTbl.Clear();

            string selectedScheduleQuery = $"SELECT appointmentId, title, description, customerName, userName, location, contact, type, url, " +
                                           $"CONVERT_TZ(start, '+0:00', '{ localMod }:00'), CONVERT_TZ(end, '+0:00', '{ localMod }:00'), CONVERT_TZ(appointment.createDate, '+0:00', '{ localMod }:00'), CONVERT_TZ(appointment.lastUpdate, '+0:00', '{ localMod }:00'), appointment.lastUpdateBy " +
                                           $"FROM appointment " +
                                           $"LEFT JOIN customer ON appointment.customerId = customer.customerId " +
                                           $"LEFT JOIN user ON appointment.userId = user.userId " +
                                           $"WHERE user.userId = { selectionID };";

            dbCon.Open();

            MySqlDataAdapter selectedScheduleAdapter = new MySqlDataAdapter();
            selectedScheduleAdapter.SelectCommand = new MySqlCommand(selectedScheduleQuery, dbCon);
            selectedScheduleAdapter.Fill(consultantViewTbl);

            ConsultantsReportTbl.ItemsSource = consultantViewTbl.DefaultView;

            dbCon.Close();

            ConsultantsReportTbl.Columns[0].Header = "Appointment ID";
            ConsultantsReportTbl.Columns[1].Header = "Title";
            ConsultantsReportTbl.Columns[2].Header = "Description";
            ConsultantsReportTbl.Columns[3].Header = "Customer Name";
            ConsultantsReportTbl.Columns[4].Header = "Consultant Name";
            ConsultantsReportTbl.Columns[5].Header = "Location";
            ConsultantsReportTbl.Columns[6].Header = "Contact";
            ConsultantsReportTbl.Columns[7].Header = "Appointment Type";
            ConsultantsReportTbl.Columns[8].Header = "URL";
            ConsultantsReportTbl.Columns[9].Header = "Start Time";
            ConsultantsReportTbl.Columns[10].Header = "End Time";
            ConsultantsReportTbl.Columns[11].Header = "Created Date";
            ConsultantsReportTbl.Columns[12].Header = "Last Updated";
            ConsultantsReportTbl.Columns[13].Header = "Updated By";
        }

        // Exit and Return to Dashboard
        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard exitConsultantView = new Dashboard(loggedUser);
            exitConsultantView.Show();

            this.Close();
        }
    }
}
