using System;
using System.ComponentModel;

namespace Combat.Requests {
public enum DamageType {
    [Description("物理")]
    Physical,

    [Description("魔法")]
    Magical,
}

// 五行元素
public enum ElementType {
    [Description("金")]
    Metal,

    [Description("木")]
    Wood,

    [Description("水")]
    Water,

    [Description("火")]
    Fire,

    [Description("土")]
    Earth,
}

public class DamageParams {
    // 伤害类型
    public DamageType DamageType = DamageType.Physical;

    // 伤害附带元素
    public ElementType? Element = null;

    // 附带元素量
    public int ElementCount = 1;
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

    // 动态伤害/治疗值
    public Func<HealthRequest, float> ValueFunc = null;

    // 是否为治疗请求
    public bool IsHeal = false;

    // 伤害参数
    public readonly DamageParams DamageParams = new();

    // 治疗参数
    public readonly HealParams HealParams = new();

    // 结束回调
    public Action<HealthRequest> OnFinish = delegate { };

    // Todo 添加控制ui动画时间的参数

#region 公开函数

    public float GetValue() {
        return ValueFunc?.Invoke(this) ?? Value;
    }

#endregion
}
}
