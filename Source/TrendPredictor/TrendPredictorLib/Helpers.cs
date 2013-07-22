using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendPredictorLib
{
    public class Helpers
    {
        private static List<int> ModFibbonacciDb = new List<int>() { 0, 1, 2 };
        /// <summary>
        /// Fibbonacci starting with 0,1,2
        /// this foo is optimized to calc this only once
        /// </summary>
        /// <param name="index">index > 0</param>
        /// <returns>0,1,2,3,5,8,13,21,...</returns>
        public static int ModifiedFibbonacci(int index)
        {
            for (int i = ModFibbonacciDb.Count; i <= index; i++)
            {
                ModFibbonacciDb.Add(ModFibbonacciDb[i-1] + ModFibbonacciDb[i-2]);
            }

            return ModFibbonacciDb[index];
        }
    }
}
