using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointRestLibrary.Data;

namespace SharePointRestLibrary.Tests
{
    [TestClass]
    public class SQLManagerTests
    {
        [TestMethod]
        public void CreateManagerTest()
        {
            //arrange
            using (var dm = new SQLManager(@"Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;"))
            {
                //act
                var items = dm.GetColumnNames("select * from GeneralLedger", "file_name");

                //assert
                Assert.AreEqual(items.Count, 4);
            }
        }

    }
}
