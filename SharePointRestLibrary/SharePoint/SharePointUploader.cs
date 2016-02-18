using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharePointRestLibrary.Extensions;
using Microsoft.SharePoint.Client;
using SharePointRestLibrary.Configuration;
using Microsoft.SharePoint.Client.Taxonomy;

namespace SharePointRestLibrary.SharePoint
{
    public class SharePointUploader : ISharePointUploader
    {
        private string _username;
        private string _password;
        private ICredentials _credentials;
        private string _baseUrl;
        private IDictionary<string, IEnumerable<string>> _filesByLibraryCache;
        private List<ContentType> _ctypes = new List<ContentType>();
        private ClientContext ctx;
        private List _spList = null;
        private IDictionary<string, string> _spFieldList = null;
        private IDictionary<string, TaxonomyField> _spTaxFields = null;
        private IDictionary<string, IDictionary<string, string>> _terms = null;

        public SharePointUploader(string username, string password, string baseUrl)
        {
            _username = username;
            _password = password;
            _credentials = new NetworkCredential(_username, _password);
            _baseUrl = baseUrl;
            ctx = new ClientContext(_baseUrl);
            ctx.AuthenticationMode = ClientAuthenticationMode.Default;
            ctx.Credentials = _credentials;
        }

        private void LoadSPFieldList(string libraryTitle, ContentType contentType)
        {
            _spFieldList = new Dictionary<string,string>();
            _spTaxFields = new Dictionary<string, TaxonomyField>();
            
            ctx.Load(contentType.Fields);
            ctx.ExecuteQuery();

            foreach (var field in contentType.Fields)
	        {
                if (field.TypeAsString.Contains("TaxonomyFieldType"))
                {
                    TaxonomyField tField = ctx.CastTo<TaxonomyField>(field);
                    _spTaxFields[field.Title] = tField;
                }
                else
                {
                    _spFieldList[field.Title] = field.InternalName;
                }
	        }
        }

        private string GetTermIdForTaxonomyField(TaxonomyField field, string term, ListItem pendingItem,Microsoft.SharePoint.Client.File pendingFile)
        {
            if (_terms == null)
                _terms = new Dictionary<string, IDictionary<string, string>>();

            if (!_terms.Keys.Contains(field.Title))
                _terms[field.Title] = new Dictionary<string, string>();

            if (_terms[field.Title].Keys.Contains(term))
                return _terms[field.Title][term].ToString();

            var termId = string.Empty;

            //before we go forward,save pending item
            pendingItem.Update();
            ctx.Load(pendingFile);
            ctx.ExecuteQuery();

            TaxonomySession tSession = TaxonomySession.GetTaxonomySession(ctx);
            ctx.Load(tSession.TermStores);
            ctx.ExecuteQuery();
            TermStore ts = tSession.TermStores.First();
            TermSet tset = ts.GetTermSet(field.TermSetId);

            LabelMatchInformation lmi = new LabelMatchInformation(ctx);

            lmi.Lcid = 1033;
            lmi.TrimUnavailable = true;
            lmi.TermLabel = term;

            TermCollection termMatches = tset.GetTerms(lmi);
            ctx.Load(tSession);
            ctx.Load(ts);
            ctx.Load(tset);
            ctx.Load(termMatches);

            ctx.ExecuteQuery();

            if (termMatches != null && termMatches.Count() > 0)
                termId = termMatches.First().Id.ToString();

            _terms[field.Title][term] = termId;

            return termId;
        }

        public bool SPFileExistInLibrary(string libraryTitle, string fileName, bool refreshCache = false)
        {
            if (_filesByLibraryCache == null)
                _filesByLibraryCache = new Dictionary<string, IEnumerable<string>>();


            if (_filesByLibraryCache == null || !_filesByLibraryCache.ContainsKey(libraryTitle) || refreshCache)
            {
                _filesByLibraryCache[libraryTitle] = GetFilesByLocation(libraryTitle);
            }

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
            var web = ctx.Web;
            var list = web.Lists.GetByTitle(libraryTitle);
            ctx.Load(list);
            ctx.Load(list.RootFolder);
            ctx.Load(list.RootFolder.Files);
            ctx.ExecuteQuery();

            foreach (var item in list.RootFolder.Files)
            {
                listOut.Add(GetFileNameFromUri(item.ServerRelativeUrl));
            }             

            return listOut;
        }

        private string GetFileNameFromUri(string uriIn)
        {
            return uriIn.Split('/').Last();
        }

        public void UploadFile(string sourceFolder, string filename, string libraryTitle, string contentType)
        {
            if (!SPFileExistInLibrary(libraryTitle, filename))
            {
                var ct = GetContentType(ctx, libraryTitle, contentType);
                var urlTarget = PushFileToSP(sourceFolder, filename, libraryTitle);
                var uploadedFile = _spList.RootFolder.Files.GetByUrl(urlTarget);
                var listItem = uploadedFile.ListItemAllFields;
                listItem["ContentTypeId"] = ct.Id.ToString();
                CommitToSP(listItem, uploadedFile);
            }
        }

        /// <summary>
        /// returns url of the file uploaded in sharepoint.
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="filename"></param>
        /// <param name="libraryTitle"></param>
        /// <param name="contentType"></param>
        /// <returns>url of the file uploaded in sharepoint.</returns>
        private string PushFileToSP(string sourceFolder, string filename, string libraryTitle)
        {
            string filepath = sourceFolder.Trim() + filename;

            if (_spList == null)
            {
                _spList = ctx.Web.Lists.GetByTitle(libraryTitle);
                ctx.Load(_spList.RootFolder);
                ctx.ExecuteQuery();
            }

            if (ctx.HasPendingRequest)
                ctx.ExecuteQuery();


            var urlTarget = string.Format("{0}/{1}", _spList.RootFolder.ServerRelativeUrl, filename.Trim());

            Microsoft.SharePoint.Client.File.SaveBinaryDirect(
                ctx, urlTarget, new FileStream(filepath, FileMode.Open, FileAccess.Read), false);

            return urlTarget;
        }

        private void ApplyMetadataRecordToListItem(SPDataRecord record, string urlTarget, ContentType ct)
        {
            
            var uploadedFile = _spList.RootFolder.Files.GetByUrl(urlTarget);
            var listItem = uploadedFile.ListItemAllFields;
            listItem["ContentTypeId"] = ct.Id.ToString();

            foreach (var spfield in record)
            {
                try
                {
                    if (spfield.SPDataType != "Taxonomy")
                        listItem[_spFieldList[spfield.SPColumnInternalName]] = spfield.SPValue;
                    else
                    {
                        try
                        {
                            listItem[_spTaxFields[spfield.SPColumnInternalName].InternalName] = GetTermIdForTaxonomyField(_spTaxFields[spfield.SPColumnInternalName], spfield.SPValue, listItem, uploadedFile);
                        }
                        catch { }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            CommitToSP(listItem, uploadedFile);

        }

        public void UploadFile(string sourceFolder, SPDataRecord record, string libraryTitle, string contentType)
        {
            var ct = GetContentType(ctx, libraryTitle, contentType);

            if (_spFieldList == null)
                LoadSPFieldList(libraryTitle, ct);

            if (!SPFileExistInLibrary(libraryTitle, record.FileName))
            {
                var urlTarget = PushFileToSP(sourceFolder, record.FileName, libraryTitle);

                ApplyMetadataRecordToListItem(record, urlTarget, ct);
            }
            else
            {
                throw new ApplicationException(string.Format("File {0} already exists on target.  Skipping.", record.FileName));
            }

        }

        private void CommitToSP(ListItem itemIn, Microsoft.SharePoint.Client.File uploadedFile)
        {
            try
            {
                itemIn.Update();
                ctx.Load(uploadedFile);
                ctx.ExecuteQuery();
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.Message);
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
            if (_ctypes.Count == 0)
            {
                var docs = ctx.Web.Lists.GetByTitle(libraryName);
                ContentTypeCollection listContentTypes = docs.ContentTypes;
                ctx.Load(listContentTypes, types => types.Include
                         (type => type.Id, type => type.Name,
                           type => type.Parent));
                var results = ctx.LoadQuery(listContentTypes);
                ctx.ExecuteQuery();

                foreach (var item in results)
                {
                    _ctypes.Add(item);
                }
            }
            var targetDocumentSetContentType = _ctypes.Single(p=>p.Name.Equals(contentType));
            return targetDocumentSetContentType;
        }

        public IEnumerable<string> GetInternalFieldNames(string libraryTitle, string fileIdResource)
        {
            throw new NotImplementedException();
        }

    }
}
