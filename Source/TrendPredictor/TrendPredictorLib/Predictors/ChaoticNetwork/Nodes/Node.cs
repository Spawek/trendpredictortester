using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public interface Node
    {
        List<Node> Outputs
        {
            get;
            set;
        }

        bool Ready
        {
            get;
        }

        double Value
        {
            get;
        }
    }
}
