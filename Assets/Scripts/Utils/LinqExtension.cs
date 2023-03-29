using System;
using System.Collections.Generic;

namespace Utils {
public static class LinqExtension {
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (var item in source) {
            action(item);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
        var i = 0;
        foreach (var item in source) {
            action(item, i);
            i++;
        }
    }
}
}
