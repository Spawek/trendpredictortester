using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class PatchFactory
    {
        private Network network_;
        private NodeFactory nodeFactory_;
        private double addRemoveChance_;
        private Random rand_ = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="network"></param>
        /// <param name="nodeFactory"></param>
        /// <param name="addRemoveChance">chance in range <0, 1> - rest is chance of tranformation</param>
        public PatchFactory(Network network, NodeFactory nodeFactory, double addRemoveChance)
        {
            if (addRemoveChance < 0.0d || addRemoveChance > 1.0d)
                throw new ArgumentOutOfRangeException("addRemoveChance");

            network_ = network;
            nodeFactory_ = nodeFactory;
            addRemoveChance_ = addRemoveChance;
        }

        public NetworkPatch CreateAndApplyPatch(int changesPerPatch, int wantedNoOfNodes)
        {
            NetworkPatch patch = new NetworkPatch();

            for (int i = 0; i < changesPerPatch; i++)
            {
                NetworkMutator mutator;
                if (rand_.NextDouble() < addRemoveChance_)
                {
                    mutator = CreateAddRemoveMutator(wantedNoOfNodes);
                }
                else
                {
                    mutator = CreateAddRemoveMutator(wantedNoOfNodes);
                }

                patch.mutators_.Add(mutator);
                mutator.Apply();
                /**
                 * NOTE: IMPORTANT: applying is here, because next mutator 
                 * can be generated wrongly if this one is not applied 
                 * (it'd fit for unchanged network, but not for changed one)
                 */
            }

            return patch;
        }

        private NetworkMutator CreateTypeChangeMutator()
        {
            Node node = GetRandomNode();
            return new NodeTypeChange(node, node.NodeType, nodeFactory_.GenerateRandomNodeType());
        }

        /// <summary></summary>
        /// <returns>random node with hierarchy in range <0, maxHierarchy-2> </returns>
        private Node GetRandomNode()
        {
            int operationsNo = network_.Operations.Count;
            int inputsNo = network_.Inputs.Count;

            int randomNo = rand_.Next(operationsNo + inputsNo);

            Node randomNode;
            if (randomNo < operationsNo)
            {
                randomNode = network_.Operations[randomNo];
                if (randomNode.Hierarchy >= network_.MaxHierarchy - 1)
                {
                    return GetRandomNode();
                }
            }
            else
            {
                randomNode = network_.Inputs[randomNo - operationsNo];
            }

            return randomNode;
        }

        private NetworkMutator CreateAddRemoveMutator(int wantedNoOfNodes)
        {
            double randomNo = rand_.NextDouble();

            if (randomNo > ((double)network_.Operations.Count / 2.0d) / (double)wantedNoOfNodes)
            {
                return CreateAddMutator();
            }
            else
            {
                return CreateRemoveMutator();
            }
        }

        private NetworkMutator CreateRemoveMutator()
        {
            Node nodeToRemove = GetRandomNotInputNode();

            int randomOutputNo = rand_.Next(nodeToRemove.Outputs.Count);
            Node output = nodeToRemove.Outputs.ElementAt(randomOutputNo);

            return new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,
                node: nodeToRemove,
                parent1: nodeToRemove.Inputs.ElementAt(0),
                parent2: nodeToRemove.Inputs.ElementAt(1),
                output: output,
                network: network_
            );
        }

        private NetworkMutator CreateAddMutator()
        {
            Node parent1 = GetRandomNode();
            while (parent1.Outputs.Count == 0)
            {
                parent1 = GetRandomNode();
            }

            Node parent2 = GetRandomNode();
            while (parent2 == parent1 || parent2.Outputs.Find(parent1) != null || parent2.Inputs.Find(parent1) != null)
            {
                //parent2 = GetRandomNode(); //on network creation it was hanging in here when found only operation node as parent1
                return CreateAddMutator(); 
            }

            int randomOutputNo = rand_.Next(parent1.Outputs.Count);
            Node output = parent1.Outputs.ElementAt(randomOutputNo);

            int newNodeHierarchy = Math.Max(parent1.Hierarchy, parent2.Hierarchy) + 1;
            if (output.Hierarchy <= newNodeHierarchy)
            {
                return CreateAddMutator();
            }

            return new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Add,
                node: new Node(nodeFactory_.GenerateRandomNodeType(), newNodeHierarchy),
                parent1: parent1,
                parent2: parent2,
                output: output,
                network: network_
            );
        }

        private Node GetRandomNotInputNode()
        {
            int randomNo = rand_.Next(network_.Operations.Count);

            return network_.Operations[randomNo];
        }
    }
}
