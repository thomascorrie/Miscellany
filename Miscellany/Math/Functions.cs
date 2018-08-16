using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miscellany.Math
{
    public static class Functions
    {
        //converts double to decimal
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

        //converts decimal to double
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
    }
}