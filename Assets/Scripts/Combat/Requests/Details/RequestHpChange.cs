using System;
using System.Collections;
using Combat.Enums;
using GUIs.Audios;
using Utils;

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

    // 是否为真实伤害
    public bool IsReal = false;

    // 伤害类型
    public DamageType Type = DamageType.Physical;

    // 元素类型
    public ElementType? Element = null;

    // 修改原因
    public string Reason = "";

#endregion

#region 输出项

    // 抵消伤害
    public float OutShieldChange;

#endregion

    public override bool CanEnqueue() {
        return
            Require(
                Target != null && Causer != null && Value >= 0,
                "非法的生命请求"
            ) &&
            Require(
                !string.IsNullOrWhiteSpace(Reason),
                "请求原因不能为空"
            );
    }

    public override IEnumerator Execute() {
        var causer = Causer;
        var target = Target;
        causer.BoardCast(effect => effect.BeforeTakeHpChange(this));
        if (!IsReal && target.BoardCastAny(effect => effect.BeforeSelfHpChange(this))) yield break;

        var value = Value;
        var state = Target.State;
        if (!IsHeal && !IsReal) {
            if (Type == DamageType.Magical) {
                value *= 1 + Causer.State.MagicDamageAmplify / 100;
                value *= 1 - Causer.State.MagicDamageReduce / 100;

                var shield = Math.Min(state.MagicShield, value);
                state.MagicShield -= shield;

                OutShieldChange = shield;

                value -= shield;
            }
            else {
                value *= 1 + Causer.State.PhysicalDamageAmplify / 100;
                value *= 1 - Causer.State.PhysicalDamageReduce / 100;

                var shield = Math.Min(state.PhysicalShield, value);
                state.PhysicalShield -= shield;

                OutShieldChange = shield;

                value -= shield;
            }
        }

        value = Math.Max(0, value);

        if (Element.HasValue) Target.TryApplyElementBreak(Causer, Element.Value.Next(), 1);

        var old = state.Health;
        state.Health += IsHeal ? value : -value;
        var change = Math.Abs(state.Health - old);
        Value = change;

        // Todo 加打击/治疗动画
        if (IsHeal) yield return GAudio.PlayHeal();
        else if (Type == DamageType.Physical) yield return GAudio.PlayPhysicalDamage();
        else yield return GAudio.PlayMagicDamage();

        var changeText = IsHeal ? "治疗" : $"{Element?.ToDescription() ?? "无"}属性{Type.ToDescription()}伤害";
        Judge.logger.AddLog($"由于{Reason},{Causer.name}对{Target.name}造成{Math.Abs(change)}点{changeText}");
        target.BoardCast(effect => effect.AfterSelfHpChange(this));
        causer.BoardCast(effect => effect.AfterTakeHpChange(this));
    }
}
}
