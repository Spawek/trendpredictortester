using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class FileDataBaseProvider : DataBaseProvider
    {
        private string filePath;
        private DataBase db = null;
        private NumberFormatInfo doubleFormatProvider = new NumberFormatInfo();

        public FileDataBaseProvider(string _filePath)
        {
            filePath = _filePath;

            doubleFormatProvider.NumberDecimalSeparator = ".";
        }

        public DataBase GetDataBase()
        {
            if (db != null)
                return db;

            db = new DataBase("DB from File");
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line = sr.ReadLine(); //1st line is not needed (for now)
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splittedLine = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedLine[0] == "5") //just take 5 - min data
                    {
                        db["open"].Add(Convert.ToDouble(splittedLine[4], doubleFormatProvider));
                        db["high"].Add(Convert.ToDouble(splittedLine[2], doubleFormatProvider));
                        db["low"].Add(Convert.ToDouble(splittedLine[3], doubleFormatProvider));
                        db["close"].Add(Convert.ToDouble(splittedLine[5], doubleFormatProvider));
                    }

                }
            }

            return db;
        }
    }

}
