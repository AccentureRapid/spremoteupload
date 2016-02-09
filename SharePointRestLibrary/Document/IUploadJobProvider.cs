using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Document
{
    public interface IUploadJobProvider
    {
        void BeginUpload();
        void CancelUpload();
        IDictionary<string, string> GetDefaultMappingFromDatabase();
        bool CheckSharePointConnection();
        bool CheckDatabaseConnection();
        bool ValidateMapping(IDictionary<string, string> mapping);
    }
}
