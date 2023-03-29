namespace Combat.Requests {
public enum DamageType {
    Physical, // 物理
    Magical,  // 魔法
}

// 五行元素
public enum ElementType {
    Metal, // 金 
    Wood,  // 木
    Water, // 水
    Fire,  // 火
    Earth, // 土
}

public class DamageParams {
    // 伤害类型
    public DamageType DamageType = DamageType.Physical;

    // 伤害附带元素
    public ElementType? ElementType = null;
}

public class HealParams { }

// 生命值请求(伤害/治疗)
public class HealthRequest {
    // 发起请求的对象
    public CombatantComponent Causer;

    // 作用请求的对象
    public CombatantComponent Target;

    // 伤害/治疗值
    public float Value = 0;

    // 是否为治疗请求
    public bool IsHeal = false;

    // 伤害参数
    public DamageParams DamageParams = new();

    // 治疗参数
    public HealParams HealParams = new();
}
}
