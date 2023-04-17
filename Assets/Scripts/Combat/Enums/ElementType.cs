using System;
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

    public static Color MainColor(this ElementType type) {
        return type switch {
            ElementType.Huo  => new Color(0.86f, 0.24f, 0.2f),
            ElementType.Shui => new Color(0.14f, 0.59f, 0.76f),
            ElementType.Mu   => new Color(0.45f, 0.59f, 0.16f),
            ElementType.Jin  => new Color(0.76f, 0.61f, 0.14f),
            ElementType.Tu   => new Color(0.46f, 0.34f, 0.28f),

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static Color MainColor(this ElementType? type) {
        return type switch {
            null => new Color(0.38f, 0.39f, 0.42f),
            _    => type.Value.MainColor(),
        };
    }

    private static int TextIndex(this ElementType type) {
        return type switch {
            ElementType.Jin  => 0,
            ElementType.Mu   => 1,
            ElementType.Shui => 2,
            ElementType.Huo  => 3,
            ElementType.Tu   => 4,

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static int TextIndex(this ElementType? type) {
        return type switch {
            null => 5,
            _    => type.Value.TextIndex(),
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
