using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    /// <summary>
    /// network should be the same after applying and reverting mutator
    /// </summary>
    public abstract class NetworkMutator
    {
        private bool wasApplied = false;
        private bool wasReverted = false;

        public void Apply()
        {
            if (wasApplied)
                throw new ApplicationException();

            ApplyAction();

            wasApplied = true;
        }

        public void Revert()
        {
            if (!wasApplied)
                throw new ApplicationException();
            if (wasReverted)
                throw new ApplicationException();

            RevertAction();

            wasReverted = true;
        }

        protected abstract void ApplyAction();
        protected abstract void RevertAction();
    }
}
