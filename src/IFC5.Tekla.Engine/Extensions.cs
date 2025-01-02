using System;
using System.Collections.Generic;
using System.Linq;

namespace IFC5.Reader;
public static class EnumerableExtensions
{
    public static bool None<TSource>(this IEnumerable<TSource> source,
                                     Func<TSource, bool> predicate)
    {
        return !source.Any(predicate);
    }
}