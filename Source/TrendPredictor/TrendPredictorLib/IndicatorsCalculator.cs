using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrendPredictorLib
{
	public static class IndicatorsCalculator
	{
		/// <summary>
		/// its like change in percents, but here are no percents (no *100)
		/// </summary>
		public static List<double> CalculateChangeIndicator(List<double> inputVector)
		{
			if (inputVector.Count < 2)
				throw new ArgumentException("input vector is too small");

			List<double> output = new List<double>();
			output.Add(double.NaN); //1st field cannot be calculated

			for (int i = 1; i < inputVector.Count; i++)
			{
				double valueChange = inputVector[i] - inputVector[i-1];
				output.Add(valueChange / inputVector[i-1]);
			}

			return output;
		}
	
		/// <summary</summary>
		/// 
        /// <param name="inputVector"></param>
		/// <param name="backstepsTaken">it includes current point, 1 would be just vector copy</param>
		/// <returns></returns>
		public static List<double> CalculateMovingMean(List<double> inputVector, int backstepsTaken)
		{
			if (backstepsTaken < 2)
				throw new ArgumentOutOfRangeException("backstepsTaken");
			if (inputVector.Count < backstepsTaken)
				throw new ArgumentException("input vector is too small");

			List<double> output = new List<double>();

			for (int i = 0; i < backstepsTaken - 1; i++)
			{
				output.Add(double.NaN);
			}

			for (int i = backstepsTaken - 1; i < inputVector.Count; i++)
			{
				output.Add(inputVector.GetRange(i - backstepsTaken + 1, backstepsTaken).Sum()/backstepsTaken);
			}

			return output;
		}

        /// <summary>
        /// http://bossa.pl/edukacja/AT/wskazniki/WilliamsR/
        /// %R = (MAX(H,n) - C)/(MAX(H,n) - MIN(L,n))
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="close"></param>
        /// <returns></returns>
        public static List<double> CalcaulatePercentRIndicator
            (List<double> high, List<double> low, List<double> close, int n)
        {
            if (n < 2)
                throw new ArgumentOutOfRangeException("n");
            if (high.Count != low.Count || low.Count != close.Count)
                throw new ArgumentException("vectors are not of equal size!");
            int dataSize = high.Count;
            if (dataSize < n)
                throw new ArgumentException("data size is smaller than n");

            List<double> output = new List<double>();
            for (int i = 0; i < n - 1; i++)
            {
                output.Add(double.NaN);
            }

            for (int i = n - 1; i < dataSize; i++)
            {
                double maxH = high.GetRange(i - n + 1, n).Max();
                double minL = low.GetRange(i - n + 1, n).Min();
                double C = close[i];

                output.Add((maxH - C) / (maxH - minL));
            }

            return output;
        }

        /// <summary>
        /// http://bossa.pl/edukacja/AT/wskazniki/WilliamsR/
        /// %R = (C - MIN(L,n))/(MAX(H,n) - MIN(L,n))
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="close"></param>
        /// <returns></returns>
        public static List<double> CalcaulatePercentRIndicatorModified
            (List<double> high, List<double> low, List<double> close, int n)
        {
            if (n < 2)
                throw new ArgumentOutOfRangeException("n");
            if (high.Count != low.Count || low.Count != close.Count)
                throw new ArgumentException("vectors are not of equal size!");
            int dataSize = high.Count;
            if (dataSize < n)
                throw new ArgumentException("data size is smaller than n");

            List<double> output = new List<double>();
            for (int i = 0; i < n - 1; i++)
            {
                output.Add(double.NaN);
            }

            for (int i = n - 1; i < dataSize; i++)
            {
                double maxH = high.GetRange(i - n + 1, n).Max();
                double minL = low.GetRange(i - n + 1, n).Min();
                double C = close[i];

                output.Add((C - minL) / (maxH - minL));
            }

            return output;

        }
	}
}
