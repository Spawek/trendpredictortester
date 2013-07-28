using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public enum NodeTypes
    {
        add, // x + y
        multiply, // x*y
        compare, // x > y ? 1 : 0
        inverse,  // 1/x
        reverse, // -x
        log, // x > 0 ? log(x) : log(-x)
        copy, // x
    }
}
