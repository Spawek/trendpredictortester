using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Network(NodeFactory nodeFactory, List<DataPoint> trainingData)
        {
            NodeFactory = nodeFactory;

            ValidateAndCopyTrainingData(trainingData);
            BuildNetworkBasis();
        }

        private void BuildNetworkBasis()
        {
            Operations = new List<Node>();
            Inputs = new List<Node>();

            for (int i = 0; i < inputsNo_; i++)
            {
                Inputs.Add(NodeFactory.CreateNode(NodeType.copy));
            }
            Output = NodeFactory.CreateNode(NodeType.copy);

            Inputs[0].ConnectWithOutput(Output);
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
