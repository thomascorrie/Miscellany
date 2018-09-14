using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miscellany.Math
{
    /// <summary>
    /// Functions
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Converts a double to a decimal
        /// </summary>
        /// <param name="number">Input integer or double</param>
        /// <returns name="decimal">Decimal</returns>
        /// <search>
        /// decimal
        /// </search>
        public static decimal ToDecimal(double number)
        {
            if (number < (double)decimal.MinValue)
                return decimal.MinValue;
            else if (number > (double)decimal.MaxValue)
                return decimal.MaxValue;
            else
                return (decimal)number;
        }
        
        /// <summary>
        /// Converts a decimal to a double
        /// </summary>
        /// <param name="inputDecimal">Input decimal</param>
        /// <returns name="double">Double</returns>
        /// <search>
        /// decimal
        /// </search>
        public static double ToDouble(decimal inputDecimal)
        {
            return decimal.ToDouble(inputDecimal);
        }
        
        /// <summary>
        /// Calculates a running total for a list of numbers
        /// </summary>
        /// <param name="list">List of doubles</param>
        /// <returns name="list">List of running totals</returns>
        /// <search>
        /// sum
        /// </search>
        public static IList<double> RunningTotal(IList<double> list)
        {
            IList<double> sum = new List<double>();
            double runningTotal = 0;
            foreach (double d in list)
            {
                runningTotal += d;
                sum.Add(runningTotal);
            }
            return sum;
        }
    }
}