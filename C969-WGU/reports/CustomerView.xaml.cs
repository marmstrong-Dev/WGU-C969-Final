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
    /// Interaction logic for CustomerView.xaml
    /// </summary>
    public partial class CustomerView : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        DataTable customerViewTbl = new DataTable();
        Consultant viewingConsultant_CV;
        private List<string> countryNames = new List<string>();
        
        // Constructor
        public CustomerView(Consultant viewer)
        {
            InitializeComponent();
            viewingConsultant_CV = viewer;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillCountriesList();
            FillCustomerViewTable();
        }

        // Sets Name Search Placeholder Text
        private void NameSearchInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NameSearchInput.Text == "Search Customer Name")
            { NameSearchInput.Text = ""; }
        }

        private void NameSearchInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NameSearchInput.Text == "")
            { NameSearchInput.Text = "Search Customer Name"; }
        }

        // Populate Countries List
        private void FillCountriesList()
        {
            string selectCountriesQuery = "SELECT country FROM country;";

            dbCon.Open();

            MySqlCommand selectCountriesCommand = new MySqlCommand(selectCountriesQuery, dbCon);
            MySqlDataReader selectCountriesReader = selectCountriesCommand.ExecuteReader();

            while (selectCountriesReader.Read())
            { countryNames.Add(selectCountriesReader.GetString(0)); }

            dbCon.Close();

            CountryPicker.Items.Add("");
            CountryPicker.SelectedIndex = 0;

            // Lambda to Populate Countries Filter Drop Down
            countryNames.ForEach(cName => CountryPicker.Items.Add(cName));
        }

        // Populate Table for Customers
        private void FillCustomerViewTable()
        {
            int localMod = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
            string customerViewQuery = $"SELECT customerId, customerName, active, address, address2, city, country, phone, CONVERT_TZ(customer.createDate, '+0:00', '{ localMod }:00'), CONVERT_TZ(customer.lastUpdate, '+0:00', '{ localMod }:00'), customer.lastUpdateBy " +
                                       $"FROM customer " +
                                       $"LEFT JOIN address ON customer.addressId = address.addressId " +
                                       $"LEFT JOIN city ON address.cityId = city.cityId " +
                                       $"LEFT JOIN country ON city.countryId = country.countryId;";

            dbCon.Open();

            MySqlDataAdapter customerViewAdapter = new MySqlDataAdapter();
            customerViewAdapter.SelectCommand = new MySqlCommand(customerViewQuery, dbCon);
            customerViewAdapter.Fill(customerViewTbl);

            CustomersReportTbl.ItemsSource = customerViewTbl.DefaultView;

            dbCon.Close();

            CustomersReportTbl.Columns[0].Header = "Customer ID";
            CustomersReportTbl.Columns[1].Header = "Customer Name";
            CustomersReportTbl.Columns[2].Header = "Is Active";
            CustomersReportTbl.Columns[3].Header = "Address";
            CustomersReportTbl.Columns[4].Header = "Apt / Unit";
            CustomersReportTbl.Columns[5].Header = "City";
            CustomersReportTbl.Columns[6].Header = "Country";
            CustomersReportTbl.Columns[7].Header = "Phone Number";
            CustomersReportTbl.Columns[8].Header = "Created Date";
            CustomersReportTbl.Columns[9].Header = "Last Updated";
            CustomersReportTbl.Columns[10].Header = "Updated By";

            CustomersReportTbl.Columns[2].IsReadOnly = true;
        }

        /*
        Button Methods 
        */

        // Filter Customer Table By Name
        private void NameSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            customerViewTbl.Clear();
            FillCustomerViewTable();

            for (int i = 0; i < customerViewTbl.Rows.Count; i++)
            {
                if (!customerViewTbl.Rows[i].ItemArray[1].ToString().Contains(NameSearchInput.Text)) 
                { customerViewTbl.Rows[i].Delete(); }
            }

            CustomersReportTbl.ItemsSource = customerViewTbl.DefaultView;
        }

        // Filter Customer Table By Country
        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            customerViewTbl.Clear();
            FillCustomerViewTable();

            for (int i = 0; i < customerViewTbl.Rows.Count; i++)
            {
                if (CityPicker.Text != "")
                {
                    if (CityPicker.Text != customerViewTbl.Rows[i].ItemArray[5].ToString())
                    { customerViewTbl.Rows[i].Delete(); }
                }
                else if (CountryPicker.SelectedItem.ToString() != "")
                {
                    if (CountryPicker.SelectedItem.ToString() != customerViewTbl.Rows[i].ItemArray[6].ToString())
                    { customerViewTbl.Rows[i].Delete(); }
                }
                else
                { Console.WriteLine("Not Found"); }
            }

             CustomersReportTbl.ItemsSource = customerViewTbl.DefaultView;
        }

        // Reset Filters on Customer Table
        private void ResetFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            customerViewTbl.Clear();
            FillCustomerViewTable();
            SelectedLocationLabel.Content = "";

        }

        // Exit and  Return to Dashboard
        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard exitAppointmentView = new Dashboard(viewingConsultant_CV);
            exitAppointmentView.Show();

            this.Close();
        }
    }
}
