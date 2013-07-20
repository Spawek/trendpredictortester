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
        List<double> high = new List<double>() { 4.0d, 4.1d, 4.0d, 4.2d, 4.4d };
        List<double> low = new List<double>() { 3.0d, 3.4d, 3.5d, 3.0d, 3.5d };
        List<double> close = new List<double>() { 3.5d, 3.6d, 3.8d, 3.9d, 3.8d };

        [TestMethod]
        public void IndicatorsCalculatorChangeIndicatorTest()
        {
            List<double> changeInd = IndicatorsCalculator.CalcChangeIndicator(smallTestData);

            Assert.AreEqual(smallTestData.Count, changeInd.Count);

            double delta = 0.0001d;
            Assert.AreEqual(double.NaN, changeInd[0]);
            Assert.AreEqual(1.0d, changeInd[1], delta); //  100% change (1.0 -> 2.0) 
            Assert.AreEqual(0.5d, changeInd[2], delta); //  50% change (2.0 -> 3.0)
        }

        [TestMethod]
        public void IndicatorsCalculatorMovingMeanTest()
        {
            List<double> movingMeanInd = IndicatorsCalculator.CalcMovingMean(smallTestData, 3);

            Assert.AreEqual(smallTestData.Count, movingMeanInd.Count);

            double delta = 0.0001d;
            Assert.AreEqual(double.NaN, movingMeanInd[0]);
            Assert.AreEqual(double.NaN, movingMeanInd[1]);
            Assert.AreEqual(2.0d, movingMeanInd[2], delta); // (1+2+3)/3 = 2;
            Assert.AreEqual(3.0d, movingMeanInd[3], delta); // (2+3+4)/3 = 2;
        }

        [TestMethod]
        public void IndicatorsCalculatorCalcaulatePercentRIndicator()
        {
            int n = 5;

            List<double> percRInd = IndicatorsCalculator.CalcPercentRIndicator(high, low, close, n);

            Assert.AreEqual(high.Count, percRInd.Count);
            for (int i = 0; i < n - 1; i++)
			{
                Assert.AreEqual(double.NaN, percRInd[i]);
			}

            // %R = (MAX(H,n) - C)/(MAX(H,n) - MIN(L,n))
            Assert.AreEqual((4.4d - 3.8d)/(4.4d - 3.0d), percRInd[4]);
        }

        [TestMethod]
        public void IndicatorsCalculatorCalcaulatePercentRIndicatorModified()
        {
            int n = 5;

            List<double> percRIndMod = IndicatorsCalculator.CalcPercentRIndicatorModified(high, low, close, n);

            Assert.AreEqual(high.Count, percRIndMod.Count);
            for (int i = 0; i < n - 1; i++)
            {
                Assert.AreEqual(double.NaN, percRIndMod[i]);
            }

            double delta = 0.0001;

            // %R = (C - MIN(L,n))/(MAX(H,n) - MIN(L,n))
            Assert.AreEqual((3.8d - 3.0d) / (4.4d - 3.0d), percRIndMod[4], delta);
        }

        [TestMethod]
        public void IndicatorsCalculatorCalculateWilliamsAccumulationDistributionIndicator()
        {
            List<double> willamsAccDistInd = IndicatorsCalculator.CalcWilliamsAccumulationDistributionIndicator(high, low, close);

            double delta = 0.0001;

            Assert.AreEqual(5, willamsAccDistInd.Count);
            Assert.AreEqual(0.0d, willamsAccDistInd[0], delta);
            Assert.AreEqual(0.2d, willamsAccDistInd[1], delta);
            Assert.AreEqual(0.5d, willamsAccDistInd[2], delta);
            Assert.AreEqual(1.4d, willamsAccDistInd[3], delta);
            Assert.AreEqual(0.8d, willamsAccDistInd[4], delta);
        }

    }
}
