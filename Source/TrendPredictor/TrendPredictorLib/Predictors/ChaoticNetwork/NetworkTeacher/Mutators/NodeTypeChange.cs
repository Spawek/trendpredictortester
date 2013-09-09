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

        public string PrintPatch()
        {
            return String.Format("nodeID: {0}\nfrom type: {1}\nto type: {2}",
                node_.NodeID.ToString(),
                fromType_.ToString(),
                toType_.ToString());
        }

        protected override void ApplyAction()
        {
            if (node_.NodeType != fromType_)
                throw new ApplicationException();
#if DEBUG
            Logger.Log(this, String.Format("Applying NodeTypeChange:\n{0}", PrintPatch()));
#endif
            node_.Transform(toType_);
        }

        protected override void RevertAction()
        {
            if (node_.NodeType != toType_)
                throw new ApplicationException();
#if DEBUG
            Logger.Log(this, String.Format("Reverting NodeTypeChange:\n{0}", PrintPatch()));
#endif
            node_.Transform(fromType_);
        }

        public NodeTypeChange(Node node, NodeType fromType, NodeType toType)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            node_ = node;
            fromType_ = fromType;
            toType_ = toType;
        }
    }
}
