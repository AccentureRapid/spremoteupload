using SharePointRestLibrary.Configuration;
using SharePointRestLibrary.Data;
using SharePointRestLibrary.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SharePointRestLibrary.Extensions;

namespace SharePointRestLibrary.Cmdlets
{
    [Cmdlet("Invoke","SPUpload")]
    public class SPInvokeSPUpload : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "This is the upload Session")]
        [Alias("UploadSession")]
        public SPUploadSession Session { get; set; }

        protected override void BeginProcessing()
        {
            //Create mappings from database to sharepoint fields
            ISharePointUploader uploader = new SharePointUploader(Session.DomainUserName, Session.DomainPassword, Session.BaseSharePointUrl);

            var mappings = Session.Mappings;

            Session.ErroredFiles = new Dictionary<string, string>();
            Session.SkippedFiles = new List<string>();

            //Get database data (collection of DBRecords)
            var sql = Session.SelectStatement;
            var sm = new SQLManager(Session.DBConnectionString);
            var records = sm.GetData(sql, Session.FileNameField).ToSPDataRecords(mappings);

            //Uploader functionality
            foreach (SPDataRecord record in records)
            {
                try
                {
                    Console.WriteLine(string.Format("Uploading File {0} at {1}", record.FileName, DateTime.Now));
                    uploader.UploadFile(Session.LocalFolder, record, Session.LibraryTitle, Session.ContentType);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Skip"))
                    {
                        Session.SkippedFiles.Add(record.FileName);
                    }
                    else
                    {
                        Session.ErroredFiles.Add(record.FileName, ex.Message);
                        Console.WriteLine(string.Format("Could not upload {0} : Error {1}", record.FileName, ex.Message));
                    }
                }
            }

            WriteObject(Session);
            base.BeginProcessing();
        }

        private bool IsUploadable()
        {
            return true;
        }
    }
}
