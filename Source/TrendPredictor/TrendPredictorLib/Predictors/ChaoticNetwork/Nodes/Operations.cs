using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public interface Operation : Node
    {
        bool Calculate();
    }
}
