using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointRestLibrary.Data;

namespace SharePointRestLibrary.Tests
{
    [TestClass]
    public class SQLManagerTests
    {
        private string _connection = @"Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;";
        [TestMethod]
        public void CreateManagerTest()
        {
            //arrange
            using (var dm = new SQLManager(_connection))
            {
                //act
                var items = dm.GetColumnNames("select * from GeneralLedger", "file_name");

                //assert
                Assert.AreEqual(items.Count, 4);
            }
        }

        [TestMethod]
        public void GetDataTest()
        {
            //arrange
            using (var dm = new SQLManager(_connection))
            {
                //act
                var items = dm.GetData("select * from GeneralLedger", "file_name");

                //assert
                Assert.AreEqual(items.Count, 5);
            }
        }

    }
}
