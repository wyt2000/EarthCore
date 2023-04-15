using System;

namespace Utils {
public static class GAlgorithm {
    public static int LowerBound<T>(T[] array, T value) where T : IComparable<T> {
        var index = Array.BinarySearch(array, value);
        if (index < 0) index = ~index;
        return index;
    } 
}
}
