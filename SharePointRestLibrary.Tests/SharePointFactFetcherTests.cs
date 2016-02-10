using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointRestLibrary.SharePoint;
using System.Collections.Generic;

namespace SharePointRestLibrary.Tests
{
    [TestClass]
    public class SharePointFactFetcherTests
    {
        private string _testUserId = "baaqmd-dev\\sp_farm";
        private string _testPassword = "P@$$word1!";
        private string _testBaseUrl = "http://baaqmd-dev.cloudapp.net/sites/RecordCenter";
        private string _testLibrary = "General Ledger";

        [TestMethod]
        public void GetFileCountByLocationTest()
        {
            //arrange
            ISharePointFactFetcher fetcher = new SharePointFactFetcher (
                _testUserId,
                _testPassword,
                _testBaseUrl
            );
            
            //act
            var count = fetcher.GetFileCountByLocation(_testLibrary);

            //assert
            Assert.AreEqual(count, 4);
        }

        [TestMethod]
        public void GetFilesByLocationTest()
        {
            //arrange
            ISharePointFactFetcher fetcher = new SharePointFactFetcher(
                _testUserId,
                _testPassword,
                _testBaseUrl
            );

            //act
            List<string> fileList = new List<string>(fetcher.GetFilesByLocation(_testLibrary));

            //assert
            Assert.AreEqual(fileList.Count, 4);
        }

        [TestMethod]
        public void SPFileExistInLibraryTest()
        {            
            //arrange
            ISharePointFactFetcher fetcher = new SharePointFactFetcher(
                _testUserId,
                _testPassword,
                _testBaseUrl
            );

            //act
            var fileList = new List<string>(fetcher.GetFilesByLocation(_testLibrary));
            var doesFileExist = fetcher.SPFileExistInLibrary(_testLibrary, fileList.ToArray()[0]);

            //assert
            Assert.IsTrue(doesFileExist);

        }

        //[TestMethod]
        //public void SPGetFieldListTest()
        //{

        //}
    }
}
