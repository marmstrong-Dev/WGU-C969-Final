using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;
using System.Data;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");
        private DataTable appointmentsDataTbl = new DataTable();
        private DataTable customerDataTbl = new DataTable();
        public Consultant loggedConsultant = new Consultant();

        // Constructor
        public Dashboard(Consultant passedCons)
        {
            InitializeComponent();
            loggedConsultant = passedCons;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // On Window Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CustomerTableBuilder();
            AppointmentsTableBuilder();
            PopulateConsultantInfo();
            NotificationBox();
        }

        // Fill Labels For Consultant Info
        private void PopulateConsultantInfo()
        {
            ConsultantIDLabel.Content = $"Consultant ID:  { loggedConsultant.consultantID }";
            ConsultantNameLabel.Content = $"Welcome:  { loggedConsultant.consultantName }";
        }

        // Creates Notification If Consultant Has Appointments Starting Within 15 Minutes
        public void NotificationBox()
        {
            if (loggedConsultant.CheckAppointments() == true)
            { MessageBox.Show("15 Mintue Warning: You Have Pending Appointments"); }

            Timer intervalTime = new Timer();
            intervalTime.AutoReset = true;
            intervalTime.Enabled = true;
            intervalTime.Interval = 900000;
            intervalTime.Elapsed += IntervalTime_Elapsed;
            intervalTime.Start();
        }

        private void IntervalTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (loggedConsultant.CheckAppointments() == true)
            { MessageBox.Show("15 Mintue Warning: You Have Pending Appointments"); }
        }

        /*
        Table Builder Methods
        */

        // Builds Customer Data Table From DB
        private void CustomerTableBuilder()
        {
            customerDataTbl.Columns.Add(new DataColumn("Selected", typeof(bool)));
            customerDataTbl.Columns[0].DefaultValue = false;
            
            string customerTableQuery = @"SELECT customerId, customerName, active, phone, country, city, address, postalCode
                                            FROM customer
                                            LEFT JOIN address
                                            ON customer.addressId = address.addressId
                                            LEFT JOIN city
                                            ON address.cityId = city.cityId
                                            LEFT JOIN country
                                            ON city.countryID = country.countryId;";

            dbCon.Open();

            MySqlDataAdapter customerDataAdapter = new MySqlDataAdapter();
            customerDataAdapter.SelectCommand = new MySqlCommand(customerTableQuery, dbCon);
            customerDataAdapter.Fill(customerDataTbl);

            CustomersDataTbl.ItemsSource = customerDataTbl.DefaultView;

            dbCon.Close();

            CustomersDataTbl.Columns[1].Header = "Customer ID";
            CustomersDataTbl.Columns[2].Header = "Customer Name";
            CustomersDataTbl.Columns[3].Header = "Is Active";
            CustomersDataTbl.Columns[4].Header = "Phone Number";
            CustomersDataTbl.Columns[5].Header = "Country";
            CustomersDataTbl.Columns[6].Header = "City";
            CustomersDataTbl.Columns[7].Header = "Address";
            CustomersDataTbl.Columns[8].Header = "Postal Code";

            CustomersDataTbl.Columns[3].IsReadOnly = true;
        }

        // Builds Appointments Data Table From DB
        private void AppointmentsTableBuilder()
        {
            appointmentsDataTbl.Columns.Add(new DataColumn("Selected", typeof(bool)));
            appointmentsDataTbl.Columns[0].DefaultValue = false;

            string appointmentTableQuery = @"SELECT appointmentId, title, description, customerName, userName, location, contact, type, url 
                                            FROM appointment
                                            LEFT JOIN customer
                                            ON appointment.customerId = customer.customerId
                                            LEFT JOIN user
                                            ON appointment.userId = user.userId;";

            dbCon.Open();

            MySqlDataAdapter appointmentsDataAdapter = new MySqlDataAdapter();
            appointmentsDataAdapter.SelectCommand = new MySqlCommand(appointmentTableQuery, dbCon);
            appointmentsDataAdapter.Fill(appointmentsDataTbl);

            AppointmentsDataTbl.ItemsSource = appointmentsDataTbl.DefaultView;

            dbCon.Close();

            AppointmentsDataTbl.Columns[1].Header = "Appointment ID";
            AppointmentsDataTbl.Columns[2].Header = "Title";
            AppointmentsDataTbl.Columns[3].Header = "Description";
            AppointmentsDataTbl.Columns[4].Header = "Customer Name";
            AppointmentsDataTbl.Columns[5].Header = "Consultant Name";
            AppointmentsDataTbl.Columns[6].Header = "Location";
            AppointmentsDataTbl.Columns[7].Header = "Contact";
            AppointmentsDataTbl.Columns[8].Header = "Appointment Type";
            AppointmentsDataTbl.Columns[9].Header = "URL";
        }

        /*
        View Button Methods
        */

        private void AppointmentViewBtn_Click(object sender, RoutedEventArgs e)
        {
            AppointmentView newAppointmentReport = new AppointmentView(loggedConsultant);
            newAppointmentReport.Show();

            this.Close();
        }

        private void ConsultantViewBtn_Click(object sender, RoutedEventArgs e)
        {
            ConsultantView newConsultantReport = new ConsultantView(loggedConsultant);
            newConsultantReport.Show();

            this.Close();
        }

        private void CustomersViewBtn_Click(object sender, RoutedEventArgs e)
        {
            CustomerView newCustomerReport = new CustomerView(loggedConsultant);
            newCustomerReport.Show();

            this.Close();
        }

        private void CalendarViewBtn_Click(object sender, RoutedEventArgs e)
        {
            CalendarView newCalendarReport = new CalendarView(loggedConsultant);
            newCalendarReport.Show();

            this.Close();
        }

        /*
        Customer Button Methods
        */

        private void AddCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerForm newCustomer = new AddCustomerForm(loggedConsultant);
            newCustomer.Show();

            this.Close();
        }

        private void EditCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            Validator editCustomerValidator = new Validator();
            if (editCustomerValidator.CheckFieldQty(customerDataTbl) == true)
            {
                EditCustomerForm editCustomer = new EditCustomerForm(editCustomerValidator.idResult, loggedConsultant);
                editCustomer.Show();

                this.Close();
            }
            else
            { MessageBox.Show("Invalid Quantity of Selections"); }
        }

        private void DeleteCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            Customer deletedCustomer = new Customer();
            Validator deleteCustomerValidator = new Validator();
            bool canDelete = false;

            MessageBoxResult confirmDeleteCustomer = MessageBox.Show("Are You Sure", "Customer(s) Deleted", MessageBoxButton.YesNo);
            if (confirmDeleteCustomer == System.Windows.MessageBoxResult.Yes)
            {
                for (int i = 0; i < customerDataTbl.Rows.Count; i++)
                {
                    if ((bool)customerDataTbl.Rows[i].ItemArray[0] == true)
                    {
                        if (deleteCustomerValidator.CheckForAppointments(Int32.Parse(customerDataTbl.Rows[i].ItemArray[1].ToString())) == true)
                        { canDelete = true; }
                    }
                }
            }

            if (canDelete == true)
            {
                for (int i = 0; i < customerDataTbl.Rows.Count; i++)
                {
                    if ((bool)customerDataTbl.Rows[i].ItemArray[0] == true)
                    {
                        deletedCustomer.customerID = Int32.Parse(customerDataTbl.Rows[i].ItemArray[1].ToString());
                        deletedCustomer.DeleteCustomer();
                    }
                }
            }
            else
            { MessageBox.Show("Cannot Delete Customers With Active Appointments"); }

            Dashboard refreshedDashboard = new Dashboard(loggedConsultant);
            refreshedDashboard.Show();

            this.Close();
        }

        /*
        Appointment Button Methods
        */

        private void AddApointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            AddAppointmentForm addAppointment = new AddAppointmentForm(loggedConsultant);
            addAppointment.Show();

            this.Close();
        }

        private void EditApointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            Validator editAppointmentValidator = new Validator();

            if (editAppointmentValidator.CheckFieldQty(appointmentsDataTbl) == true)
            {
                EditAppointmentForm editAppointment = new EditAppointmentForm(editAppointmentValidator.idResult, loggedConsultant);
                editAppointment.Show();

                this.Close();
            }
            else
            { MessageBox.Show("Invalid Quantity of Selections"); }
        }

        private void DeleteApointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            Appointment deletedAppointment = new Appointment();

            MessageBoxResult confirmDeleteAppointment = MessageBox.Show("Are You Sure", "Appointment(s) Deleted", MessageBoxButton.YesNo);
            if (confirmDeleteAppointment == System.Windows.MessageBoxResult.Yes)
            {
                for (int i = 0; i < appointmentsDataTbl.Rows.Count; i++)
                {
                    if ((bool)appointmentsDataTbl.Rows[i].ItemArray[0] == true)
                    {
                        deletedAppointment.appointmentID = Int32.Parse(appointmentsDataTbl.Rows[i].ItemArray[1].ToString());
                        deletedAppointment.DeleteAppointment();
                    }
                }

                Dashboard refreshedDashboard = new Dashboard(loggedConsultant);
                refreshedDashboard.Show();

                this.Close();
            }
        }

        // Sign Out and Return to Main Login Window
        private void SignOutBtn_Click(object sender, RoutedEventArgs e)
        {
            Log signoutLog = new Log(loggedConsultant.consultantName);
            signoutLog.LogoutRecord();

            MainWindow returnToLogin = new MainWindow();
            returnToLogin.Show();

            this.Close();
        }
    }
}
