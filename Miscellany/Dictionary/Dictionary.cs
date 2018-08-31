using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;

namespace Miscellany
{
    public static class Dictionary
    {
        /// <summary>
        /// Similar to the builtin node to obtain the value at a specified key and return null where the key does not exist but this does so quietly. Needs refinement to handle different data structures.
        /// </summary>
        /// <param name="dictionary">DesignScript.BuiltIn.Dictionary</param>
        /// <param name="key">The key in the Dictionary to obtain</param>
        /// <returns name="value">The value of the specified key or null if it is not set</returns>
        /// <search>
        /// decimal
        /// </search>
        static public object ValueAtKey([ArbitraryDimensionArrayImport] System.Collections.IDictionary dictionary, string key)
        {
            if (dictionary.Contains(key) == true)
            {
                return dictionary[key];
            }
            else
            {
                return null;
            }
        }
    }
}
