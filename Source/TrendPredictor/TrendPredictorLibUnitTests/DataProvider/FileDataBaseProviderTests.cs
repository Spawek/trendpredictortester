using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrendPredictorLib;

namespace TrendPredictorLibUnitTests
{
    [TestClass]
    public class FileDataBaseProviderTests
    {
        [TestMethod]
        public void FileDataBaseProviderTest()
        {
            DataBaseProvider dbProvider = new FileDataBaseProvider("../../../../../Data/EURUSD.csv");
            var db = dbProvider.GetDataBase();

            Assert.AreNotEqual(0, db.CountVectorElements);
            //provider will throw exception in case of any problems so more assertions are not needed
        }

    }
}
