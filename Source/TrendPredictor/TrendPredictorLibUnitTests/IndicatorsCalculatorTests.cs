using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrendPredictorLib;
using System.Collections.Generic;

namespace TrendPredictorLibUnitTests
{
    [TestClass]
    public class IndicatorsCalculatorTests
    {
        List<double> smallTestData = new List<double>() { 1.0d, 2.0d, 3.0d, 4.0d, 5.0d, 6.0d };

        [TestMethod]
        public void IndicatorsCalculatorChangeIndicatorTest()
        {
            List<double> changeInd = IndicatorsCalculator.CalculateChangeIndicator(smallTestData);

            Assert.AreEqual(smallTestData.Count, changeInd.Count);

            double delta = 0.0001d;
            Assert.AreEqual(double.NaN, changeInd[0]);
            Assert.AreEqual(1.0d, changeInd[1], delta); //  100% change (1.0 -> 2.0) 
            Assert.AreEqual(0.5d, changeInd[2], delta); //  50% change (2.0 -> 3.0)
        }

        [TestMethod]
        public void IndicatorsCalculatorMovingMeanTest()
        {
            List<double> movingMeanInd = IndicatorsCalculator.CalculateMovingMean(smallTestData, 3);

            Assert.AreEqual(smallTestData.Count, movingMeanInd.Count);

            double delta = 0.0001d;
            Assert.AreEqual(double.NaN, movingMeanInd[0]);
            Assert.AreEqual(double.NaN, movingMeanInd[1]);
            Assert.AreEqual(2.0d, movingMeanInd[2], delta); // (1+2+3)/3 = 2;
            Assert.AreEqual(3.0d, movingMeanInd[3], delta); // (2+3+4)/3 = 2;
        }

    }
}
