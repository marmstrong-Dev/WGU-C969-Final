using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace C969_Final
{
    public class Customer
    {
        MySqlConnection dbCon = new MySqlConnection("DB Info Here");

        private int _customerID;
        private string _customerName;
        private bool _isActive;

        /*
        Getters and Setters
        */

        public int customerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }

        public string customerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        public bool isActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        /*
        CRUD Methods
        */

        // Add New Customer
        public void AddCustomer(int addID, string creatorName)
        {
            string addCustomerQuery = $"INSERT INTO customer (customerName, addressId, active, createDate, createdBy) VALUES('{ _customerName }', { addID }, true, utc_timestamp(), '{ creatorName }');";

            dbCon.Open();

            MySqlCommand addCustomerCommand = new MySqlCommand(addCustomerQuery, dbCon);
            addCustomerCommand.ExecuteNonQuery();

            dbCon.Close();
        }

        // Edit Existing Customer
        public void EditCustomer(int editID, string editorName)
        {
            int activeCustomer = isActive ? 1 : 0;
            string editCustomerQuery = $"UPDATE customer " +
                                        $"SET customerName = '{ _customerName }', addressId = { editID }, active = { activeCustomer }, lastUpdate = utc_timestamp(), lastUpdateBy = '{ editorName }' " +
                                        $"WHERE customerId = { _customerID };";

            dbCon.Open();

            MySqlCommand editCustomerCommand = new MySqlCommand(editCustomerQuery, dbCon);
            editCustomerCommand.ExecuteNonQuery();

            dbCon.Close();
        }

        // Delete Customer from DB
        public void DeleteCustomer()
        {
            string deleteCustomerQuery = $"DELETE FROM customer WHERE customerId = { _customerID }";

            dbCon.Open();

            MySqlCommand deleteCustomerCommand = new MySqlCommand(deleteCustomerQuery, dbCon);
            deleteCustomerCommand.ExecuteNonQuery();

            dbCon.Close();
        }

        // Lookup Existing Customer
        public int LookupCustomer()
        {
            string lookupCustomerQuery = $"SELECT customerName, active, addressId FROM customer WHERE customerId = { _customerID };";
            int customerAddress = 0;

            dbCon.Open();

            MySqlCommand lookupCustomerCommand = new MySqlCommand(lookupCustomerQuery, dbCon);
            MySqlDataReader lookupCustomerReader = lookupCustomerCommand.ExecuteReader();

            if (lookupCustomerReader.Read())
            {
                _customerName = lookupCustomerReader.GetString(0);
                _isActive = lookupCustomerReader.GetBoolean(1);
                customerAddress = lookupCustomerReader.GetInt32(2);
            }

            dbCon.Close();

            return customerAddress;
        }
    }
}
