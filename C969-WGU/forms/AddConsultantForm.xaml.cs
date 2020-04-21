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

namespace C969_Final
{
    /// <summary>
    /// Interaction logic for AddConsultantForm.xaml
    /// </summary>
    public partial class AddConsultantForm : Window
    {
        // Constructor
        public AddConsultantForm()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        // Build New Consultant
        public void BuildConsultant()
        {
            Validator addConsultantValidator = new Validator();

            if (addConsultantValidator.CheckIfExists(NewConsultantNameInput.Text, "userName", "user") == true)
            { MessageBox.Show("Name Taken - Please Select Different User Name"); }
            else
            {
                Consultant addedConsultant = new Consultant();
                addedConsultant.consultantName = NewConsultantNameInput.Text;
                addedConsultant.consultantPass = InitPassInput.Password;
                addedConsultant.AddConsultant();

                MessageBox.Show("Registered Successfully");

                MainWindow successfulRegister = new MainWindow();
                successfulRegister.Show();

                this.Close();
            }
        }

        /*
        Button Methods
        */

        // Save New Consultant
        private void SaveConsultantBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NewConsultantNameInput.Text != "")
            {
                if (InitPassInput.Password == ConfirmPassInput.Password)
                { BuildConsultant(); }
                else 
                { MessageBox.Show("Passwords Do Not Match"); }
            }
            else 
            { MessageBox.Show("Please Enter a User Name"); }
        }

        // Cancel and Return to Login
        private void RegCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow cancelRegistration = new MainWindow();
            cancelRegistration.Show();

            this.Close();
        }
    }
}
