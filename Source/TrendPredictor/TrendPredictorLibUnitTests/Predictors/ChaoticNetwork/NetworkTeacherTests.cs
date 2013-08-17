using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendPredictorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrendPredictorLibUnitTests
{
    [TestClass]
    class NetworkTeacherTests
    {
        public List<DataPoint> GenerateSinWaveDataPoints(int pointsNo, double intervalBetwPoints)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            for (double i = 0.0d; i < pointsNo; i++)
            {
                double currInput = i*intervalBetwPoints;
                double currOutput = Math.Sin(currInput);

                DataPoint dp = new DataPoint();
                dp.input.Add(currInput);
                dp.input.Add(0.0d);
                dp.output = currOutput;

                dataPoints.Add(dp);
            }

            return dataPoints;
        }

        [TestMethod]
        public void SinWaveTrainingTest()
        {
            NodeFuncGenerator funcGenerator = new NodeFuncGenerator();
            Dictionary<NodeType, double> nodeProbabilityMap = new Dictionary<NodeType,double>()
            {
                {NodeType.add, 1.0d},
                {NodeType.compare, 1.0d},
                {NodeType.copy, 0.0d},
                {NodeType.inverse, 1.0d},
                {NodeType.log, 1.0d},
                {NodeType.multiply, 1.0d},
                {NodeType.reverse, 1.0d}
            };
            NodeFactory nodeFactory = new NodeFactory(funcGenerator, nodeProbabilityMap);
            List<DataPoint> dataPoints = GenerateSinWaveDataPoints(100, 0.06d);
            Network network = new Network(nodeFactory, dataPoints, 1000);
            double addRemoveChance = 0.5;
            PatchFactory patchFactory = new PatchFactory(network, nodeFactory, addRemoveChance);
            NetworkTeacher networkTeacher = new NetworkTeacher(network, patchFactory);

            networkTeacher.Teach(TimeSpan.FromSeconds(2.0d), 2000, 3, 30);
        }
        
    }
}
