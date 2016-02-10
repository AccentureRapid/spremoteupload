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

namespace SharePointRestLibrary.SharePoint
{
    public class SharePointFactFetcher : ISharePointFactFetcher
    {
        private string _username;
        private string _password;
        private string _baseUrl;

        public SharePointFactFetcher(string username, string password, string baseUrl)
        {
            _username = username;
            _password = password;
            _baseUrl = baseUrl;
        }


        public double GetFileCountByLocation(string libraryID)
        {
            var resource = string.Format("/_api/web/lists/getByTitle('{0}')/itemCount", libraryID);
            var response = ExecuteGet(resource);

            var wrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            var value = JsonConvert.DeserializeObject<Dictionary<string, double>>(wrapper.Values.First().ToString());

            return value["ItemCount"];
        }

        public IEnumerable<string> GetFilesByLocation(string libraryTitle)
        {
            var resource = string.Format("/_api/web/Lists/GetByTitle('{0}')/Items?$expand=Folder&$select=FileLeafRef", libraryTitle);
            var response = ExecuteGet(resource);

            var wrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(wrapper["d"].ToString());
            var results = JsonConvert.DeserializeObject<List<SPNameValue>>(d["results"].ToString());

            var filesOut = results.Select(p => p.FileLeafRef);

            //foreach (var item in results.Values)
            //{
            //    var value = JsonConvert.DeserializeObject<Dictionary<string, string>>(item);
            //    filesOut.Add(value["Title"]);
            //}

            return filesOut;
        }

        private string ExecuteGet(string resource)
        {
            var url = _baseUrl + resource;
            var endpointRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            endpointRequest.Method = "GET";
            endpointRequest.Accept = "application/json;odata=verbose";
            var cred = new System.Net.NetworkCredential(_username, _password);
            endpointRequest.Credentials = cred;
            var endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();

            var webResponse = endpointRequest.GetResponse();
            var webStream = webResponse.GetResponseStream();
            var responseReader = new StreamReader(webStream);
            var response = responseReader.ReadToEnd();

            return response;
        }

    }

    public class SPNameValue
    {
        public string FileLeafRef { get; set; }
    }
}
