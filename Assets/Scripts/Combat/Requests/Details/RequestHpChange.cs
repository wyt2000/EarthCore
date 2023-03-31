using System;
using UnityEngine;
using Utils;

namespace Combat.Requests.Details {
using DamageType = Combat.Enums.DamageType;

// 伤害/回血
public class RequestHpChange : CombatRequest {
#region 配置项

    // 作用请求的对象
    public CombatantComponent Target;

    // 伤害/治疗值
    public LazyValue<RequestHpChange, float> Value = 0;

    // 是否为治疗请求
    public bool IsHeal = false;

    // 伤害类型
    public DamageType DamageType = DamageType.Physical;

#endregion

    public override bool PreCheckValid() {
        Value.Bind(this);
        if (Target != null && Causer != null && Value >= 0) return true;
        Debug.LogWarning("Invalid health request");
        return false;
    }

    public override void ExecuteLogic() {
        // request.Target.State.ApplyHealthChange(request);
        throw new NotImplementedException();
    }

    public override string ToString() {
        return $"{Causer.name}对{Target.name}造成{Value}点{(IsHeal ? "治疗" : "伤害")}";
    }
}
}
