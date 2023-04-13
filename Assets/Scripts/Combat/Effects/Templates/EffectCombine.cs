using System.Collections.Generic;
using System.Linq;
using Combat.Requests.Details;

namespace Combat.Effects.Templates {
// 转发多个buff请求
public class EffectCombine : EffectOnce {
    private readonly List<Effect> m_effects = new();

    private EffectCombine(IEnumerable<Effect> effects) {
        m_effects.AddRange(effects);
        UiHidde  = true;
        LgAction = _ => ProxyAll();
    }

    public static EffectCombine Create(params Effect[] effects) {
        return new EffectCombine(effects);
    }

    private void ProxyAll() {
        foreach (var effect in (m_effects as IEnumerable<Effect>).Reverse()) {
            Target.Judge.Requests.AddFirst(new RequestEffect {
                Causer = Causer,
                Effect = effect,
                Attach = true
            });
        }
        Target.Judge.Requests.AddFirst(new RequestEffect {
            Causer = Causer,
            Effect = this,
            Attach = false
        });
    }
}
}
