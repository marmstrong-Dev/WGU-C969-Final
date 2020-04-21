using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Timers;
using System.Threading;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    public class Consultant
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");

        private int _consultantID;
        private string _consultantName;
        private string _consultantPass;
        private bool _activeConsultant;
        private TimeSpan _geoCode;

        /*
        Getters and Setters
        */

        public int consultantID 
        {
            get { return _consultantID; }
            set { _consultantID = value; }
        }

        public string consultantName
        {
            get { return _consultantName; }
            set { _consultantName = value; }
        }

        public string consultantPass
        {
            get { return _consultantPass; }
            set { _consultantPass = value; }
        }

        public bool activeConsultant
        {
            get { return _activeConsultant; }
            set { _activeConsultant = value; }
        }

        public TimeSpan geoCode
        {
            get { return _geoCode; }
            set { _geoCode = value; }
        }

        // Add New Consultant
        public void AddConsultant()
        {
            string addConsultantQuery = $"INSERT INTO user (userName, password, active, createDate) VALUES ('{ consultantName }', '{ consultantPass }', true, utc_timestamp());";

            dbCon.Open();

            MySqlCommand addConsultantCommand = new MySqlCommand(addConsultantQuery, dbCon);
            addConsultantCommand.ExecuteNonQuery();

            dbCon.Close();
        }

        // Lookup Existing Consultant
        public int LookupConsultant()
        {
            string lookupConsultantQuery = $"SELECT userId, password, active FROM user WHERE userName = '{ _consultantName }';";

            dbCon.Open();

            MySqlCommand lookupConsultantCommand = new MySqlCommand(lookupConsultantQuery, dbCon);
            MySqlDataReader lookupConsultantReader = lookupConsultantCommand.ExecuteReader();

            if (lookupConsultantReader.Read())
            {
                _consultantID = lookupConsultantReader.GetInt32(0);
                _consultantPass = lookupConsultantReader.GetString(1);

                if (lookupConsultantReader.GetInt16(2) == 0)
                { _activeConsultant = false; }
                else
                { _activeConsultant = true; }
            }
            else
            { _consultantID = 0; }

            dbCon.Close();

            return _consultantID;
        }

        // Check For Associated Appointments Starting in Less Than 15 Minutes
        public bool CheckAppointments()
        {
            bool hasUpcoming = false;

            string checkAppointmentsQuery = $"SELECT TIMESTAMPDIFF (MINUTE, utc_timestamp, start) FROM appointment WHERE userId = { _consultantID } AND start > utc_timestamp";

            dbCon.Open();

            MySqlCommand checkAppointmentsCommand = new MySqlCommand(checkAppointmentsQuery, dbCon);
            MySqlDataReader checkAppointmentsReader = checkAppointmentsCommand.ExecuteReader();

            while (checkAppointmentsReader.Read())
            {
                if (checkAppointmentsReader.GetInt32(0) < 15)
                { hasUpcoming = true; }
            }

            dbCon.Close();

            return hasUpcoming;
        }
    }
}
