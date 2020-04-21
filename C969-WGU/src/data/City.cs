using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    public class City : Country
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");

        private int _cityID;
        private string _cityName;

        /*
        Getters and Setters
        */

        public int cityID
        {
            get { return _cityID; }
            set { _cityID = value; }
        }

        public string cityName
        {
            get { return _cityName; }
            set { _cityName = value; }
        }
        // Add New City
        public int AddCity(string templateName_city, int templateID_country, string creatorName)
        {
            string addCityQuery = $"INSERT INTO city (city, countryId, createDate, createdBy) VALUES('{ templateName_city }', { templateID_country }, utc_timestamp(), '{ creatorName }');";
            string getIDQuery = $"SELECT cityId FROM city WHERE city = '{ templateName_city }';";

            dbCon.Open();

            MySqlCommand addCityCommand = new MySqlCommand(addCityQuery, dbCon);
            addCityCommand.ExecuteNonQuery();

            MySqlCommand getIDCommand = new MySqlCommand(getIDQuery, dbCon);
            MySqlDataReader getIDReader = getIDCommand.ExecuteReader();

            if (getIDReader.Read())
            { _cityID = getIDReader.GetInt32(0); }

            dbCon.Close();

            return _cityID;
        }

        // Lookup Existing City
        public int LookupCity()
        {
            string lookupCityQuery = $"SELECT city, countryId FROM city WHERE cityId = { _cityID };";
            int cityCountry = 0;

            dbCon.Open();

            MySqlCommand lookupCityCommand = new MySqlCommand(lookupCityQuery, dbCon);
            MySqlDataReader lookupCityReader = lookupCityCommand.ExecuteReader();

            if (lookupCityReader.Read())
            {
                _cityName = lookupCityReader.GetString(0);
                cityCountry = lookupCityReader.GetInt32(1);
            }

            dbCon.Close();

            return cityCountry; 
        }
    }
}
