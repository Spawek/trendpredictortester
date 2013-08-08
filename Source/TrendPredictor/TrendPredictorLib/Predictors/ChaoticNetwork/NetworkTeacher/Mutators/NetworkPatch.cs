using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public class NetworkPatch
    {
        public List<NetworkMutator> mutators_ = new List<NetworkMutator>();

        public void Apply()
        {
            for (int i = 0; i < mutators_.Count; i++)
            {
                mutators_[i].Apply();
            }
        }

        public void Revert()
        {
            for (int i = mutators_.Count - 1; i >= 0; i--)
            {
                mutators_[i].Revert();
            }
        }

    }
}
