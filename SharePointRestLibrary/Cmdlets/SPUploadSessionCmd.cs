using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace SharePointRestLibrary.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "SPUploadSession")]
    public class SPUploadSessionCmd : PSCmdlet
    {
        //private string _localFolder = "c:\\testDocuments\\";
        //private string _testUserId = "baaqmd\\roger.boone";
        //private string _testPassword = "1.Greatb155";
        //private string _testBaseUrl = "http://records.westus.cloudapp.azure.com/";
        //private string _testLibrary = "General Ledger";
        //private string _connection = @"Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;";

        [Parameter(
            Mandatory=true,
            ValueFromPipeline=true,
            ValueFromPipelineByPropertyName=true,
            Position=0,
            HelpMessage="This is the folder where the source documents are stored")]
        [Alias("Source","Path")]
        public string LocalFolder { get; set; }
        
        [Parameter(
            Mandatory=true,
            ValueFromPipeline=true,
            ValueFromPipelineByPropertyName=true,
            Position=1,
            HelpMessage="This is the domain\\username that has access to the sharepoint site")]
        [Alias("UserName")]
        public string DomainUserName { get; set; }

        [Parameter(
            Mandatory=true,
            ValueFromPipeline=true,
            ValueFromPipelineByPropertyName=true,
            Position=2,
            HelpMessage="This is the users password (not a secure string yet!) that has access to the sharepoint site")]
        [Alias("Password")]
        public string DomainPassword { get; set; }
        
        
        [Parameter(
            Mandatory=true,
            ValueFromPipeline=true,
            ValueFromPipelineByPropertyName=true,
            Position=3,
            HelpMessage="This is the sharepoint web site ie http://mysite/web")]
        [Alias("Url")]
        public string BaseSharePointUrl { get; set; }

        [Parameter(
            Mandatory=true,
            ValueFromPipeline=true,
            ValueFromPipelineByPropertyName=true,
            Position=4,
            HelpMessage="This is the sharepoint Library title")]
        [Alias("Library")]
        public string LibraryTitle { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 4,
            HelpMessage = "This is the sharepoint contenttype title")]
        [Alias("SPContentType")]
        public string ContentType { get; set; }

        [Parameter(
            Mandatory=true,
            ValueFromPipeline=true,
            ValueFromPipelineByPropertyName=true,
            Position=5,
            HelpMessage="This is the fully credentialed connection string to a sql database.")]
        [Alias("ConnectionString")]
        public string DBConnectionString { get; set; }

        [Parameter(
            Mandatory=true,
            ValueFromPipeline=true,
            ValueFromPipelineByPropertyName=true,
            Position=6,
            HelpMessage="Full select statement for getting a list.")]
        [Alias("Sql", "Select")]
        public string SelectStatement { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 7,
            HelpMessage = "The field from the database that represents the filename.")]
        public string FileNameField { get; set; }

        protected override void BeginProcessing()
        {
            var session = new SPUploadSession()
            {
                BaseSharePointUrl = BaseSharePointUrl,
                DBConnectionString = DBConnectionString,
                DomainPassword = DomainPassword,
                DomainUserName = DomainUserName,
                LibraryTitle = LibraryTitle,
                LocalFolder = LocalFolder,
                SelectStatement = SelectStatement,
                FileNameField = FileNameField
            };
            WriteObject(session);
            base.BeginProcessing();
        }
    }
}
