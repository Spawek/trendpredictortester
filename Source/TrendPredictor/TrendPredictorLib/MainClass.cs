using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    class MainClass
    {
        static void Main(string[] args)
        {
            FileDataBaseProvider dbProvider = new FileDataBaseProvider("../../../../../Data/EURUSD.csv");
            var db = dbProvider.GetDataBase();
            db.PrintDBProperties();
            db += new DataBase(IndicatorsCalculator.CalculateChangeIndicator(db["open"]), "openChangeInd");
            db.PrintDBProperties();


            Console.WriteLine("Press any button");
            Console.ReadKey();
        }
    }
}
