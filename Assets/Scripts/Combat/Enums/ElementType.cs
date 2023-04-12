﻿using System.ComponentModel;

namespace Combat.Enums {
// 卡牌附带的五行元素
public enum ElementType {
    [Description("金")]
    Jin = 0,

    [Description("木")]
    Mu = 1,

    [Description("水")]
    Shui = 2,

    [Description("火")]
    Huo = 3,

    [Description("土")]
    Tu = 4,
}

public static class ElementTypeFunc {
    private const int N = 5;

    private static ElementType Next(this ElementType type) {
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
