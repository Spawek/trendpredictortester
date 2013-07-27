using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    // IMPORTANT: NOTE: it works on assumption that parent order is not important
    public abstract class Node
    {
        private int parentsNo_;
        private List<double> arguments = new List<double>(2);

        public List<Node> Inputs = new List<Node>(2);
        public List<Node> Outputs = new List<Node>();

        public double Value { get; private set; }

        public Node(int parentsNo)
        {
            parentsNo_ = parentsNo;
        }

        public void AddOutput(Node output)
        {
            if (output.parentsNo_ == Inputs.Count)
                throw new ApplicationException("output node already has all it inputs");

            Outputs.Add(output);
            output.Inputs.Add(this);
        }

        public void Reset()
        {
            arguments.Clear();
        }

        public void ProvideArgument(double arg)
        {
#if DEBUG
            if (arguments.Count == parentsNo_)
                throw new ArgumentException("too many arguments");
#endif 
            arguments.Add(arg);

            if (arguments.Count == parentsNo_)
            {
                Value = Calculate(arguments);
                foreach (var item in Outputs)
                {
                    item.ProvideArgument(arg);
                }
            }
        }

        abstract protected double Calculate(List<double> args);
    }
}
