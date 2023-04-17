using System.ComponentModel;
using UnityEngine;

namespace Combat.Enums {
// 卡牌附带的五行元素
public enum ElementType {
    [Description("金")]
    Jin,

    [Description("木")]
    Mu,

    [Description("土")]
    Tu,

    [Description("水")]
    Shui,

    [Description("火")]
    Huo,
}

public static class ElementTypeFunc {
    private const int N = 5;

    public static Color MainColor(this ElementType? type) {
        return type switch {
            ElementType.Huo  => new Color(0.78f, 0.24f, 0.1f),
            ElementType.Shui => new Color(0.41f, 0.68f, 0.74f),
            ElementType.Mu   => new Color(0.34f, 0.76f, 0.53f),
            ElementType.Jin  => new Color(0.65f, 0.51f, 0.29f),
            ElementType.Tu   => new Color(0.43f, 0.27f, 0.18f),

            _ => new Color(0.38f, 0.39f, 0.42f),
        };
    }

    public static ElementType Next(this ElementType type) {
        var next = (int)type + 1;
        if (next >= N) next = 0;
        return (ElementType)next;
    }

    private static ElementType Prev(this ElementType type) {
        var prev = (int)type - 1;
        if (prev < 0) prev = N - 1;
        return (ElementType)prev;
    }

    // type1克制type2
    public static bool IsNext(this ElementType type1, ElementType type2) {
        return Next(type1) == type2;
    }

    // type1被type2克制
    public static bool IsPrev(this ElementType type1, ElementType type2) {
        return Prev(type1) == type2;
    }
}
}
