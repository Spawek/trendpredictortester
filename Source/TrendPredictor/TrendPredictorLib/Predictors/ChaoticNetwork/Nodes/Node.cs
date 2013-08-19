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
        public int Hierarchy { get; private set; }
        public int NodeID { get; private set; }

        public const double MAGIC_NUMBER = -53532627245.453523643743;

        public Node(NodeType nodeType, int hierarchy)
        {
            Transform(nodeType);
            Hierarchy = hierarchy;
#if DEBUG
            NodeID = getNextNodeID();
#endif
        }

        public override string ToString()
        {
            bool is1st = true;
            string inputsString = "{";
            foreach (Node input in Inputs)
            {
                if (!is1st)
                {
                    inputsString += ", ";
                }
                is1st = false;
                inputsString += input.NodeID.ToString();
            }
            inputsString += "}";

            is1st = true;
            string outputsString = "{";
            foreach (Node output in Outputs)
            {
                if (!is1st)
                {
                    outputsString += ", ";
                }
                is1st = false;
                outputsString += output.NodeID.ToString();
            }
            outputsString += "}";

            return String.Format("ID:{0}; I:{1}; O:{2} ", 
                NodeID.ToString(), 
                inputsString, 
                outputsString);
        }

        public string ToVisualizationString()
        {
            string nodeTypeString;
            if (Inputs.Count == 0)
            {
                nodeTypeString = "Input";
            }
            else if (Outputs.Count == 0)
            {
                nodeTypeString = "Output";
            }
            else
            {
                nodeTypeString = NodeType.ToString();
            }

            return String.Format("{0}\n{1}",
                    NodeID.ToString(),
                    nodeTypeString);
        }

        public override int GetHashCode()
        {
            int hash = 0;

            hash += 3 * ParentsNo;
            hash += NodeType.ToString().GetHashCode();
            foreach (Node input in Inputs)
            {
                hash += 5 * input.NodeID;
            }
            foreach (Node output in Outputs)
            {
                hash += 7 * output.NodeID;
            }
            hash += 9 * Hierarchy;

            return hash;
        }

        private static int nextNodeID = 1;
        private static int getNextNodeID()
        {
            return nextNodeID++;
        }

        public void checkIfNodeConnectionsAreCorrrect()
        {
            foreach (Node node in Inputs)
            {
                if (node.Outputs.Find(this) == null)
                    throw new ApplicationException();
            }

            foreach (Node node in Outputs)
            {
                if (node.Inputs.Find(this) == null)
                    throw new ApplicationException();
            }
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
            if (output.Inputs.Count == output.ParentsNo)
                throw new ApplicationException("output node already has all it inputs"); //does not work with ne ParentsNo (1 for copy)

            Outputs.AddLast(output);
            output.Inputs.AddLast(this);
        }

        public void Reset()
        {
            arguments.Clear();
#if DEBUG
            Value = MAGIC_NUMBER;
#endif
        }

        public void ProvideArgument(double arg)
        {
            arguments.Add(arg);

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