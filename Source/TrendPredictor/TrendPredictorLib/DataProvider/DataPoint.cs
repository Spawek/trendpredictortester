using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class DataPoint
    {
        public List<double> input;
        public double output;

        public DataPoint()
        {
            input = new List<double>();
        }

        public DataPoint(List<double> _input, double _output)
        {
            input = _input;
            output = _output;
        }
    }
}
