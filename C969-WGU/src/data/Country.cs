using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    public class Country
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");

        private int _countryID;
        private string _countryName;

        /*
        Getters and Setters
        */

        public int countryID
        {
            get { return _countryID; }
            set { _countryID = value; }
        }

        public string countryName
        {
            get { return _countryName; }
            set { _countryName = value; }
        }

        // Add New Country
        public int AddCountry(string templateName_country, string creatorName)
        {
            string addCountryQuery = $"INSERT INTO country (country, createDate, createdBy) VALUES('{ templateName_country }', utc_timestamp(), '{ creatorName }');";
            string getIDQuery = $"SELECT countryId FROM country WHERE country = '{ templateName_country }';";

            dbCon.Open();

            MySqlCommand addCountryCommand = new MySqlCommand(addCountryQuery, dbCon);
            addCountryCommand.ExecuteNonQuery();

            MySqlCommand getIDCommand = new MySqlCommand(getIDQuery, dbCon);
            MySqlDataReader getIDReader = getIDCommand.ExecuteReader();

            if (getIDReader.Read())
            { _countryID = getIDReader.GetInt32(0); }

            dbCon.Close();

            return _countryID;
        }

        // Lookup Country
        public void LookupCountry()
        {
            string countryLookupQuery = $"SELECT country FROM country WHERE countryId = { _countryID };";

            dbCon.Open();

            MySqlCommand countryLookupCommand = new MySqlCommand(countryLookupQuery, dbCon);
            MySqlDataReader countryLookupReader = countryLookupCommand.ExecuteReader();

            if (countryLookupReader.Read())
            { _countryName = countryLookupReader.GetString(0); }

            dbCon.Close();
        }
    }
}
