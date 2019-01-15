using System.Collections.Generic;
using System.Collections; //Need this to handle non-specific IList as opposed to IList<T>

namespace Miscellany
{
    /// <summary>
    /// Modify
    /// </summary>
    public static class Lists
    {
        /// <summary>
        ///     Pairs items in a list: [a,b,c,d] > [[a,b],[b,c],[c,d]]
        /// </summary>
        /// <param name="list">List of items to pair</param>
        /// <returns name="list">Nested list of paired items</returns>
        /// <search>
        /// list, pair
        /// </search>
        public static IList PairItems(IList list)
        {
            List<List<object>> newList = new List<List<object>>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                List<object> subList = new List<object>();
                subList.Add(list[i]);
                subList.Add(list[i + 1]);
                newList.Add(subList);
            }
            return newList;
        }
    }
}
