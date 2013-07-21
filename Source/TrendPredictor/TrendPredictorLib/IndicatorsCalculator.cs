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
		public static List<double> CalcChangeIndicator(List<double> inputVector)
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
		public static List<double> CalcMovingMean(List<double> inputVector, int backstepsTaken)
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
        public static List<double> CalcPercentRIndicator
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
        public static List<double> CalcPercentRIndicatorModified
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

        /// <summary>
        /// it looks almost exactly like "close" - it doesnt seem that it is very useful
        /// 
        /// http://bossa.pl/edukacja/AT/wskazniki/WilliamsAccumulationDistribution/
        /// Jeżeli C > C [-1] to A/D = C - (MinL (C [-1]; L)
        /// Jeżeli C < C [-1] to A/D = C - (MaxL (C [-1]; H)
        /// Jeżeli C = C [-1] to A/D = 0
        /// A/D Williamsa = A/D + A/D Williamsa [-1]
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="close"></param>
        /// <returns></returns>
        public static List<double> CalcWilliamsAccumulationDistributionIndicator
            (List<double> high, List<double> low, List<double> close)
        {
            if (high.Count != low.Count || low.Count != close.Count)
                throw new ArgumentException("vectors are not of equal size!");
            int dataSize = high.Count;
            if (dataSize < 2)
                throw new ArgumentException("data size is smaller than 2");

            List<double> output = new List<double>();

            output.Add(0.0d);
            
            double ADWilliams = 0.0;
            for (int i = 1; i < dataSize; i++)
            {
                double AD;
                if(close[i] > close[i-1])
                {
                    AD = close[i] - Math.Min(close[i - 1], low[i]);
                }
                else if (close[i] < close[i - 1])
                {
                    AD = close[i] - Math.Max(close[i - 1], high[i]);
                }
                else //close[i] == close[i-1]
                {
                    AD = 0;
                }

                ADWilliams += AD;
                output.Add(ADWilliams);
            }

            return output;
        }

        /// <summary>
        ///http://bossa.pl/edukacja/AT/wskazniki/VolatilityChaikins/
        ///EMA = Exponental Moving Average (modified version)
        ///CV=(EMA(H-L,n) - EMA(H-L,n)_[-n])/(EMA(H-L,n)_[-n])
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static List<double> CalcChaikinsVolatilityIndex(List<double> high, List<double> low, int n)
        {
            if (n < 2)
                throw new ArgumentOutOfRangeException("n");
            if (high.Count != low.Count)
                throw new ArgumentException("vectors are not of equal size!");
            int dataSize = high.Count;
            if (dataSize < n*2)
                throw new ArgumentException("data size is smaller than n * 2");

            List<double> HMinusL = CalcAMinusB(high, low);
            List<double> HMinusLMHA = CalcExponentalMovingAverageModified(HMinusL, n);
            
            List<double> output = new List<double>();

            for (int i = 0; i < n; i++)
            {
                output.Add(double.NaN);
            }

            for (int i = n; i < dataSize; i++)
            {
                double EmaHMinusL = HMinusLMHA[i];
                double EmaHMinusLBackByN = HMinusLMHA[i-n];
                output.Add((EmaHMinusL - EmaHMinusLBackByN) / EmaHMinusLBackByN);
            }

            return output;
        }

        public static List<double> CalcAMinusB(List<double> a, List<double> b)
        {
            if (a.Count != b.Count)
                throw new ArgumentException("a size != b size");
            if (a.Count < 2)
                throw new ArgumentException("a size is lesser than 2");

            List<double> output = new List<double>(a.Count);

            for (int i = 0; i < a.Count; i++)
            {
                output.Add(a[i] - b[i]);
            }

            return output;
        }

        /// <summary>
        ///http://en.wikipedia.org/wiki/Moving_average_(finance)#Exponential_moving_average
        /// </summary>
        /// <param name="input"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static List<double> CalcExponentalMovingAverage(List<double> input, double alpha)
        {
            if (input.Count < 2)
                throw new ArgumentException("input size is lesser than 2");
            if (alpha <= 0.0d || alpha >= 1.0d)
                throw new ArgumentException("alpha is not in (0,1) range");

            double currAvg = input[0];

            List<double> output = new List<double>();
            output.Add(currAvg);
            for (int i = 1; i < input.Count; i++)
            {
                currAvg *= (1 - alpha);
                currAvg += alpha * input[i];

                output.Add(currAvg);
            }

            return output;
        }

        /// <summary>
        ///http://en.wikipedia.org/wiki/Moving_average_(finance)#Modified_moving_average
        /// </summary>
        /// <param name="input"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static List<double> CalcExponentalMovingAverageModified(List<double> input, int n)
        {
            if (input.Count < n)
                throw new ArgumentException("input size is lesser than 2");
            if (n < 2)
                throw new ArgumentException("n < 2");

            double alpha = 1.0d / (double)n;

            return CalcExponentalMovingAverage(input, alpha);
        }

	}
}
