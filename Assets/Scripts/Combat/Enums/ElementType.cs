using System.ComponentModel;

namespace Combat.Enums {
// 卡牌附带的五行元素
public enum ElementType {
    [Description("金")]
    Metal = 0,

    [Description("木")]
    Wood = 1,

    [Description("水")]
    Water = 2,

    [Description("火")]
    Fire = 3,

    [Description("土")]
    Earth = 4,
}

public static class ElementTypeFunc {
    private const int N = 5;

    public static ElementType Next(this ElementType type) {
        var next = (int)type + 1;
        if (next >= N) next = 0;
        return (ElementType)next;
    }

    public static ElementType Prev(this ElementType type) {
        var prev = (int)type - 1;
        if (prev < 0) prev = N - 1;
        return (ElementType)prev;
    }

    public static bool IsNext(this ElementType type1, ElementType type2) {
        return Next(type1) == type2;
    }

    public static bool IsPrev(this ElementType type1, ElementType type2) {
        return Prev(type1) == type2;
    }

    public static bool IsClose(this ElementType type1, ElementType type2) {
        return IsNext(type1, type2) || IsPrev(type1, type2);
    }
}
}
