using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class NodeFuncGenerator
    {
        public Func<List<double>, double> GenerateFunc(NodeTypes type)
        {
            switch (type)
            {
                case NodeTypes.add:
                    return Add;

                case NodeTypes.multiply:
                    return Multiply;

                case NodeTypes.compare:
                    return Compare;

                case NodeTypes.inverse:
                    return Inverse;

                case NodeTypes.reverse:
                    return Reverse;

                case NodeTypes.log:
                    return Log;

                case NodeTypes.copy:
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
