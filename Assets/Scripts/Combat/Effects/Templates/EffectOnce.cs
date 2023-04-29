using System;

namespace Combat.Effects.Templates {
// 一次性效果,只在附加时触发一次然后立刻移除
public class EffectOnce : Effect {
    public Action<EffectOnce> LgAction = delegate { };

    public EffectOnce() {
        UiHidde = true;
    }

    protected override void OnAfterAttach() {
        base.OnAfterAttach();

        LgAction(this);
        Remove();
    }
}
}
