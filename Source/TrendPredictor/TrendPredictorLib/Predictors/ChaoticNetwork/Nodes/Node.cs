using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    // IMPORTANT: NOTE: it works on assumption that parent order is not important
    public class Node
    {
        const int PARENTS_NO = 2;

        private List<double> arguments = new List<double>(PARENTS_NO);
        private Func<List<double>, double> calculateFoo_;

        public List<Node> Inputs = new List<Node>(PARENTS_NO);
        public List<Node> Outputs = new List<Node>();

        public double Value { get; private set; }

        public Node(Func<List<double>, double> calculateFoo)
        {
            calculateFoo_ = calculateFoo;
        }

        /// <summary>
        /// make connected nodes avoid this node in network
        /// </summary>
        public void SelfDestruct()
        {
            if (Inputs.Count() == 0)
                throw new ApplicationException("its input node - cant remove it");

            Node nodeToConnectOutputsTo = Inputs[0];
            foreach (var item in Outputs)
            {
                item.Inputs.Remove(this);
                item.Inputs.Add(nodeToConnectOutputsTo);
                nodeToConnectOutputsTo.Outputs.Add(item);
            }

            foreach (var item in Inputs)
            {
                item.Outputs.Remove(this);
            }
        }

        public void Transform(Func<List<double>, double> calculateFoo)
        {
            calculateFoo_ = calculateFoo;
        }

        public void ConnectWithOutput(Node output)
        {
            if (output.Inputs.Count == PARENTS_NO)
                throw new ApplicationException("output node already has all it inputs");

            Outputs.Add(output);
            output.Inputs.Add(this);
        }

        public void Reset()
        {
            arguments.Clear();
        }

        public void ProvideArgument(double arg)
        {
            arguments.Add(arg);

            // when is only 1 argument needed its just 1st parameter taken (it could be 2nd - its not important)
            if (arguments.Count == PARENTS_NO)
            {
                Value = calculateFoo_(arguments);
                foreach (var item in Outputs)
                {
                    item.ProvideArgument(arg);
                }
            }
        }
    }
} 