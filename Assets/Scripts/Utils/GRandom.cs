using System;
using System.Collections.Generic;

namespace Utils {
// 随机数单例
public static class GRandom {
    private static readonly Random Instance = new();

    public static int Range(int min, int max) {
        return Instance.Next(min, max);
    }

    public static void Shuffle<T>(IList<T> list) {
        var n = list.Count;
        while (n > 1) {
            n--;
            var k = Range(0, n);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
}
