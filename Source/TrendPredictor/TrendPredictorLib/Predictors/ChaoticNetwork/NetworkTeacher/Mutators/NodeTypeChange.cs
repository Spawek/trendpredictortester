using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class NodeTypeChange : NetworkMutator
    {
        Node node_;
        NodeType fromType_;
        NodeType toType_;

        protected override void ApplyAction()
        {
            if (node_.NodeType != fromType_)
                throw new ApplicationException();

            node_.Transform(toType_);
        }

        protected override void RevertAction()
        {
            if (node_.NodeType != toType_)
                throw new ApplicationException();

            node_.Transform(fromType_);
        }

        public NodeTypeChange(Node node, NodeType fromType, NodeType toType)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (fromType == null)
                throw new ArgumentNullException("fromType");
            if (toType == null)
                throw new ArgumentNullException("toType");

            node_ = node;
            fromType_ = fromType;
            toType_ = toType;
        }
    }
}
