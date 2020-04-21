using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace C969_Final
{
    public class Log
    {
        private string loginName;
        private DateTime authTime = DateTime.Now.ToUniversalTime();
        private DateTime modTime = DateTime.Now.ToUniversalTime();

        /*
        Directory Locations
        */

        private string getAuthDir = @"..\..\src\Logs\AuthLog.txt";
        private string getModDir = @"..\..\src\Logs\ModLog.txt";

        // Constructor
        public Log(string loggedIn)
        { loginName = loggedIn; }

        /*
        Auth Log Methods
        */

        // Creates TimeStamp For Consultant Login
        public void LoginRecord()
        {
            string[] loginMessage =
            {
                "Consultant Logged In",
                $"UserName: { loginName }",
                $"Login Time: { authTime } UTC",
                " ",
                "-------------------------------------------"
            };

            File.AppendAllLines(getAuthDir, loginMessage);
        }

        // Creates TimeStamp For Consultant Logout
        public void LogoutRecord()
        {
            string[] logoutMessage =
            {
                "Consultant Logged Out",
                $"UserName: { loginName }",
                $"Logout Time: { authTime } UTC",
                " ",
                "-------------------------------------------"
            };

            File.AppendAllLines(getAuthDir, logoutMessage);
        }

        /*
        Record Mod Logs
        */

        // Creates TimeStamp For Adding Record
        public void AddRecord(string recordType, string recordName)
        {
            string[] recordAddedMessage =
            {
                $"{ recordType } Added To DB",
                $"{ recordType } Name: { recordName }",
                $"Added On: { modTime } UTC",
                $"Added By: { loginName }",
                " ",
                "-------------------------------------------"
            };

            File.AppendAllLines(getModDir, recordAddedMessage);
        }

        // Creates TimeStamp For Editing Record
        public void EditRecord(string recordType_edit, string recordName_edit)
        {
            string[] recordAddedMessage =
            {
                $"{ recordType_edit } Edited In DB",
                $"{ recordType_edit } Name: { recordName_edit }",
                $"Modified On: { modTime } UTC",
                $"Modified By: { loginName }",
                " ",
                "-------------------------------------------"
            };

            File.AppendAllLines(getModDir, recordAddedMessage);
        }
    }
}
