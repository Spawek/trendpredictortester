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
        private Node output_;
        private Network network_; //ouch! ... //TODO: rethink if it is really needed in here

        protected override void ApplyAction()
        {
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
            if (parent1_.Outputs.Find(output_) == null)
                throw new ApplicationException("output is not parent 1 output");

            node_.Inputs.AddLast(parent1_);
            node_.Inputs.AddLast(parent2_);
            node_.Outputs.AddLast(output_);

            //replace like this saves inputs/outputs order after apply/remove
            parent1_.Outputs.Find(output_).Value = node_;

            parent2_.Outputs.AddLast(node_);

            output_.Inputs.Find(parent1_).Value = node_;

            network_.Operations.Add(node_);
        }

        private void RemoveNode()
        {
            if (node_.Inputs.Count != 2)
                throw new ApplicationException();

            node_.Inputs.Clear();

            parent1_.Outputs.Find(node_).Value = output_;

            if (!parent2_.Outputs.Remove(node_))
                throw new ApplicationException();

            foreach (Node output in node_.Outputs)
            {
                output.Inputs.Find(node_).Value = parent1_;
            }
            node_.Outputs.Clear();

            if (!network_.Operations.Remove(node_))
                throw new ApplicationException();
        }

        public AddRemoveNode(NodeChangeType nodeChangeType, Node node, Node parent1, Node parent2, Node output, Network network)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (parent1 == null)
                throw new ArgumentNullException("parent1");
            if (parent2 == null)
                throw new ArgumentNullException("parent2");
            if (output == null)
                throw new ArgumentNullException("output");
            if (node == parent1 || node == parent2 || node == output ||
                parent1 == parent2 || parent1 == output || parent2 == output)
                throw new ArgumentException();

            nodeChangeType_ = nodeChangeType;
            node_ = node;
            parent1_ = parent1;
            parent2_ = parent2;
            output_ = output;
            network_ = network;
        }
    }
}
