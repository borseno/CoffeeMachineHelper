using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int columns)
        {
            var result = new List<List<T>>();

            int count = 0;
            var currentList = new List<T>(columns);
            foreach (var i in enumerable)
            {
                if (count == columns)
                {
                    result.Add(currentList);
                    currentList = new List<T>(columns);
                    count = 0;
                }
                currentList.Add(i);
                count++;
            }

            result.Add(currentList);

            return result;
        } 
    }
}
