using System.Collections;
using Combat.Effects;

namespace Combat.Requests.Details {
// 效果附着/脱离请求
public class RequestEffect : CombatRequest {
#region 配置项

    public Effect Effect;
    public bool   Attach;

#endregion

    public override bool CanEnqueue() {
        return
            Require(
                Effect != null && Effect.Target != null,
                "无效的效果请求"
            ) &&
            Require(
                Effect != null && !Effect.LgTags.Contains(EffectTag.Fixed),
                "固定效果不可移除"
            );
    }

    public override IEnumerator Execute() {
        var effect = Effect;
        var attach = Attach;
        if (attach) {
            if (effect.DoAttach()) yield return Effect.Target.effectList.AddEffect(effect);
        }
        else {
            effect.DoRemove();
            if (!effect.UiHidde) yield return Effect.Target.effectList.RemoveEffect(effect);
        }
    }

    public override string Description() {
        return $"{(Attach ? "附着" : "脱离")} {Effect.UiName}";
    }
}
}
