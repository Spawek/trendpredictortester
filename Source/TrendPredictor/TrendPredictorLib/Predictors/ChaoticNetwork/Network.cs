using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrendPredictorLib;

namespace TrendPredictorLib
{
    public class Network
    {
        private List<Node> Operations { get; set; }
        private List<Node> Inputs { get; set; }
        private Node Output { get; set; }
        private NodeFactory NodeFactory { get; set; }

        private List<DataPoint> trainingData_;
        private int inputsNo_;

        public Network(NodeFactory nodeFactory, NetworkTeacher teacher, List<DataPoint> trainingData)
        {
            NodeFactory = nodeFactory;

            ValidateAndCopyTrainingData(trainingData);
            BuildNetworkBasis();
        }

        public double CalculateTrainingSqrError()
        {
            double error = 0.0;
            foreach (DataPoint trainingPoint in trainingData_)
            {
                for (int i = 0; i < inputsNo_; i++)
                {
                    Inputs[i].ProvideArgument(trainingPoint.input[i]);
                }
                error += Math.Pow(Output.Value - trainingPoint.output, 2.0d);
            }

            return error;
        }

        /// <summary>
        /// input1 -> copyNode -> output
        /// </summary>
        private void BuildNetworkBasis()
        {
            Operations = new List<Node>();
            Operations.Add(NodeFactory.CreateNode(NodeType.copy));

            Inputs = new List<Node>();

            for (int i = 0; i < inputsNo_; i++)
            {
                Inputs.Add(NodeFactory.CreateNode(NodeType.copy));
            }
            Output = NodeFactory.CreateNode(NodeType.copy);

            Inputs[0].ConnectWithOutput(Operations[0]);
        }

        private void ValidateAndCopyTrainingData(List<DataPoint> trainingData)
        {
            if (trainingData.Count < 2)
                throw new ArgumentException();

            inputsNo_ = trainingData_[0].input.Count;
            if (inputsNo_ < 1)
                throw new ArgumentException();

            foreach (var item in trainingData)
            {
                if (item.input.Count != inputsNo_)
                    throw new ArgumentException();
            }

            trainingData_ = new List<DataPoint>(trainingData);  // copied for safety
        }

    }
}
