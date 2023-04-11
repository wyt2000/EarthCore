using System;

namespace Combat.States {
// 所有的战斗属性
public class CombatState : CombatAddableState {
    private float m_health;
    private float m_mana;

#region 计算字段

    // 玩家最大生命值
    public float HealthMax => HealthMaxBase * (1 + HealthMaxPercent / 100) + HealthMaxExtra;

    // 玩家最大法力
    public float ManaMax => ManaMaxBase * (1 + ManaMaxPercent / 100) + ManaMaxExtra;

#endregion

#region 区间约束字段

    // 玩家当前生命值
    public float Health {
        get => m_health;
        set => m_health = Math.Clamp(value, 0, HealthMax);
    }

    // 玩家当前法力
    public float Mana {
        get => m_mana;
        set => m_mana = Math.Clamp(value, 0, ManaMax);
    }

#endregion
}
}
