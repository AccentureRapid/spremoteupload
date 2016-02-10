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
            var count = fetcher.GetFileCountByLocation("General Ledger");

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
            List<string> fileList = new List<string>(fetcher.GetFilesByLocation("General Ledger"));

            //assert
            Assert.AreEqual(fileList.Count, 4);
        }
    }
}
