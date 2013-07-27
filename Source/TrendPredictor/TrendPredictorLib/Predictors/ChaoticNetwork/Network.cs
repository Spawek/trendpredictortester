using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public class Network
    {
        private List<DataPoint> trainingData_;
        private int inputsNo_;

        public Network(List<DataPoint> trainingData)
        {
            ValidateAndCopyTrainingData(trainingData);
            BuildNetworkBasis();
        }

        private void BuildNetworkBasis()
        {
            Nodes = new List<Node>();
            Inputs = new List<CopyNode>();

            for (int i = 0; i < inputsNo_; i++)
            {
                CopyNode input = new CopyNode();

                Inputs.Add(input);
                Nodes.Add(input);
            }
            Output = new CopyNode(); //NODE: output is not on node list
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

        private List<Node> Nodes { get; set; }
        private List<CopyNode> Inputs { get; set; }
        private CopyNode Output { get; set; }
        private OperationFactory OperationFactory { get; set; }
    }
}
