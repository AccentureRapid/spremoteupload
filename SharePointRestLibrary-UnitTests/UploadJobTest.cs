using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointRestLibrary.Document;
using SharePointRestLibrary.Data;

namespace SharePointRestLibrary_UnitTests
{
    [TestClass]
    public class UploadJobTest
    {
        private IUploadJobProvider uploader;

        [TestMethod]
        public void ValidateConnection()
        {
            var mgr = new SqlManager("Server = rogerb-pc\\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;");

        }
    }
}
