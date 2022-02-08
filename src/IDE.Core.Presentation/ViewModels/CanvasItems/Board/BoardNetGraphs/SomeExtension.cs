using System;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Designers
{
    public static class SomeExtension
    {
        public static IEnumerable<TSource> Distinct<TSource, TCompare>(this IEnumerable<TSource> source, Func<TSource, TCompare> selector)
        {
            return source.Distinct(new LambdaEqualityComparer<TSource, TCompare>(selector));
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }


    }
}
