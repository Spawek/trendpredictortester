using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrendPredictorLib;
using System.Collections.Generic;

namespace TrendPredictorLibUnitTests.DataProvider
{
    [TestClass]
    public class ChaoticNetworkTests
    {
        private Network network;
        private PatchFactory patchFactory;
        private NetworkTeacher networkTeacher;

        [TestInitialize]
        public void SetUp()
        {
            NodeFuncGenerator nodeFuncGenerator = new NodeFuncGenerator();
            NodeFactory nodeFactory = new NodeFactory(
                nodeFuncGenerator,
                new Dictionary<NodeType, double>() { { NodeType.add, 1.0d } });
            network = new Network(
                nodeFactory: nodeFactory,
                trainingData: new List<DataPoint>()
                {
                    new DataPoint(new List<double>(){2.0d, 2.0d, 2.0d}, 5.0d) //IMPORTANT: TODO: was modified from "new DataPoint(new List<double>(){2.0d, 2.0d}, 5.0d)" - now test can work incorrectly
                },
                maxHierarchy: 1000
            );
            patchFactory = new PatchFactory(network, nodeFactory, 1.0d);
            networkTeacher = new NetworkTeacher(network, patchFactory);
        }

        [TestMethod]
        public void CalculateSquareErrorOnJustCreatedNetworkTest()
        {
            double expectedError = Math.Pow(5.0d - (2.0d + 2.0d), 2.0d);
            double actualError = network.CalculateTrainingSqrError();
            double delta = 1E-5d;

            Assert.AreEqual(expectedError, actualError, delta);
        }

        /* (2.0d) (4.0d)    (6.0d)    (6.0d)
         * I1 -> _Add------>_Add ---> Output
         *       /|         /|
         *(2.0d)/          /
         *   I2-----------/
         */
        [TestMethod]
        public void CalculateSquareErrorOnNetworkWith1AddedNodeTest()
        {
            patchFactory.CreateAndApplyPatch(1, int.MaxValue); //it should create add patch

            double expectedError = 1.0d;
            double actualError = network.CalculateTrainingSqrError();
            double delta = 1E-5d;

            Assert.AreEqual(expectedError, actualError, delta);
        }
    }
}
