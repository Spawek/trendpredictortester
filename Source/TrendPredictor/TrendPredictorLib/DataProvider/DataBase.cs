using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class DataBase
    {
        private Dictionary<string, List<double>> db = new Dictionary<string, List<double>>();
        private string dbName;

        public DataBase(string _dbName = "DB")
        {
            dbName = _dbName;
        }

        public DataBase(List<double> data, string dataName, string _dbName = "DB")
        {
            db[dataName] = data;
            dbName = _dbName;
        }

        public List<double> this[string key]
        {
            get
            {
                if (!db.ContainsKey(key))
                {
                    db[key] = new List<double>();
                }

                return db[key];
            }

            set { db[key] = value; }
        }

        public void PrintDBProperties()
        {
            Console.WriteLine("\n");
            Console.WriteLine("DB name: {0}", dbName);
            Console.WriteLine("DB elements no: {0}", CountVectorElements);
            Console.WriteLine("DB keys:");
            foreach (var key in db.Keys)
            {
                Console.WriteLine("\t{0}", key);
            }
        }

        /// <summary>
        /// joins databases
        /// </summary>
        /// <param name="database">databases cannot contain same keys!</param>
        /// <returns></returns>
        public static DataBase operator + (DataBase db1, DataBase db2)
        {
            DataBase newDb = new DataBase();

            foreach (var item in db1.db)
            {
                if (db2.db.ContainsKey(item.Key))
                    throw new ArgumentException("databases you want to add contains same key");

                newDb[item.Key].AddRange(item.Value);
            }

            foreach (var item in db2.db)
            {
                newDb[item.Key].AddRange(item.Value);
            }

            return newDb;
        }

        public int CountVectorElements{ get { return ValidateElementsNoInVectors(); } }

        public DataBase CloneDbRange(int beginIndex, int count)
        {
            int dataVectorSize = CountVectorElements;
            if (beginIndex < 0 || beginIndex >= dataVectorSize)
                throw new ArgumentOutOfRangeException("beginIndex");
            if(count < 1)
                throw new ArgumentOutOfRangeException("count");
            if (beginIndex + count > dataVectorSize)
                throw new ArgumentOutOfRangeException("beginIndex + count");

            DataBase dataBase = new DataBase();
            foreach (var item in db)
            {
                dataBase[item.Key] = item.Value.GetRange(beginIndex, count);
            }

            return dataBase;
        }

        //TODO: method should not throw exceptions as in it usuall workflow,
        //EXPLANATION: if anything goes wrong in data validation, it means that
        //             something went very wrong and program should stop it flow anyway
        //             throwing an exception gives better debug possibility than just 
        //             returning false (and some msg can be easily passed)
        public void ValidateDB() 
        {
            if (db.Count < 0)
                throw new ApplicationException("db is empty");

            ValidateElementsNoInVectors();
            ValidateElementsCorrectnessInVectors();
        }

        private void ValidateElementsCorrectnessInVectors()
        {
            foreach (var record in db)
            {
                foreach (var item in record.Value)
                {
                    if (Double.IsNaN(item) ||
                        Double.IsInfinity(item))
                    {
                        throw new ApplicationException(
                            String.Format("wrong value: {0} in doule value in vector: {1}",
                                item.ToString(),
                                record.Key));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>no of elements in each vector</returns>
        private int ValidateElementsNoInVectors()
        {
            //all vectors should have same no of elements
            int expectedElementsNo = db.First(x => true).Value.Count;

            foreach (var item in db)
            {
                if (item.Value.Count != expectedElementsNo)
                {
                    throw new ApplicationException(
                        String.Format("wrong no of elements in {0}: {1} instead of {2}",
                            item.Key,
                            item.Value.Count(),
                            expectedElementsNo));
                }
            }

            return expectedElementsNo;
        }
    }
}
