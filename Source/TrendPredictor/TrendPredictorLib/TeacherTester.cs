using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    class TeacherTester
    {
        public static List<DataPoint> GenerateSinWaveDataPoints(int pointsNo, double intervalBetwPoints)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            for (double i = 0.0d; i < pointsNo; i++)
            {
                double currInput = i * intervalBetwPoints;
                double currOutput = Math.Sin(currInput);

                DataPoint dp = new DataPoint();
                dp.input.Add(currInput);
                dp.input.Add(0.0d);
                dp.input.Add(0.0d);
                dp.output = currOutput;

                dataPoints.Add(dp);
            }

            return dataPoints;
        }

        public TeacherTester()
        {
            Logger.Log(this, "App start");

            NodeFuncGenerator funcGenerator = new NodeFuncGenerator();
            Dictionary<NodeType, double> nodeProbabilityMap = new Dictionary<NodeType, double>()
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
            List<DataPoint> dataPoints = GenerateSinWaveDataPoints(100, 0.19d);
            Network network = new Network(nodeFactory, dataPoints, 1000);
            double addRemoveChance = 0.5;
            PatchFactory patchFactory = new PatchFactory(network, nodeFactory, addRemoveChance);
            NetworkTeacher networkTeacher = new NetworkTeacher(network, patchFactory);

            Logger.Log(this, String.Format("initial error: {0}", network.CalculateTrainingSqrError()));
            networkTeacher.Teach(
                teachingTime: TimeSpan.FromSeconds(10000000.0d),
                wantedNoOfNodes: 300,
                changesPerPatch: 8,
                patchesPerTeachingSerie: 1
            );
            Logger.Log(this, String.Format("error after learning: {0}", network.CalculateTrainingSqrError()));

            Logger.Log(this, "App finish");
            Console.ReadKey();
        }
    }
}
