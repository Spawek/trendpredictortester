using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public class Input : Node
    {
        public List<Node> Outputs
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Ready
        {
            get { throw new NotImplementedException(); }
        }

        public double Value
        {
            get { throw new NotImplementedException(); }
        }
    }
}
