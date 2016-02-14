using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointRestLibrary.Configuration;
using SharePointRestLibrary.Data;
using System.Collections.Generic;
using SharePointRestLibrary.Extensions;
using SharePointRestLibrary.SharePoint;

namespace SharePointRestLibrary.Tests
{
    [TestClass]
    public class SharePointUploadTests
    {
        private string _localFolder = "c:\\testDocuments\\";
        private string _testUserId = "baaqmd\\roger.boone";
        private string _testPassword = "1.Greatb155";
        private string _testBaseUrl = "http://records.westus.cloudapp.azure.com/";
        private string _testLibrary = "General Ledger";
        private string _connection = @"Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;";

        [TestMethod]
        public void UploadIntegrationTest1()
        {
            //Create mappings from database to sharepoint fields
            ISharePointUploader uploader = new SharePointUploader(_testUserId, _testPassword, _testBaseUrl);
            
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
                uploader.UploadFile(_localFolder, record, _testLibrary, string.Empty);
            }           
        }
    }
}
