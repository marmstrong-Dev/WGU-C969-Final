using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
    /// Interaction logic for AddCustomerForm.xaml
    /// </summary>
    public partial class AddCustomerForm : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        Consultant loggedConsultant_AC = new Consultant();
        int nextID;

        // Constructor
        public AddCustomerForm(Consultant loggedUser)
        {
            InitializeComponent();
            loggedConsultant_AC = loggedUser;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateCustomerID();
        }

        // Generate Customer ID
        private void GenerateCustomerID()
        {
            string getNextIDQuery = $"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'U07042' AND TABLE_NAME = 'customer';";

            dbCon.Open();

            MySqlCommand getNextIDCommand = new MySqlCommand(getNextIDQuery, dbCon);
            MySqlDataReader getNextIDReader = getNextIDCommand.ExecuteReader();

            if (getNextIDReader.Read())
            { nextID = getNextIDReader.GetInt32(0); }

            dbCon.Close();

            CustomerIDLabel.Content = $"Customer ID: { nextID }";
        }

        // Build New Address
        private void BuildAddress()
        {
            Address addedAddress = new Address();
            Validator addAddressValidator = new Validator();

            if (addAddressValidator.CheckIfExists(AddressCountryInput.Text, "country", "country") == true)
            { addedAddress.countryID = addAddressValidator.idResult; }
            else
            {
                Country addedCountry = new Country();
                addedAddress.countryID = addedCountry.AddCountry(AddressCountryInput.Text, loggedConsultant_AC.consultantName);
            }

            if (addAddressValidator.CheckIfExists(AddressCityInput.Text, "city", "city") == true)
            { addedAddress.cityID = addAddressValidator.idResult; }
            else
            {
                City addedCity = new City();
                addedAddress.cityID = addedCity.AddCity(AddressCityInput.Text, addedAddress.countryID, loggedConsultant_AC.consultantName);
            }

            Console.WriteLine($"CountryID: { addedAddress.countryID }");
            Console.WriteLine($"CityID: { addedAddress.cityID }");

            addedAddress.addressStreet = AddressStreetInput.Text;
            addedAddress.addressApt = AddressAptInput.Text;
            addedAddress.addressPostal = AddressPostalInput.Text;
            addedAddress.phoneNumber = AddressPhoneInput.Text;

            addedAddress.AddAddress(loggedConsultant_AC.consultantName);

            BuildCustomer(addedAddress.addressID);
        }

        // Build New Customer
        private void BuildCustomer(int newAddressID)
        {
            Customer addedCustomer = new Customer();

            addedCustomer.customerName = CustomerNameInput.Text;
            addedCustomer.AddCustomer(newAddressID, loggedConsultant_AC.consultantName);
        }

        /*
        Button Methods
        */

        // Save New Customer
        private void CustomerSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] customerInput =
            {
                CustomerNameInput.Text,
                AddressPhoneInput.Text,
                AddressCountryInput.Text,
                AddressAptInput.Text,
                AddressPostalInput.Text,
                AddressCityInput.Text,
                AddressStreetInput.Text
            };

            Validator addCustomerValidator = new Validator();

            if (addCustomerValidator.CheckForNulls(customerInput) == true)
            {
                // Lambda Expression used to strip non numeric characters from phone number
                string formattedPhone = string.Concat(AddressPhoneInput.Text.Where(a => char.IsDigit(a)));

                if (addCustomerValidator.CheckNums(formattedPhone, AddressPostalInput.Text) == true)
                {
                    AddressPhoneInput.Text = formattedPhone;
                    BuildAddress();

                    Log addedCustLog = new Log(loggedConsultant_AC.consultantName);
                    addedCustLog.AddRecord("Customer", CustomerNameInput.Text);

                    Dashboard savedAddCustomer = new Dashboard(loggedConsultant_AC);
                    savedAddCustomer.loggedConsultant = loggedConsultant_AC;
                    savedAddCustomer.Show();

                    this.Close();
                }
                else 
                { MessageBox.Show(addCustomerValidator.formError); }
            }
            else 
            { MessageBox.Show(addCustomerValidator.formError); }
        }

        // Cancel and Return to Dashboard
        private void CustomerCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard cancelAddCustomer = new Dashboard(loggedConsultant_AC);
            cancelAddCustomer.Show();

            this.Close();
        }
    }
}
