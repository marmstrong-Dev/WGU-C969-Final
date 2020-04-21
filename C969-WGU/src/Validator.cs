using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Net;
using System.Windows.Navigation;

namespace C969_Final
{
    public class Validator
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");

        private string _formError;
        private string _nameResult;
        private int _idResult = 0;
        private bool _isValid = true;

        // Getters
        public string formError
        { get { return _formError; } }

        public string nameResult
        { get { return _nameResult; } }

        public int idResult
        { get { return _idResult; } }

        // Checks Form For Null Fields
        public bool CheckForNulls(string[] contextStrings)
        {
            for (int i = 0; i < contextStrings.Length; i++)
            {
                if (contextStrings[i] == "")
                { _isValid = false; }
            }
            
            if (_isValid == false)
            { _formError = "Invalid Submission: Please Fill All Fields"; }

            return _isValid;
        }

        // Check Phone and Postal
        public bool CheckNums(string phoneNum, string zipCode)
        {
            _isValid = true;

            if (phoneNum.Length != 10)
            { 
                _isValid = false;
                _formError = "Invalid Phone Number";
            }

            if (zipCode.Length != 5) 
            { 
                _isValid = false;
                _formError = "Invalid Postal Code";
            }

            try
            { int castedPostal = Int32.Parse(zipCode); }
            catch 
            { 
                _isValid = false;
                _formError = "Invalid Postal Code";
            }

            return _isValid;
        }

        // Checks if Address Components Exist in DB
        public bool CheckIfExists(string lookupName, string lookupField, string lookupTable)
        {
            string lookupQuery = $"SELECT * FROM { lookupTable } WHERE { lookupField } = '{ lookupName }';";

            dbCon.Open();

            MySqlCommand lookupCommand = new MySqlCommand(lookupQuery, dbCon);
            MySqlDataReader lookupReader = lookupCommand.ExecuteReader();

            if (lookupReader.Read())
            {
                _idResult = lookupReader.GetInt32(0);
                _nameResult = lookupReader.GetString(1);

                _isValid = true;
            }
            else
            { _isValid = false; }

            dbCon.Close();

            return _isValid;
        }

        // Check if Appointments Overlap
        public bool CheckForAppointmentConflicts(DateTime begin, DateTime end, int consID)
        {
            begin.ToUniversalTime();
            end.ToUniversalTime();

            string formattedStart = begin.ToString("yyyy-MM-dd H:mm:ss");
            string formattedEnd = end.ToString("yyyy-MM-dd H:mm:ss");
            int timeCounter = 0;

            string timeQuery = $"SELECT appointmentId FROM appointment " +
                                $"WHERE ('{ formattedStart }' BETWEEN start AND end) " +
                                $"OR ('{ formattedEnd }' BETWEEN start AND end) " +
                                $"OR ((start > '{ formattedStart }') AND ('{ formattedEnd }' > end))" +
                                $"AND userId = { consID };";

            dbCon.Open();

            MySqlCommand timeCommand = new MySqlCommand(timeQuery, dbCon);
            MySqlDataReader timeReader = timeCommand.ExecuteReader();

            if (timeReader.Read())
            { 
                timeCounter++;
                _idResult = timeReader.GetInt32(0);
            }

            dbCon.Close();

            if (begin.ToLocalTime().Hour < 8 || end.ToLocalTime().Hour > 17) 
            { _isValid = false; _formError = "Error: Scheduled Outside of Business Hours"; }
            else if (timeCounter != 0)
            { _isValid = false; _formError = "Error: Scheduling Conflict - Choose Another Time"; }
            else if (begin > end) 
            { _isValid = false; _formError = "Error: Begin Time Cannot Occur After End Time"; }
            else
            { _isValid = true; }

            return _isValid;
        }

        /*
        DashBoard Validations
        */

        // Checks if Multiple Fields Are Selected
        public bool CheckFieldQty(DataTable candidateTable)
        {
            int selectionCounter = 0;
            
            for (int i = 0; i < candidateTable.Rows.Count; i++)
            {
                if ((bool)candidateTable.Rows[i].ItemArray[0] == true)
                {
                    selectionCounter++;
                    _idResult = (int)candidateTable.Rows[i].ItemArray[1];
                }
            }

            if (selectionCounter != 1)
            { _isValid = false; }

            Console.WriteLine(selectionCounter);
            return _isValid;
        }

        // Checks if Customer Has Appointments
        public bool CheckForAppointments(int deleteCandidate)
        {
            string checkAppointmentsQuery = $"SELECT customerId FROM appointment WHERE customerId = { deleteCandidate };";
            int appointmentCounter = 0;
            _isValid = true;

            dbCon.Open();

            MySqlCommand checkAppointmentsCommand = new MySqlCommand(checkAppointmentsQuery, dbCon);
            MySqlDataReader checkAppointmentsReader = checkAppointmentsCommand.ExecuteReader();

            while (checkAppointmentsReader.Read())
            { appointmentCounter++; }

            dbCon.Close();
           
            if (appointmentCounter > 0) 
            { _isValid = false; }

            return _isValid;
        }
    }
}
