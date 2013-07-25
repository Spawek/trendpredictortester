using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class DataPointCutter
    {
        private int backTicksPerIndicator_;
        private int forwardStepsToPredict_;
        private string outputName_;
        private ISet<string> indicatorsToUseNames_;

        public DataPointCutter(int backTicksPerIndicator, ISet<string> indicatorsToUseNames, int forwardStepsToPredict, string outputName)
        {
            backTicksPerIndicator_ = backTicksPerIndicator;
            forwardStepsToPredict_ = forwardStepsToPredict;
            indicatorsToUseNames_ = new HashSet<string>(indicatorsToUseNames);
            outputName_ = outputName;
        }

        public DataPoint Cut(DataBase db, int offset)
        {
            int dataSize = db.CountVectorElements;
            if (dataSize < offset + forwardStepsToPredict_)
                throw new ArgumentOutOfRangeException();
            if (offset < Helpers.ModifiedFibbonacci(backTicksPerIndicator_ - 1))
                throw new ArgumentOutOfRangeException(String.Format(
                    "offset should be > {0}", 
                    Helpers.ModifiedFibbonacci(backTicksPerIndicator_ - 1).ToString()));

            DataPoint outputPoint = new DataPoint();

            //changing loops with their places would speed it up, but this possition will make output more understandable
            foreach(string currIndicator in indicatorsToUseNames_)
            {
                for (int i = 0; i < backTicksPerIndicator_; i++)
                {
                    int currIndex = offset - Helpers.ModifiedFibbonacci(i);

                    outputPoint.input.Add(db[currIndicator][currIndex]);
                }
            }

            outputPoint.output = db[outputName_][offset + forwardStepsToPredict_];

            return outputPoint;
        }
    }
}
