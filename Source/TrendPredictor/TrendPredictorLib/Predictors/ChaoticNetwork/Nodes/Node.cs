using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    // IMPORTANT: NOTE: it works on assumption that parent order is not important
    public class Node
    {
        int ParentsNo { get; set; }

        private List<double> arguments = new List<double>();
        private Func<List<double>, double> calculateFoo_;
        private NodeType nodeType_;

        public LinkedList<Node> Inputs = new LinkedList<Node>();
        public LinkedList<Node> Outputs = new LinkedList<Node>();

        public double Value { get; private set; }
        public NodeType NodeType { get { return nodeType_; } }

        public Node(NodeType nodeType)
        {
            Transform(nodeType);
        }

        /// <summary>
        /// make connected nodes avoid this node in network
        /// </summary>
        public void SelfDestruct()
        {
            if (Inputs.Count() == 0)
                throw new ApplicationException("its input node - cant remove it");

            Node nodeToConnectOutputsTo = Inputs.First();
            foreach (var item in Outputs)
            {
                item.Inputs.Remove(this);
                item.Inputs.AddLast(nodeToConnectOutputsTo);
                nodeToConnectOutputsTo.Outputs.AddLast(item);
            }

            foreach (var item in Inputs)
            {
                item.Outputs.Remove(this);
            }
        }

        public void Transform(NodeType nodeType)
        {
            nodeType_ = nodeType;
            ParentsNo = 2;
            if (nodeType == TrendPredictorLib.NodeType.copy) //IMPORTANT: TODO: This is heavy hack - inputs was not working - fix it by changing this property someway for inputs
                ParentsNo = 1;
            calculateFoo_ = NodeFuncGenerator.GenerateFunc(nodeType);
        }

        public void ConnectWithOutput(Node output)
        {
            //if (output.Inputs.Count == ParentsNo)
            //    throw new ApplicationException("output node already has all it inputs"); //does not work with ne ParentsNo (1 for copy)

            Outputs.AddLast(output);
            output.Inputs.AddLast(this);
        }

        public void Reset()
        {
            arguments.Clear();
        }

        public void ProvideArgument(double arg)
        {
            arguments.Add(arg);

            // when is only 1 argument needed its just 1st parameter taken (it could be 2nd - its not important)
            if (arguments.Count == ParentsNo)
            {
                Value = calculateFoo_(arguments);
                foreach (var item in Outputs)
                {
                    item.ProvideArgument(Value);
                }
            }
        }
    }
} 