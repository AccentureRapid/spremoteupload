using SharePointRestLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Cmdlets
{
    public class SPUploadSession
    {
        public SPUploadSession()
        {
        }

        public string LocalFolder { get; set; }

        public string DomainUserName { get; set; }

        public string DomainPassword { get; set; }

        public string BaseSharePointUrl { get; set; }

        public string LibraryTitle { get; set; }

        public string DBConnectionString { get; set; }

        public string SelectStatement { get; set; }

        public string FileNameField { get; set; }

        public bool OverwriteIfExists { get; set; }

        public string ContentType { get; set; }

        public Dictionary<string, string> ErroredFiles { get; set; }
        public List<string> SkippedFiles { get; set; }

        public SPColumnMappings Mappings { get; set; }
    }
}
