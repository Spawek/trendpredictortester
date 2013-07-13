using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class DataBase
    {
        private Dictionary<string, List<double>> db;

        public DataBase()
        {
            db = new Dictionary<string, List<double>>();
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

        public int CountVectorElements
        { 
            get 
            { 
                return ValidateElementsNoInVectors(); 
            } 
        }

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
        /// 
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
