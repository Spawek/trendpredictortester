using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    /// <summary>
    /// add/remove node
    /// </summary>
    public class AddRemoveNode : NetworkMutator
    {
        public enum NodeChangeType
        {
            Add,
            Remove
        }

        private NodeChangeType nodeChangeType_;
        private Node node_;
        private Node parent1_;
        private Node parent2_;
        private Node parent2NodeToInsertAfter_ = null; //it can't be LinkedListNode, coz it the "Node" can be removed in added in meantime so it wont be the same LinkedListNode, althouht this will still be the same node
        private List<Node> outputs_;
        private Network network_; //ouch! ... //TODO: rethink if it is really needed in here

        public string PrintPatch()
        {
            string patchPrint = String.Format("\nType: {0} \nNode: {1} \nParent1: {2} \nParent2: {3} \nOutputs:",
                nodeChangeType_.ToString(),
                node_.NodeID.ToString(),
                parent1_.NodeID.ToString(),
                parent2_.NodeID.ToString()
            );
            foreach (var output in outputs_)
            {
                patchPrint += String.Format("\n\t{0}", output.NodeID.ToString());
            }
            patchPrint += "\n";

            return patchPrint;
        }

        protected override void ApplyAction()
        {
#if DEBUG
            Logger.Log(this, String.Format("applying Patch: {0}", PrintPatch()));
#endif
            if (nodeChangeType_ == NodeChangeType.Add)
            {
                AddNode();
            }
            else
            {
                RemoveNode();
            }
        }

        protected override void RevertAction()
        {
#if DEBUG
            Logger.Log(this, String.Format("reverting Patch: {0}", PrintPatch()));
#endif
            if (nodeChangeType_ == NodeChangeType.Add)
            {
                RemoveNode();
            }
            else
            {
                AddNode();
            }
        }

        /// <summary>
        /// output is parent 1 output
        /// </summary>
        private void AddNode()
        {
            if (outputs_.Count == 0)
                throw new ApplicationException();

            foreach (Node output in outputs_)
            {
                if (parent1_.Outputs.Find(output) == null)
                    throw new ApplicationException("output is not parent 1 output");
            }

            node_.Inputs.AddLast(parent1_);
            node_.Inputs.AddLast(parent2_);

            bool nodeWasInjectedToParent = false;
            foreach (Node output in outputs_)
            {
                node_.Outputs.AddLast(output);

                //replace like this saves inputs/outputs order after apply/remove
                if (!nodeWasInjectedToParent)
                {
                    parent1_.Outputs.Find(output).Value = node_;
                    nodeWasInjectedToParent = true;
                }
                else
                {
                    if(!parent1_.Outputs.Remove(output))
                        throw new ApplicationException();
                }
                output.Inputs.Find(parent1_).Value = node_;
            }

            if (parent2NodeToInsertAfter_ != null)
            {
                parent2_.Outputs.AddAfter(parent2_.Outputs.Find(parent2NodeToInsertAfter_), node_);
            }
            else
            {
                parent2_.Outputs.AddFirst(node_);
            }

            network_.Operations.Add(node_);
        }

        private void RemoveNode()
        {
#if DEBUG
            if (parent2_.Outputs.Count == 1 && parent2_.Inputs.Count != 0) //parent2 has no more outputs than this node and parent2 is not network input //to prevent creating nodes without outputs
                throw new ApplicationException();

            if (node_.Inputs.Count != 2)
                throw new ApplicationException();

            if (outputs_.Count == 0)
                throw new ApplicationException();
#endif

            node_.Inputs.Clear();

            if (!parent2_.Outputs.Remove(node_))
                throw new ApplicationException();

            LinkedListNode<Node> nodeToAddAfter = parent1_.Outputs.Find(node_);
            foreach (Node output in node_.Outputs)
            {
                nodeToAddAfter = parent1_.Outputs.AddAfter(nodeToAddAfter, output);
                output.Inputs.Find(node_).Value = parent1_;
            }
            if (!parent1_.Outputs.Remove(node_))
                throw new ApplicationException();

            node_.Outputs.Clear();

            if (!network_.Operations.Remove(node_))
                throw new ApplicationException();
        }

        public AddRemoveNode(NodeChangeType nodeChangeType, Node node, Node parent1, Node parent2, List<Node> outputs, Network network)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (parent1 == null)
                throw new ArgumentNullException("parent1");
            if (parent2 == null)
                throw new ArgumentNullException("parent2");
            if (outputs == null)
                throw new ArgumentNullException("output");
            //if (node == parent1 || node == parent2 || node == outputs || //TODO: FIX THIS checker
            //    parent1 == parent2 || parent1 == outputs || parent2 == outputs)
            //    throw new ArgumentException();

            nodeChangeType_ = nodeChangeType;
            node_ = node;
            parent1_ = parent1;
            parent2_ = parent2;
            outputs_ = outputs;
            network_ = network;
            
            // NODE: it is only way I was able to create to make parent 2 children stay in same order after revert
            parent2NodeToInsertAfter_ = null;
            if (nodeChangeType == NodeChangeType.Remove)
            {
                LinkedListNode<Node> parent2LinkedListNodeToInsertAfter = parent2_.Outputs.Find(node_).Previous;
                if (parent2LinkedListNodeToInsertAfter != null)
                {
                    parent2NodeToInsertAfter_ = parent2_.Outputs.Find(node_).Previous.Value;
                }
            }
        }
    }
}
