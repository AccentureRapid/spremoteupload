using SharePointRestLibrary.Configuration;
using SharePointRestLibrary.Data;
using SharePointRestLibrary.SharePoint;
using System;
using System.IO;
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


            var filenameList = Directory.EnumerateFiles(Session.LocalFolder).Select(p=>Path.GetFileName(p)).ToList<string>();
            bool recordFoundFlag = false;
            //Uploader functionality
            foreach (var filename in filenameList)
            {
                

                if(IsRecordAvailable(records, filename)) {
                    var record = records.Single(p => p.FileName.Equals(filename, StringComparison.InvariantCultureIgnoreCase));

                    //handle the uploading
                    Console.WriteLine(string.Format("Uploading File {0} at {1} - {2}", filename, DateTime.Now, "with Metadata"));
                    try
                    {
                        uploader.UploadFile(Session.LocalFolder, record, Session.LibraryTitle, Session.ContentType, Session.OverwriteIfExists);
                        MoveToUploaded(Session.LocalFolder, filename);
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex.Message, filename);
                    }

                }
                else 
                {
                    Session.ErroredFiles.Add(filename, "No Metadata found");
                    //Console.WriteLine(string.Format("Uploading File {0} at {1} - {2}", filename, DateTime.Now, "NO Metadata found."));
                    //try 
                    //{
                    //    uploader.UploadFile(Session.LocalFolder, filename, Session.LibraryTitle, Session.ContentType);
                    //} 
                    //catch (Exception ex2)
                    //{
                    //    HandleError(ex2.Message, filename);
                    //}
                }
            }

            WriteObject(Session);
            base.BeginProcessing();
        }

        private void MoveToUploaded(string sourcePath, string filename)
        {
            Console.WriteLine("Moving File to completed folder.");
            var targetFolder = sourcePath + @"completed\";
            if (!Directory.Exists(targetFolder))
                System.IO.Directory.CreateDirectory(targetFolder);

            File.Move(sourcePath + filename, targetFolder + filename);
        }
        

        private void HandleError(string message, string filename) {
            if (message.Contains("Skip"))
                Session.SkippedFiles.Add(filename);
            else
            {
                Session.ErroredFiles.Add(filename, message);
                Console.WriteLine(string.Format("Could not upload {0} : Error {1}", filename, message));
            }
        }

        private bool IsRecordAvailable(IEnumerable<SPDataRecord> records, string fileName) 
        {
            try 
            {
                var record = records.Single(p => p.FileName.Equals(fileName.Trim(), StringComparison.InvariantCultureIgnoreCase));
                return true;
            } 
            catch {
                Console.WriteLine(string.Format("No database record, Skipping {0}", fileName));
                return false;
            }

        }

        private bool IsUploadable()
        {
            return true;
        }
    }
}
