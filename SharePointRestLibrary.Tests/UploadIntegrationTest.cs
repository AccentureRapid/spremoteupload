using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointRestLibrary.Configuration;
using SharePointRestLibrary.Data;
using System.Collections.Generic;
using SharePointRestLibrary.Extensions;
using SharePointRestLibrary.SharePoint;
using SharePointRestLibrary.Cmdlets;

namespace SharePointRestLibrary.Tests
{
    [TestClass]
    public class SharePointUploadTests
    {
        private string _localFolder = "d:\\";
        private string _testUserId = "baaqmd\\roger.boone";
        private string _testPassword = "1.Greatb155";
        private string _testBaseUrl = "http://baaqmd-records.westus.cloudapp.azure.com/permitting/applications/";
        private string _testLibrary = "Permit Applications";
        private string _contentType = "Permit Application";
        private string _connection = @"Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;";
        private string _sql = "select * from vw_pas";
        
        [TestMethod]
        public void UploadIntegrationTest1()
        {
            //Create mappings from database to sharepoint fields
            //ISharePointUploader uploader = new SharePointUploader(_testUserId, _testPassword, _testBaseUrl);
            SPUploadSession session = new SPUploadSession()
            {
                BaseSharePointUrl=_testBaseUrl,
                ContentType=_contentType,
                DBConnectionString = _connection,
                DomainUserName = _testUserId,
                DomainPassword=_testPassword,
                ErroredFiles = new Dictionary<string,string>(),
                FileNameField = "file_name",
                LibraryTitle = _testLibrary,
                LocalFolder = _localFolder,
                SelectStatement =    _sql,
                SkippedFiles = new List<string>()
            };
                        
            var mappings = new SPColumnMappings("file_name");
            mappings.AddMapping("Application Title", "Title", "Text");
            mappings.AddMapping("Application Number", "Application Number", "Text");
            mappings.AddMapping("Application Title", "Application Title", "Text");
            mappings.AddMapping("Site Number", "Site Number", "Text");
            mappings.AddMapping("Plant Number", "Plant Number", "Numeric");
            mappings.AddMapping("Facility Name", "Facility Name", "Text");
            mappings.AddMapping("Engineer", "Engineer", "Text");
            mappings.AddMapping("PA Status", "PA Status", "Taxonomy");
            mappings.AddMapping("Status Date", "Status Date", "Date");

            //Create mappings from database to sharepoint fields
            ISharePointUploader uploader = new SharePointUploader(session.DomainUserName, session.DomainPassword, session.BaseSharePointUrl);


            //Get database data (collection of DBRecords)
            var sql = session.SelectStatement;
            var sm = new SQLManager(session.DBConnectionString);
            var records = sm.GetData(sql, session.FileNameField).ToSPDataRecords(mappings);

            //Uploader functionality
            foreach (SPDataRecord record in records)
            {
                try
                {
                    Console.WriteLine(string.Format("Uploading File {0} at {1}", record.FileName, DateTime.Now));
                    uploader.UploadFile(session.LocalFolder, record, session.LibraryTitle, session.ContentType);
                }
                catch (Exception ex)
                {
                    session.ErroredFiles.Add(record.FileName, ex.Message);
                    Console.WriteLine(string.Format("Could not upload {0} : Error {1}", record.FileName, ex.Message));
                }
            }    
        }
    }
}
