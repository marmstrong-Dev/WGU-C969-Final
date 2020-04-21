using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    public class Address : City
    {
        MySqlConnection dbCon = new MySqlConnection("server=3.227.166.251;userid=U07042;database=U07042;password=53688926466;port=3306");

        private int _addressID;
        private string _addressStreet;
        private string _addressApt;
        private string _addressPostal;
        private string _phoneNumber;

        /*
        Getters and Setters
        */

        public int addressID
        {
            get { return _addressID; }
            set { _addressID = value; }
        }

        public string addressStreet
        {
            get { return _addressStreet; }
            set { _addressStreet = value; }
        }

        public string addressApt
        {
            get { return _addressApt; }
            set { _addressApt = value; }
        }

        public string addressPostal
        {
            get { return _addressPostal; }
            set { _addressPostal = value; }
        }

        public string phoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        // Add New Address
        public int AddAddress(string creatorName)
        {
            string addAddressQuery = $"INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy) VALUES('{ _addressStreet }', '{ _addressApt }', { this.cityID }, '{ _addressPostal }', { _phoneNumber }, utc_timestamp(), '{ creatorName }');";
            string getIDQuery = $"SELECT addressId FROM address WHERE address = '{ _addressStreet }';";

            dbCon.Open();

            MySqlCommand addAddressCommand = new MySqlCommand(addAddressQuery, dbCon);
            addAddressCommand.ExecuteNonQuery();

            MySqlCommand getIDCommand = new MySqlCommand(getIDQuery, dbCon);
            MySqlDataReader getIDReader = getIDCommand.ExecuteReader();

            if (getIDReader.Read())
            { _addressID = getIDReader.GetInt32(0); }

            dbCon.Close();

            return _addressID;
        }

        // Update Existing Address
        public int EditAddress(string editorName)
        {
            string editAddressQuery = $"UPDATE address " +
                                        $"SET address = '{ _addressStreet }', address2 = '{ _addressApt }', cityId = { this.cityID }, postalCode = '{ _addressPostal }', phone = '{ _phoneNumber }', " +
                                        $"lastUpdate = utc_timestamp(), lastUpdateBy = '{ editorName }' " +
                                        $"WHERE addressId = { _addressID };";

            dbCon.Open();

            MySqlCommand editAddressCommand = new MySqlCommand(editAddressQuery, dbCon);
            editAddressCommand.ExecuteNonQuery();

            dbCon.Close();

            return _addressID;
        }

        // Lookup Address
        public int LookupAddress()
        {
            string lookupAddressQuery = $"SELECT address, address2, cityId, postalCode, phone FROM address WHERE addressId = { _addressID };";
            int addressCity_lookup = 0;

            dbCon.Open();

            MySqlCommand lookupAddressCommand = new MySqlCommand(lookupAddressQuery, dbCon);
            MySqlDataReader lookupAddressReader = lookupAddressCommand.ExecuteReader();

            if (lookupAddressReader.Read())
            {
                _addressStreet = lookupAddressReader.GetString(0);
                _addressApt = lookupAddressReader.GetString(1);
                addressCity_lookup = lookupAddressReader.GetInt32(2);
                _addressPostal = lookupAddressReader.GetString(3);
                _phoneNumber = lookupAddressReader.GetString(4);
            }

            dbCon.Close();

            return addressCity_lookup;
        }
    }
}
