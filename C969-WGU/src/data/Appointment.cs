using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    public class Appointment
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");

        private int _appointmentID;
        private string _appointmentName;
        private string _appointmentType;
        private string _appointmentContact;
        private string _appointmentLocation;
        private string _appointmentDescription;
        private string _appointmentURL;
        private DateTime _startTime;
        private DateTime _endTime;

        /*
        Getters and Setters
        */

        public int appointmentID
        {
            get { return _appointmentID; }
            set { _appointmentID = value; }
        }

        public string appointmentName
        {
            get { return _appointmentName; }
            set { _appointmentName = value; }
        }

        public string appointmentType
        {
            get { return _appointmentType; }
            set { _appointmentType = value; }
        }

        public string appointmentContact
        {
            get { return _appointmentContact; }
            set { _appointmentContact = value; }
        }

        public string appointmentLocation
        {
            get { return _appointmentLocation; }
            set { _appointmentLocation = value; }
        }

        public string appointmentDescription
        {
            get { return _appointmentDescription; }
            set { _appointmentDescription = value; }
        }

        public string appointmentURL
        {
            get { return _appointmentURL; }
            set { _appointmentURL = value; }
        }

        public DateTime startTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public DateTime endTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        /*
        CRUD Methods
        */

        // Add New Appointment
        public void AddAppointment(int custID, int userID, string creatorName)
        {
            _startTime = _startTime.ToUniversalTime();
            _endTime = _endTime.ToUniversalTime();
            string formattedStart = _startTime.ToString("yyyy-MM-dd H:mm:ss");
            string formattedEnd = _endTime.ToString("yyyy-MM-dd H:mm:ss");
            
            string addAppointmentQuery = $"INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy) " +
                                         $"VALUES ({ custID }, { userID }, '{ _appointmentName }', '{ _appointmentDescription }', '{ _appointmentLocation }', '{ _appointmentContact }', '{ _appointmentType }', '{ _appointmentURL }', '{ formattedStart }', '{ formattedEnd }', utc_timestamp(), '{ creatorName }');";
            
            dbCon.Open();

            MySqlCommand addAppointmentCommand = new MySqlCommand(addAppointmentQuery, dbCon);
            addAppointmentCommand.ExecuteNonQuery();

            dbCon.Close();
        }

        // Edit Existing Appointment
        public void EditAppointment(int updatedCust, int updatedUser, string editorName)
        {
            _startTime = _startTime.ToUniversalTime();
            _endTime = _endTime.ToUniversalTime();
            string updatedStart = _startTime.ToString("yyyy-MM-dd H:mm:ss");
            string updatedEnd = _endTime.ToString("yyyy-MM-dd H:mm:ss");
            
            string editAppointmentQuery = $"UPDATE appointment " +
                                          $"SET customerId = { updatedCust }, userId = { updatedUser }, title = '{ _appointmentName }', description = '{ _appointmentDescription }', location = '{ _appointmentLocation }', contact = '{ _appointmentContact }' , type = '{ _appointmentType }', url = '{ _appointmentURL }', start = '{ updatedStart }', end = '{ updatedEnd }', " +
                                          $"lastUpdate = utc_timestamp(), lastUpdateBy = '{ editorName }' " +
                                          $"WHERE appointmentId = { _appointmentID };";

            dbCon.Open();

            MySqlCommand editAppointmentCommand = new MySqlCommand(editAppointmentQuery, dbCon);
            editAppointmentCommand.ExecuteNonQuery();

            dbCon.Close();
        }

        // Delete Appointment From DB
        public void DeleteAppointment()
        {
            string deleteAppointmentQuery = $"DELETE FROM appointment WHERE appointmentId = { _appointmentID }";

            dbCon.Open();

            MySqlCommand deleteAppointmentCommand = new MySqlCommand(deleteAppointmentQuery, dbCon);
            deleteAppointmentCommand.ExecuteNonQuery();

            dbCon.Close();
        }

        // Lookup Existing Appointment
        public int LookupAppointment()
        {
            int custID = 0;

            string lookupAppointmentQuery = $"SELECT customerId, title, description, location, contact, type, url, start, end FROM appointment WHERE appointmentId = { _appointmentID };";

            dbCon.Open();

            MySqlCommand lookupAppointmentCommand = new MySqlCommand(lookupAppointmentQuery, dbCon);
            MySqlDataReader lookupAppointmentReader = lookupAppointmentCommand.ExecuteReader();

            if (lookupAppointmentReader.Read())
            {
                custID = lookupAppointmentReader.GetInt32(0);
                _appointmentName = lookupAppointmentReader.GetString(1);
                _appointmentDescription = lookupAppointmentReader.GetString(2);
                _appointmentLocation = lookupAppointmentReader.GetString(3);
                _appointmentContact = lookupAppointmentReader.GetString(4);
                _appointmentType = lookupAppointmentReader.GetString(5);
                _appointmentURL = lookupAppointmentReader.GetString(6);

                _startTime = lookupAppointmentReader.GetDateTime(7).ToLocalTime();
                _endTime = lookupAppointmentReader.GetDateTime(8).ToLocalTime();
            }

            dbCon.Close();

            return custID;
        }
    }
}
