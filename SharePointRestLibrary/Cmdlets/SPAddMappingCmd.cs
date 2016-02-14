using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using SharePointRestLibrary.Configuration;

namespace SharePointRestLibrary.Cmdlets
{
    [Cmdlet(VerbsCommon.Add, "SPMapping")]
    public class SPAddMappingCmd : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "This is the upload Session")]
        [Alias("UploadSession")]
        public SPUploadSession Session { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            HelpMessage = "This is the name of the source field from the database.")]
        public string DBSourceField { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 2,
            HelpMessage = "This is the destination field in Sharepoint.  Use internal sharepoint field name.")]
        public string SPDestinationField { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 3,
            HelpMessage = "This is the sharepoint data type.  Text, Numeric, Date")]
        public string SPDataType { get; set; }

        protected override void BeginProcessing()
        {
            if (Session.Mappings == null || Session.Mappings.Count == 0)
                Session.Mappings = new SPColumnMappings(Session.FileNameField);

            Session.Mappings.Add(new SPColumnMapping()
            {
                DBSourceField = DBSourceField,
                SharePointDataType = SPDataType,
                SharePointDestinationField = SPDestinationField
            });

            WriteObject(Session);
            base.BeginProcessing();
        }
    }
}
