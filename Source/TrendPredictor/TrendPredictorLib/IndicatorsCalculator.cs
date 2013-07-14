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
	
		/// <summary>
		/// 
		/// </summary>
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
	}
}
