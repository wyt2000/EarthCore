﻿using System.Collections;
using Combat.Effects;
using UnityEngine;

namespace Combat.Requests.Details {
// 效果附着/脱离请求
public class RequestEffect : CombatRequest {
#region 配置项

    public Effect Effect;
    public bool   Attach;

#endregion

    public override bool CanEnqueue() {
        if (Effect != null && Effect.Target != null) return true;
        Debug.LogWarning("Invalid effect request");
        return false;
    }

    public override IEnumerator Execute() {
        var effect = Effect;
        var attach = Attach;
        if (attach) {
            effect.DoAttach();
            if (!effect.UiHidde) yield return Effect.Target.effectList.AddEffect(effect);
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
