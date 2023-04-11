using System;

namespace Utils {
public static class GRandom {
    private static readonly Random Instance = new();

    public static int Range(int min, int max) {
        return Instance.Next(min, max);
    }
}
}
