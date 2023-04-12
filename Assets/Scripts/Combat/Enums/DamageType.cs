using System.ComponentModel;

namespace Combat.Enums {
public enum DamageType {
    [Description("物理")]
    Physical,

    [Description("魔法")]
    Magical,

    // Todo 真伤/穿透
}
}
