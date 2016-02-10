using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SharePointRestLibrary.Extensions;

namespace SharePointRestLibrary.SharePoint
{
    public class SharePointFactFetcher : ISharePointFactFetcher
    {
        private string _username;
        private string _password;
        private string _baseUrl;
        private IDictionary<string, IEnumerable<string>> _filesByLibraryCache;

        public SharePointFactFetcher(string username, string password, string baseUrl)
        {
            _username = username;
            _password = password;
            _baseUrl = baseUrl;
        }

        public bool SPFileExistInLibrary(string libraryTitle, string fileName, bool refreshCache = false)
        {
            if (!_filesByLibraryCache.ContainsKey(libraryTitle) || refreshCache)
                GetFileCountByLocation(libraryTitle);

            return _filesByLibraryCache[libraryTitle].Contains(fileName, StringComparer.InvariantCultureIgnoreCase);
        }

        public double GetFileCountByLocation(string libraryID)
        {
            var resource = string.Format("/_api/web/lists/getByTitle('{0}')/itemCount", libraryID);
            var response = this.ExecuteGet(resource, _baseUrl,_username, _password);

            var wrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            var value = JsonConvert.DeserializeObject<Dictionary<string, double>>(wrapper.Values.First().ToString());

            return value["ItemCount"];
        }

        public IEnumerable<string> GetFilesByLocation(string libraryTitle)
        {
            var resource = string.Format("/_api/web/Lists/GetByTitle('{0}')/Items?$expand=Folder&$select=FileLeafRef", libraryTitle);
            var response = this.ExecuteGet(resource, _baseUrl, _username, _password);

            var wrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(wrapper["d"].ToString());
            var results = JsonConvert.DeserializeObject<List<SPNameValue>>(d["results"].ToString());

            var filesOut = results.Select(p => p.FileLeafRef);
            if(_filesByLibraryCache == null)
                _filesByLibraryCache = new Dictionary<string, IEnumerable<string>>();

            _filesByLibraryCache.Add(libraryTitle, filesOut);

            return filesOut;
        }

        public IEnumerable<string> GetInternalFieldNames(string libraryTitle, string fileIdResource)
        {
            var resource = string.Format("/_api/{0}", fileIdResource);
            var response = this.ExecuteGet(resource, _baseUrl, _username, _password);

            var wrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(wrapper["d"].ToString());

            return new List<string>();
        }
    }

    public class SPNameValue
    {
        public string id {get; set;}
        public string FileLeafRef { get; set; }
    }
}
