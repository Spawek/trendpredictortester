using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public class NodeFactory
    {
        private Dictionary<NodeType, double> typeDistributionMap_;
        double typeDistributionMapMax_;
        Random rand_ = new Random();
        NodeFuncGenerator funcGenerator_;

        public NodeFactory(NodeFuncGenerator funcGenerator, IDictionary<NodeType, double> probabilityMap)
        {
            if (probabilityMap.Count == 0)
                throw new ArgumentException();
            
            foreach (var item in probabilityMap)
            {
                if (item.Value < 0)
                    throw new ArgumentException();
            }

            funcGenerator_ = funcGenerator;

            typeDistributionMap_ = new Dictionary<NodeType, double>(probabilityMap);
            double sum = 0.0d;
            for (int i = 0; i < probabilityMap.Count; i++)
            {
                sum += probabilityMap.ElementAt(i).Value;
                typeDistributionMap_.Add(probabilityMap.ElementAt(i).Key, sum);
            }
            typeDistributionMapMax_ = sum;
        }

        private NodeType GenerateRandomNodeType()
        {
            double randomNo = rand_.NextDouble() * typeDistributionMapMax_;
            return typeDistributionMap_.First(x => x.Value > randomNo).Key;
        }

        /// <summary>
        /// it generates rand val in range (0, typeDistrubutionMap.Sum()),
        /// it finds key of 1st val higher than rand
        /// </summary>
        /// <returns></returns>
        public Node CreateRandomNode()
        {
            return CreateNode(GenerateRandomNodeType());
        }

        public Node CreateNode(NodeType type)
        {
           return new Node(type);
        }

        public void TransformNode(Node node, NodeType targetType)
        {
            node.Transform(targetType);
        }

        public void RandomlyTransformNode(Node node)
        {
            TransformNode(node, GenerateRandomNodeType());
        }
    }
}
