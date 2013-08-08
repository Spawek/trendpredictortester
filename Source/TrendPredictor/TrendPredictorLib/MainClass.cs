using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsPlotter;

namespace TrendPredictorLib
{
    class MainClass
    {
        static void OldMain(string[] args)
        {
            FileDataBaseProvider dbProvider = new FileDataBaseProvider("../../../../../Data/EURUSD.csv");
            DataBase inputDb = dbProvider.GetDataBase();
            inputDb.PrintDBProperties();

            DataBase indicatorsDb = new DataBase("indicators DB");
            indicatorsDb["openChangeInd"] = IndicatorsCalculator.CalcChangeIndicator(inputDb["open"]);
            indicatorsDb["openChangeIndMAvg20"] = IndicatorsCalculator.CalcMovingMean(indicatorsDb["openChangeInd"], 20);
            indicatorsDb["openChangeIndMAvg50"] = IndicatorsCalculator.CalcMovingMean(indicatorsDb["openChangeInd"], 50);
            indicatorsDb["%RInd10"] = IndicatorsCalculator.CalcPercentRIndicator(inputDb["high"],inputDb["low"], inputDb["close"], 10);
            indicatorsDb["%RInd20"] = IndicatorsCalculator.CalcPercentRIndicator(inputDb["high"], inputDb["low"], inputDb["close"], 20);
            indicatorsDb["%RInd10Mod"] = IndicatorsCalculator.CalcPercentRIndicatorModified(inputDb["high"], inputDb["low"], inputDb["close"], 10);
            indicatorsDb["%RInd20Mod"] = IndicatorsCalculator.CalcPercentRIndicatorModified(inputDb["high"], inputDb["low"], inputDb["close"], 20);
            indicatorsDb["WilliamsAccDistr"] = IndicatorsCalculator.CalcWilliamsAccumulationDistributionIndicator(inputDb["high"], inputDb["low"], inputDb["close"]);
            indicatorsDb.PrintDBProperties();

            var plotter = Plotter.GetInstance();
            plotter.AddPlot(inputDb["close"].GetRange(1000, 200), "close");
            //plotter.AddPlot(indicatorsDb["openChangeInd"].GetRange(1000, 500), "openChangeInd");
            //plotter.AddPlot(indicatorsDb["openChangeIndMAvg20"].GetRange(1000, 500), "openChangeIndMAvg20");
            //plotter.AddPlot(indicatorsDb["openChangeIndMAvg50"].GetRange(1000, 500), "openChangeIndMAvg50");
            //plotter.AddPlot(indicatorsDb["%RInd10"].GetRange(1000, 500), "RInd10");
            //plotter.AddPlot(indicatorsDb["%RInd20"].GetRange(1000, 500), "RInd20");
            plotter.AddPlot(indicatorsDb["WilliamsAccDistr"].GetRange(1000, 200), "WilliamsAccDistr");


            Console.WriteLine("Press any button");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            TeacherTester teacherTester = new TeacherTester();
        }
    }
}
