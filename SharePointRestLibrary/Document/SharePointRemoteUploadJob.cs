using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Document
{
    public class SharePointRemoteUploadJob : IUploadJobProvider
    {
        private JobSpec Spec { get; set; }
        
       
        public SharePointRemoteUploadJob(JobSpec spec)
        {
            Spec = spec;
        }


        public void BeginUpload()
        {
            throw new NotImplementedException();
        }

        public void CancelUpload()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> GetDefaultMappingFromDatabase()
        {
            throw new NotImplementedException();
        }

        public bool CheckSharePointConnection()
        {
            throw new NotImplementedException();
        }

        public bool CheckDatabaseConnection()
        {
            throw new NotImplementedException();
        }

        public bool ValidateMapping(IDictionary<string, string> mapping)
        {
            throw new NotImplementedException();
        }
    }
}
