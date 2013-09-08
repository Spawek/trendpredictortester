using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrendPredictorLib;
using QuickGraph;

namespace TrendPredictorLib
{
    public class Network
    {
        public List<Node> Operations { get; private set; }
        public List<Node> Inputs { get; private set; }
        private Node Output { get; set; }
        private NodeFactory NodeFactory { get; set; }
        public int MaxHierarchy { get; private set; }

        private List<DataPoint> trainingData_;
        private int inputsNo_;

        public Network(NodeFactory nodeFactory, List<DataPoint> trainingData, int maxHierarchy)
        {
            NodeFactory = nodeFactory;
            MaxHierarchy = maxHierarchy;

            ValidateAndCopyTrainingData(trainingData);
            BuildNetworkBasis();
        }

        public double CalculateTrainingSqrError()
        {
            double error = 0.0;
            foreach (DataPoint trainingPoint in trainingData_)
            {
                ResetAllNodes();
                for (int i = 0; i < inputsNo_; i++)
                {
                    Inputs[i].ProvideArgument(trainingPoint.input[i]);
                }
                error += Math.Pow(Output.Value - trainingPoint.output, 2.0d);
            }
#if DEBUG
            CheckNetworkCorrectness();
#endif

            return error;
        }

        public string PrintNetworkStruct()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Inputs:");
            foreach (Node node in Inputs)
            {
                builder.AppendLine(node.ToString());
            }

            builder.AppendLine("Operations:\n");
            foreach (Node node in Operations)
            {
                builder.AppendLine(node.ToString());
            }

            builder.AppendLine("Output:");
            builder.AppendLine(Output.ToString());

            return builder.ToString();
        }

        public IBidirectionalGraph<object, IEdge<object>> ConvertToQuickGraphForm() //TODO: refactor it!
        {
            BidirectionalGraph<object, IEdge<object>> graph = new BidirectionalGraph<object, IEdge<object>>();
            
            // add vertices
            foreach (Node input in Inputs)
            {
                graph.AddVertex(input.ToVisualizationString());
            }
            foreach (Node operation in Operations)
            {
                graph.AddVertex(operation.ToVisualizationString());
            }
            graph.AddVertex(Output.ToVisualizationString());

            // add edges
            foreach (Node node in Inputs) //IMPORTANT: this edges are probably not needed - operations will do it all
            {
                string nodeVisualizationString = node.ToVisualizationString();
                foreach (var input in node.Inputs)
	            {
                    graph.AddEdge(new Edge<object>(input.ToVisualizationString(), nodeVisualizationString));
	            }
                foreach (var output in node.Outputs)
                {
                    graph.AddEdge(new Edge<object>(nodeVisualizationString, output.ToVisualizationString()));
                }
            }
            foreach (Node node in Operations)
            {
                string nodeVisualizationString = node.ToVisualizationString();
                foreach (var input in node.Inputs)
                {
                    graph.AddEdge(new Edge<object>(input.ToVisualizationString(), nodeVisualizationString));
                }
                foreach (var output in node.Outputs)
                {
                    graph.AddEdge(new Edge<object>(nodeVisualizationString, output.ToVisualizationString()));
                }
            }
            foreach (var input in Output.Inputs)
            {
                graph.AddEdge(new Edge<object>(input.ToVisualizationString(), Output.ToVisualizationString()));
            }
            foreach (var output in Output.Outputs)
            {
                graph.AddEdge(new Edge<object>(Output.ToVisualizationString(), output.ToVisualizationString()));
            }

            return graph;
        }

        private void CheckNetworkCorrectness()
        {
            foreach (Node node in Inputs)
            {
                node.checkIfNodeConnectionsAreCorrrect();
                if (node.Value == Node.MAGIC_NUMBER)
                    throw new ApplicationException();
                if (node.Outputs.Distinct().ToList().Count != node.Outputs.Count)
                    throw new ApplicationException();
                if (node.Inputs.Count != 0)
                    throw new ApplicationException();
            }

            foreach (Node node in Operations)
            {
                node.checkIfNodeConnectionsAreCorrrect();
                if (node.Value == Node.MAGIC_NUMBER)
                    throw new ApplicationException();
                if (node.Inputs.Count != 2)
                    throw new ApplicationException("Wrong no of inputs!");
                if (node.Outputs.Count > 0)
                {
                    if (node.Hierarchy >= node.Outputs.Max(x => x.Hierarchy))
                        throw new ApplicationException("Wrong node hierarchy!");
                }
                if (node.Hierarchy <= node.Inputs.Max(x => x.Hierarchy))
                    throw new ApplicationException();
                if (node.Inputs.Distinct().ToList().Count != node.Inputs.Count)
                    throw new ApplicationException();
                if (node.Outputs.Distinct().ToList().Count != node.Outputs.Count)
                    throw new ApplicationException();
            }

            Output.checkIfNodeConnectionsAreCorrrect();
            if (Output.Value == Node.MAGIC_NUMBER)
                throw new ApplicationException();
            if (Output.Inputs.Distinct().ToList().Count != Output.Inputs.Count)
                throw new ApplicationException();
            if (Output.Outputs.Count != 0)
                throw new ApplicationException();
        }

        /// <summary>
        /// USE ONLY IN UT
        /// </summary>
        public void UTTestingFoo()
        {
            foreach (Node node in Inputs)
            {
                node.checkIfNodeConnectionsAreCorrrect();
                if (node.Outputs.Distinct().ToList().Count != node.Outputs.Count)
                    throw new ApplicationException();
            }

            foreach (Node node in Operations)
            {
                node.checkIfNodeConnectionsAreCorrrect();
                if (node.Inputs.Distinct().ToList().Count != node.Inputs.Count)
                    throw new ApplicationException();
                if (node.Outputs.Distinct().ToList().Count != node.Outputs.Count)
                    throw new ApplicationException();
            }
        }

        private void ResetAllNodes()
        {
            foreach (var item in Operations)
            {
                item.Reset();
            }
            foreach (var item in Inputs)
            {
                item.Reset();
            }
            Output.Reset();
        }

        /// <summary>
        /// input1 -> copyNode -> output
        /// input2(not connected)
        /// ...
        /// inputn
        /// </summary>
        private void BuildNetworkBasis()
        {
            Operations = new List<Node>();
            Operations.Add(NodeFactory.CreateNode(NodeType.add, 1));

            Inputs = new List<Node>();

            for (int i = 0; i < inputsNo_; i++)
            {
                Inputs.Add(NodeFactory.CreateNode(NodeType.copy, 0));
            }
            Output = NodeFactory.CreateNode(NodeType.copy, MaxHierarchy);

            Inputs[0].ConnectWithOutput(Operations[0]);
            Inputs[1].ConnectWithOutput(Operations[0]);
            Operations[0].ConnectWithOutput(Output);
        }

        private void ValidateAndCopyTrainingData(List<DataPoint> trainingData)
        {
            if (trainingData.Count < 1)
                throw new ArgumentException();

            inputsNo_ = trainingData[0].input.Count;
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
