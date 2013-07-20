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
        static void Main(string[] args)
        {
            FileDataBaseProvider dbProvider = new FileDataBaseProvider("../../../../../Data/EURUSD.csv");
            DataBase inputDb = dbProvider.GetDataBase();
            inputDb.PrintDBProperties();

            DataBase indicatorsDb = new DataBase("indicators DB");
            indicatorsDb["openChangeInd"] = IndicatorsCalculator.CalculateChangeIndicator(inputDb["open"]);
            indicatorsDb["openChangeIndMAvg20"] = IndicatorsCalculator.CalculateMovingMean(indicatorsDb["openChangeInd"], 20);
            indicatorsDb["openChangeIndMAvg50"] = IndicatorsCalculator.CalculateMovingMean(indicatorsDb["openChangeInd"], 50);
            indicatorsDb.PrintDBProperties();

            var plotter = Plotter.GetInstance();
            plotter.AddPlot(indicatorsDb["openChangeInd"].GetRange(1000, 200), "openChangeInd");
            plotter.AddPlot(indicatorsDb["openChangeIndMAvg20"].GetRange(1000, 200), "openChangeIndMAvg20");
            plotter.AddPlot(indicatorsDb["openChangeIndMAvg50"].GetRange(1000, 200), "openChangeIndMAvg50");

            Console.WriteLine("Press any button");
            Console.ReadKey();
        }
    }
}
