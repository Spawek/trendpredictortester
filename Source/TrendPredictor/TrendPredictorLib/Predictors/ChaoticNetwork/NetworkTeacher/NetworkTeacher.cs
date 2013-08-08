using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
    public class NetworkTeacher
    {
        private Network network_;
        private PatchFactory patchFactory_;

        public NetworkTeacher(Network network, PatchFactory patchFactory)
        {
            network_ = network;
            patchFactory_ = patchFactory;
        }

        public void Teach(TimeSpan teachingTime, int wantedNoOfNodes, int changesPerPatch, int patchesPerTeachingSerie)
        {
            DateTime startTime = DateTime.Now;

            int trainingSessionNo = 1;
            while (DateTime.Now - startTime < teachingTime)
            {
                PerformTeachingSerie(wantedNoOfNodes, changesPerPatch, patchesPerTeachingSerie);
                Logger.Log(this, 
                    String.Format("Error after {0} training session: {1}. (network operations: {2})", 
                    trainingSessionNo, 
                    network_.CalculateTrainingSqrError(), 
                    network_.Operations.Count)
                );
                trainingSessionNo++;
            }
        }

        private void PerformTeachingSerie(int wantedNoOfNodes, int changesPerPatch, int patchesPerTeachingSerie)
        {
            // patch --> its error change - lower = better
            Dictionary<NetworkPatch, double> patchesEfficiency = new Dictionary<NetworkPatch, double>();
            double serieStartError = network_.CalculateTrainingSqrError();

            while (patchesEfficiency.Count < patchesPerTeachingSerie)
            {
                NetworkPatch patch = patchFactory_.CreateAndApplyPatch(changesPerPatch, wantedNoOfNodes);
                patchesEfficiency[patch] = network_.CalculateTrainingSqrError() - serieStartError;
                patch.Revert();
            }

            double bestChange = patchesEfficiency.Min(x => x.Value);
            NetworkPatch bestPatch = patchesEfficiency.First(patch => patch.Value == bestChange).Key;
            if(!double.IsInfinity(patchesEfficiency[bestPatch]) || !double.IsNaN(patchesEfficiency[bestPatch]))
            {
                bestPatch.Apply();
            }
        }
    }
}
