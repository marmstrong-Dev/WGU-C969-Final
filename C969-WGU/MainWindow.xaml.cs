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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace C969_Final
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int timeZoneOffset;

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            RegionCode();
        }

        private void RegionCode() // FixMe
        {
            timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
            
            Console.WriteLine(timeZoneOffset);
            if (timeZoneOffset == 2)
            {
                WelcomeLabel.Content = "Planlegg Assistent";
                ConsultantNameLabel.Content = "Brukernavn";
                ConsultantPassLabel.Content = "Passord";

                LoginBtn.Content = "Logg Inn";
                RegisterBtn.Content = "Registrere";
                CloserBtn.Content = "Lukke";
            }
        }

        // Authenticate Consultant
        private void AuthenticateConsultant()
        {
            Consultant authCandidate = new Consultant();
            authCandidate.geoCode = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
            authCandidate.consultantName = ConsultantNameInput.Text;
            authCandidate.LookupConsultant();

            if (authCandidate.consultantID == 0)
            {
                if (timeZoneOffset == 2)
                { MessageBox.Show("Bruker Ikke Funnet"); }
                else
                { MessageBox.Show("User Not Found"); }
            }
            else if (authCandidate.consultantPass != ConsultantPassInput.Password)
            {
                if (timeZoneOffset == 2)
                { MessageBox.Show("Ugyldig Passord"); }
                else
                { MessageBox.Show("Invalid Password"); }
            }
            else
            {
                Log newLog = new Log(ConsultantNameInput.Text);
                newLog.LoginRecord();

                Dashboard consultantDashboard = new Dashboard(authCandidate);
                consultantDashboard.Show();

                this.Close();
            }
        }

        /*
        Button Methods
        */

        // Logs Consultant In If Authenticated
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] consultantFields =
            {
                ConsultantNameInput.Text,
                ConsultantPassInput.Password
            };

            Validator loginValidator = new Validator();
            
            if (loginValidator.CheckForNulls(consultantFields) == true)
            { AuthenticateConsultant(); }
            else
            {
                if (timeZoneOffset == 2)
                { MessageBox.Show("Ugyldig Innsending: Vennligst Fyll Ut Alle Felt"); }
                else
                { MessageBox.Show(loginValidator.formError); }
            }
        }

        // Register New Consultant
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            AddConsultantForm newConsultantForm = new AddConsultantForm();
            newConsultantForm.Show();
            
            this.Close();
        }

        // Exit Application
        private void CloserBtn_Click(object sender, RoutedEventArgs e)
        { Close(); }
    }
}
