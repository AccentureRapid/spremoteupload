using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointRestLibrary.Configuration;
using SharePointRestLibrary.Data;
using System.Collections.Generic;
using SharePointRestLibrary.Extensions;

namespace SharePointRestLibrary.Tests
{
    [TestClass]
    public class SharePointUploadTests
    {
        private string _testUserId = "baaqmd-dev\\sp_farm";
        private string _testPassword = "P@$$word1!";
        private string _testBaseUrl = "http://baaqmd-dev.cloudapp.net/sites/RecordCenter";
        private string _testLibrary = "General Ledger";
        private string _connection = @"Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;";

        [TestMethod]
        public void UploadIntegrationTest1()
        {
            //Create mappings from database to sharepoint fields
            var mappings = new SPColumnMappings("file_name");
            mappings.AddMapping("Title", "Title", "Text");
            mappings.AddMapping("Book Number", "Book_x0020_Number", "Number");
            mappings.AddMapping("Retention Date", "Retention_x0020_Date", "Date");

            //Get database data (collection of DBRecords)
            var sql = "Select * from GeneralLedger";
            var sm = new SQLManager(_connection);
            var records = sm.GetData(sql, "file_name").ToSPDataRecords(mappings);

            //Uploader functionality
            foreach (SPDataRecord record in records)
            {
                record.Upload(@"c:\testDocuments", _testBaseUrl, _testLibrary, _testUserId, _testPassword);
            }           
        }
    }
}
