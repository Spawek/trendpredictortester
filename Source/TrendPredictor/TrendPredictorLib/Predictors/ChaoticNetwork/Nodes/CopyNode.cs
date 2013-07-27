using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public class CopyNode : Node
    {
        public CopyNode()
            : base(1)
        {

        }

        protected override double Calculate(List<double> args)
        {
            throw new NotImplementedException();
        }
    }
}
