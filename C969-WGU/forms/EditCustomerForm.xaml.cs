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
    /// Interaction logic for EditCustomerForm.xaml
    /// </summary>
    public partial class EditCustomerForm : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        Customer workingCustomer_edit = new Customer();
        Address workingAddress_edit = new Address();
        Consultant loggedConsultant_EC = new Consultant();

        // Constructor
        public EditCustomerForm(int selectionID, Consultant editingUser)
        {
            InitializeComponent();
            workingCustomer_edit.customerID = selectionID;
            loggedConsultant_EC = editingUser;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FormFillCustomer();
            FormFillAddress();
        }

        /*
        Fill Form With Info From Selected Customer
        */

        // Generate Editable Customer Form
        private void FormFillCustomer()
        {
            workingAddress_edit.addressID = workingCustomer_edit.LookupCustomer();

            if (workingCustomer_edit.isActive == true)
            { activeCustomerInput.IsChecked = true; }
            else
            { inactiveCustomerInput.IsChecked = true; }

            CustomerNameInput_edit.Text = workingCustomer_edit.customerName;
            CustomerIDLabel_edit.Content = $"Customer ID: { workingCustomer_edit.customerID }";
        }

        // Generate Editable Address Form
        private void FormFillAddress()
        {
            workingAddress_edit.cityID = workingAddress_edit.LookupAddress();
            workingAddress_edit.countryID = workingAddress_edit.LookupCity();
            workingAddress_edit.LookupCountry();

            AddressStreetInput_edit.Text = workingAddress_edit.addressStreet;
            AddressAptInput_edit.Text = workingAddress_edit.addressApt;
            AddressPostalInput_edit.Text = workingAddress_edit.addressPostal;
            AddressCityInput_edit.Text = workingAddress_edit.cityName;
            AddressCountryInput_edit.Text = workingAddress_edit.countryName;
            AddressPhoneInput_edit.Text = workingAddress_edit.phoneNumber;
        }

        // Build Address
        private void BuildAddress_edit()
        {
            workingAddress_edit.addressStreet = AddressStreetInput_edit.Text;
            workingAddress_edit.addressApt = AddressAptInput_edit.Text;
            workingAddress_edit.phoneNumber = AddressPhoneInput_edit.Text;
            workingAddress_edit.addressPostal = AddressPostalInput_edit.Text;

            workingAddress_edit.addressID = workingAddress_edit.EditAddress(loggedConsultant_EC.consultantName);
        }

        // Build Customer
        private void BuildCustomer_edit()
        {
            workingCustomer_edit.customerName = CustomerNameInput_edit.Text;

            if (activeCustomerInput.IsChecked == true)
            { workingCustomer_edit.isActive = true; }
            else if (inactiveCustomerInput.IsChecked == true)
            { workingCustomer_edit.isActive = false; }

            workingCustomer_edit.EditCustomer(workingAddress_edit.addressID, loggedConsultant_EC.consultantName);
        }

        /*
        Button Methods
        */

        // Save Changes to Existing Customer
        private void CustomerSaveBtn_edit_Click(object sender, RoutedEventArgs e)
        {
            string[] customerInput_edit =
            {
                CustomerNameInput_edit.Text,
                AddressPhoneInput_edit.Text,
                AddressCountryInput_edit.Text,
                AddressAptInput_edit.Text,
                AddressPostalInput_edit.Text,
                AddressCityInput_edit.Text,
                AddressStreetInput_edit.Text
            };

            Validator editCustomerValidator = new Validator();

            if (editCustomerValidator.CheckForNulls(customerInput_edit) == true)
            {
                // Lambda Expression used to strip non numeric characters from phone number
                string formattedPhone = string.Concat(AddressPhoneInput_edit.Text.Where(a => char.IsDigit(a)));

                if (editCustomerValidator.CheckNums(formattedPhone, AddressPostalInput_edit.Text) == true)
                {
                    AddressPhoneInput_edit.Text = formattedPhone;

                    if (editCustomerValidator.CheckIfExists(AddressCountryInput_edit.Text, "country", "country") == true)
                    { workingAddress_edit.countryID = editCustomerValidator.idResult; }
                    else
                    {
                        workingAddress_edit.countryID = workingAddress_edit.AddCountry(AddressCountryInput_edit.Text, loggedConsultant_EC.consultantName);
                        workingAddress_edit.countryName = AddressCountryInput_edit.Text;
                    }

                    if (editCustomerValidator.CheckIfExists(AddressCityInput_edit.Text, "city", "city") == true)
                    { workingAddress_edit.cityID = editCustomerValidator.idResult; }
                    else
                    {
                        workingAddress_edit.cityID = workingAddress_edit.AddCity(AddressCityInput_edit.Text, workingAddress_edit.countryID, loggedConsultant_EC.consultantName);
                        workingAddress_edit.cityName = AddressCityInput_edit.Text;
                    }

                    BuildAddress_edit();
                    BuildCustomer_edit();

                    Dashboard savedEditCustomer = new Dashboard(loggedConsultant_EC);
                    savedEditCustomer.Show();

                    this.Close();
                }
                else 
                { MessageBox.Show(editCustomerValidator.formError); }
            }
            else
            { MessageBox.Show(editCustomerValidator.formError); }
        }

        // Cancel Changes and Return to Dashboard
        private void CustomerCancelBtn_edit_Click(object sender, RoutedEventArgs e)
        {
            Dashboard cancelEditCustomer = new Dashboard(loggedConsultant_EC);
            cancelEditCustomer.Show();

            this.Close();
        }
    }
}
