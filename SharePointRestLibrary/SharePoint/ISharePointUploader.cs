using SharePointRestLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.SharePoint
{
    public interface ISharePointUploader
    {
        IEnumerable<string> GetFilesByLocation(string libraryTitle);
        int GetFileCountByLocation(string libraryTitle);
        bool SPFileExistInLibrary(string libraryTitle, string fileName, bool refreshCache=false);
        IEnumerable<string> GetInternalFieldNames(string libraryTitle, string fileIdResource);
        void UploadFile(string sourceFolder, SPDataRecord record, string libraryTitle, string contentType);
    }
}
