using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class NodeFuncGenerator
    {
        public static Func<List<double>, double> GenerateFunc(NodeType type)
        {
            switch (type)
            {
                case NodeType.add:
                    return Add;

                case NodeType.multiply:
                    return Multiply;

                case NodeType.compare:
                    return Compare;

                case NodeType.inverse:
                    return Inverse;

                case NodeType.reverse:
                    return Reverse;

                case NodeType.log:
                    return Log;

                case NodeType.copy:
                    return Copy;

                default:
                    throw new NotImplementedException();
            }
        }

        //static for optimization
        private static double Add(List<Double> args)
        {
            return args[0] + args[1];
        }

        private static double Multiply(List<Double> args)
        {
            return args[0] * args[1];
        }

        private static double Compare(List<Double> args)
        {
            return args[0] < args[1] ? 1.0d : 0.0d;
        }

        private static double Inverse(List<Double> args)
        {
            return 1.0d/args[0];
        }

        private static double Reverse(List<Double> args)
        {
            return -args[0];
        }

        private static double Log(List<Double> args)
        {
            return args[0] > 0.0d ? Math.Log(args[0]) : Math.Log(-args[0]);
        }

        private static double Copy(List<Double> args)
        {
            return args[0];
        }
    }
}
