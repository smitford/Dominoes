using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dominoes.DB;

namespace Dominoes.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DbWorks()
        {
            var dbconn = new DbConnector();
            var result = dbconn.Add("Test Name", true);
            var history = dbconn.GetHistory();
            bool deleted = dbconn.Remove(result);
            Assert.IsTrue(history.Contains(result) && deleted);
        }
    }
}
