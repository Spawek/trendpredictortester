using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrendPredictorLib;
using System.Collections.Generic;

namespace TrendPredictorLibUnitTests
{
    [TestClass]
    public class DataPointCutterTests
    {
        [TestMethod]
        public void DataPointCutterCutMethodTest()
        {
            DataBase db = new DataBase(new List<double> { 1.0d, 2.0d, 3.0d, 4.0d, 5.0d }, "test");

            DataPointCutter cutter = new DataPointCutter(2, new HashSet<string>() { "test" }, 2, "test");

            DataPoint dataPoint = cutter.Cut(db, 2);

            Assert.AreEqual(2, dataPoint.input.Count);
            Assert.AreEqual(3.0d, dataPoint.input[0]);
            Assert.AreEqual(2.0d, dataPoint.input[1]);

            Assert.AreEqual(5.0d, dataPoint.output);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DataPointCutterCutMethodShouldThrowTest()
        {
            DataBase db = new DataBase(new List<double> { 1.0d, 2.0d, 3.0d, 4.0d, 5.0d }, "test");

            DataPointCutter cutter = new DataPointCutter(2, new HashSet<string>() { "test" }, 2, "test");

            DataPoint dataPoint = cutter.Cut(db, 0);
        }
    }
}
