using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrendPredictorLib;

namespace TrendPredictorLibUnitTests
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        public void ModifiedFibbonacciTest()
        {
            Assert.AreEqual(0, Helpers.ModifiedFibbonacci(0));
            Assert.AreEqual(1, Helpers.ModifiedFibbonacci(1));
            Assert.AreEqual(2, Helpers.ModifiedFibbonacci(2));
            Assert.AreEqual(3, Helpers.ModifiedFibbonacci(3));
            Assert.AreEqual(5, Helpers.ModifiedFibbonacci(4));
            Assert.AreEqual(8, Helpers.ModifiedFibbonacci(5));
            Assert.AreEqual(13, Helpers.ModifiedFibbonacci(6));
            Assert.AreEqual(21, Helpers.ModifiedFibbonacci(7));
            Assert.AreEqual(34, Helpers.ModifiedFibbonacci(8));
        }
    }
}
