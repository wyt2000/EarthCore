using System;
using System.Collections;
using Combat.Enums;

namespace Combat.Requests.Details {
// 伤害/回血
public class RequestHpChange : CombatRequest {
#region 配置项

    // 作用请求的对象
    public CombatantComponent Target;

    // 伤害/治疗值
    public float Value;

    // 是否为治疗请求
    public bool IsHeal = false;

    // 伤害类型
    public DamageType Type = DamageType.Physical;

    // 元素类型
    public ElementType? Element = null;

    // 修改原因
    public string Reason = "";

#endregion

    public override bool CanEnqueue() {
        return Require(
            Target != null && Causer != null && Value >= 0,
            "非法的生命请求"
        );
    }

    // Todo 加动画
    public override IEnumerator Execute() {
        var causer = Causer;
        var target = Target;
        causer.BoardCast(effect => effect.BeforeTakeHpChange(this));
        if (target.BoardCastAny(effect => effect.BeforeSelfHpChange(this))) yield break;

        var value = Value;
        var state = Target.State;
        if (!IsHeal) {
            if (Type == DamageType.Magical) {
                value *= 1 + Causer.State.MagicDamageAmplify / 100;
                value *= 1 - Causer.State.MagicDamageReduce / 100;

                var shield = Math.Min(state.MagicShield, value);
                state.MagicShield -= shield;

                value -= shield;
            }
            else {
                value *= 1 + Causer.State.PhysicalDamageAmplify / 100;
                value *= 1 - Causer.State.PhysicalDamageReduce / 100;

                var shield = Math.Min(state.PhysicalShield, value);
                state.PhysicalShield -= shield;

                value -= shield;
            }
        }

        value = Math.Max(0, value);

        if (Element.HasValue) Target.TryApplyElementBreak(Causer, Element.Value, 1);

        var old = state.Health;
        if (IsHeal) {
            state.Health += value;
        }
        else {
            state.Health -= value;
        }

        var change = state.Health - old;

        Value = Math.Abs(change);
        target.BoardCast(effect => effect.AfterSelfHpChange(this));
        causer.BoardCast(effect => effect.AfterTakeHpChange(this));
    }

    public override string Description() {
        return $"由于{Reason},{Causer.name}对{Target.name}造成{Value}点{(IsHeal ? "治疗" : "伤害")}";
    }
}
}
