using System.Collections.Generic;
using System;

namespace DreamLog.Tools
{
    public static class LinqUtils
    {
        public static int IndexOf<T>(this IEnumerable<T> collection, Predicate<T> comparator)
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            int currentIndex = 0;
            int foundIndex = -1;
            while(enumerator.MoveNext() && foundIndex == -1)
            {
                if(comparator(enumerator.Current))
                {
                    foundIndex = currentIndex;
                }
                currentIndex++;
            }
            return foundIndex;
        }
    }
}
