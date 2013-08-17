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

            //NOTE: what is interesting double.NaN is lowest possible number AND double.NaN != double.NaN //WTF??
            double bestChange = patchesEfficiency.Min(x => double.IsNaN(x.Value) ? double.PositiveInfinity : x.Value);
            Logger.Log(this, String.Format("best change {0}", bestChange));
            if (bestChange < 0.0d && //IMPORTANT: for now only
                !double.IsInfinity(bestChange) &&
                !double.IsNaN(bestChange)) 
            {
                Logger.Log(this, "applying patch!");
                NetworkPatch bestPatch = patchesEfficiency.First(patch => patch.Value == bestChange).Key;
                bestPatch.Apply();
                double errorAfterApplyingPatch = network_.CalculateTrainingSqrError();
                Logger.Log(this, String.Format("error after appplying patch {0}", errorAfterApplyingPatch));
                double delta = 1E-4;
                if (serieStartError + bestChange - errorAfterApplyingPatch > delta)
                    throw new ApplicationException("something went rly bad");
            }
            else
            {
                Logger.Log(this, "NOT applying patch");
                double errorAfterNotApplyingPatch = network_.CalculateTrainingSqrError();
                if (serieStartError != errorAfterNotApplyingPatch)
                    throw new ApplicationException("revert went wrong");
            }
        }
    }
}
