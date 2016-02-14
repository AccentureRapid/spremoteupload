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
using Microsoft.SharePoint.Client;
using SharePointRestLibrary.Configuration;

namespace SharePointRestLibrary.SharePoint
{
    public class SharePointUploader : ISharePointUploader
    {
        private string _username;
        private string _password;
        private ICredentials _credentials;
        private string _baseUrl;
        private IDictionary<string, IEnumerable<string>> _filesByLibraryCache;
        private ClientContext ctx;

        public SharePointUploader(string username, string password, string baseUrl)
        {
            _username = username;
            _password = password;
            _credentials = new NetworkCredential(_username, _password);
            _baseUrl = baseUrl;
            ctx = new ClientContext(_baseUrl);
        }

        public bool SPFileExistInLibrary(string libraryTitle, string fileName, bool refreshCache = false)
        {
            if (!_filesByLibraryCache.ContainsKey(libraryTitle) || refreshCache)
                GetFileCountByLocation(libraryTitle);

            return _filesByLibraryCache[libraryTitle].Contains(fileName, StringComparer.InvariantCultureIgnoreCase);
        }

        public int GetFileCountByLocation(string libraryID)
        {
            var itemCount = 0;

            Web web = ctx.Web;
            List list = web.Lists.GetByTitle(libraryID);
            ctx.Load(list);
            ctx.ExecuteQuery();

            itemCount = list.ItemCount;
            
            return itemCount;
        }

        public IEnumerable<string> GetFilesByLocation(string libraryTitle)
        {
            var listOut = new List<string>();

            ctx.Credentials = _credentials;
            var web = ctx.Web;
            var list = web.Lists.GetByTitle(libraryTitle);
            ctx.Load(list);
            ctx.Load(list.RootFolder);
            ctx.Load(list.RootFolder.Files);
            ctx.ExecuteQuery();

            foreach (var item in list.RootFolder.Files)
            {
                listOut.Add(item.Name);
            }             

            return listOut;
        }

        public void UploadFile(string sourceFolder, SPDataRecord record, string libraryTitle, string contentType)
        {
            string filepath = sourceFolder + record.FileName;
            ContentType ct = GetContentType(ctx, libraryTitle, contentType);

            var info = new FileInfo(filepath);
            var filesize = info.Length;
            if (filesize < 20000000)
            {
                var fileData = System.IO.File.ReadAllBytes(sourceFolder + record.FileName);
                ctx.Credentials = _credentials;
                var fi = new FileCreationInformation();
                var list = ctx.Web.Lists.GetByTitle(libraryTitle);
                ctx.Load(list.RootFolder);

                var fileUrl = record.FileName;
                fi.Overwrite = false;
                fi.Url = fileUrl;
                fi.Content = fileData;
                var uploadedFile = list.RootFolder.Files.Add(fi);
                var listItem = uploadedFile.ListItemAllFields;

                foreach (var spfield in record)
                {
                    listItem[spfield.SPColumnInternalName] = spfield.SPValue;
                }

                listItem.Update();
                ctx.Load(uploadedFile);
                ctx.ExecuteQuery();
            }
            else
            {
                ctx.Credentials = _credentials;
                
                var list = ctx.Web.Lists.GetByTitle(libraryTitle);
                ctx.Load(list.RootFolder);
                ctx.ExecuteQuery();
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(
                    ctx, list.RootFolder.ServerRelativeUrl, new FileStream(filepath, FileMode.Open, FileAccess.Read), false);

                var uploadedFile = list.RootFolder.Files.GetByUrl(list.RootFolder.ServerRelativeUrl + record.FileName);
                var listItem = uploadedFile.ListItemAllFields;
                listItem["ContentTypeId"] = ct.Id.ToString();

                foreach (var spfield in record)
                {
                    listItem[spfield.SPColumnInternalName] = spfield.SPValue;
                }

                listItem.Update();
                ctx.Load(uploadedFile);
                ctx.ExecuteQuery();
            }
        }
        /// <summary>
        /// Gets the Content Type Object of the given content type string
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="docs"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private ContentType GetContentType(ClientContext ctx,string libraryName, string contentType)
        {
            var docs = ctx.Web.Lists.GetByTitle(libraryName);
            ContentTypeCollection listContentTypes = docs.ContentTypes;
            ctx.Load(listContentTypes, types => types.Include
                     (type => type.Id, type => type.Name,
                       type => type.Parent));
            var result = ctx.LoadQuery(listContentTypes.Where(c => c.Name == contentType));
            ctx.ExecuteQuery();
            ContentType targetDocumentSetContentType = result.FirstOrDefault();
            return targetDocumentSetContentType;
        }

        public IEnumerable<string> GetInternalFieldNames(string libraryTitle, string fileIdResource)
        {
            throw new NotImplementedException();
        }
    }
}
