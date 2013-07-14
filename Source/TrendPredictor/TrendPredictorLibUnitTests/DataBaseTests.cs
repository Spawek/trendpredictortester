using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrendPredictorLib;

namespace TrendPredictorLibUnitTests
{
    [TestClass]
    public class DataBaseTests
    {
        [TestMethod]
        public void DataBaseAccessTest()
        {
            DataBase db = new DataBase();

            const double val1 = 2.0d;
            const double val2 = 3.0d;
            const string key = "Dupa";

            db[key].Add(val1);
            db[key].Add(val2);

            Assert.AreEqual(2, db[key].Count);
            Assert.AreEqual(val1, db[key][0]);
            Assert.AreEqual(val2, db[key][1]);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DataBaseValidationShouldFailOnFDiffNoOfParameters()
        {
            DataBase db = new DataBase();

            db["a"].Add(1.0d);

            db["b"].Add(1.0d);
            db["b"].Add(2.0d);

            db.ValidateDB(); //expect exception
        }

        [TestMethod]
        public void DataBaseValidationShouldPasslOnCorrectNoOfParametes()
        {
            DataBase db = new DataBase();

            db["a"].Add(1.0d);
            db["a"].Add(2.0d);

            db["b"].Add(1.0d);
            db["b"].Add(2.0d);

            db.ValidateDB(); //expect no exception
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DataBaseValidationShouldFailOnNaNValueInsideDb()
        {
            DataBase db = new DataBase();
            db["a"].Add(Double.NaN);

            db.ValidateDB(); //expect exception
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DataBaseValidationShouldFailOnInfValueInsideDb()
        {
            DataBase db = new DataBase();
            db["a"].Add(Double.PositiveInfinity);

            db.ValidateDB(); //expect exception
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DataBaseValidationShouldFailOnMinusInfValueInsideDb()
        {
            DataBase db = new DataBase();
            db["a"].Add(Double.NegativeInfinity);

            db.ValidateDB(); //expect exception
        }

        [TestMethod]
        public void DataBaseValidationShouldCountVectorElementsCorrectly()
        {
            DataBase db = new DataBase();

            db["b"].Add(1.0d);
            db["b"].Add(1.0d);

            Assert.AreEqual(2, db.CountVectorElements);
        }

        [TestMethod]
        public void DataBaseCloneRangeTest()
        {
            DataBase db = new DataBase();

            db["a"].Add(1.0d);
            db["a"].Add(2.0d);
            db["a"].Add(3.0d);

            db["b"].Add(11.0d);
            db["b"].Add(12.0d);
            db["b"].Add(13.0d);

            
            const int beginIndex = 1;
            const int count = 2;
            DataBase dbClone = db.CloneDbRange(beginIndex, count);

            Assert.AreEqual(2, dbClone.CountVectorElements);
            Assert.AreEqual(2.0d, dbClone["a"][0]);
            Assert.AreEqual(3.0d, dbClone["a"][1]);
            Assert.AreEqual(12.0d, dbClone["b"][0]);
            Assert.AreEqual(13.0d, dbClone["b"][1]);
        }


        [TestMethod]
        public void DataBasePlusOperatorTest()
        {
            DataBase db1 = new DataBase();
            db1["a"].Add(1.0d);

            DataBase db2 = new DataBase();
            db2["b"].Add(2.0d);

            db1 += db2;

            Assert.AreEqual(1.0d, db1["a"][0]);
            Assert.AreEqual(2.0d, db1["b"][0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataBasePlusOperatorShouldFailIfKeysAreSame()
        {
            DataBase db1 = new DataBase();
            db1["a"].Add(1.0d);

            DataBase db2 = new DataBase();
            db2["a"].Add(2.0d);

            db1 += db2;
        }

    }
}
