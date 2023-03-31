using Combat.Effects;
using UnityEngine;

namespace Combat.Requests.Details {
// 效果附着/脱离请求
public class RequestEffect : CombatRequest {
#region 配置项

    public Effect Effect;
    public bool   Attach;

#endregion

    public override bool PreCheckValid() {
        if (Effect != null && Effect.Target != null) return true;
        Debug.LogWarning("Invalid effect request");
        return false;
    }

    public override void ExecuteLogic() {
        var effect = Effect;
        var attach = Attach;
        if (attach) {
            effect.DoAttach();
        }
        else {
            effect.DoRemove();
        }
    }

    public override string ToString() {
        return $"{(Attach ? "附着" : "脱离")} {Effect.UiName}";
    }
}
}
