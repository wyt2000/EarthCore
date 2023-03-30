using System.Collections.Generic;
using System.Linq;
using Combat.Requests;
using Utils;

namespace Combat.Effects.Templates {
// 复合buff效果(需要保证所有buff的生命周期一致)
public sealed class EffectComposed : Effect {
    private readonly IList<Effect> m_effects;

    public EffectComposed(IEnumerable<Effect> effects) {
        m_effects = effects.ToList();
        foreach (var effect in m_effects) {
            effect.Parent = this;
        }
    }

#region 事件转发

    protected override bool OnBeforeAttach(Effect effect) {
        return base.OnBeforeAttach(effect) || m_effects.Any(e => e.BeforeAttach(effect));
    }

    protected override void OnAfterAttach() {
        base.OnAfterAttach();
        m_effects.ForEach(e => e.AfterAttach());
    }

    protected override void OnLeaveAttach() {
        base.OnLeaveAttach();
        m_effects.ForEach(e => e.LeaveAttach());
    }

    protected override void OnBeforeTurnStart() {
        base.OnBeforeTurnStart();
        m_effects.ForEach(e => e.BeforeTurnStart());
    }

    protected override void OnAfterTurnEnd() {
        base.OnAfterTurnEnd();
        m_effects.ForEach(e => e.AfterTurnEnd());
    }

    protected override void OnBeforeTakeHpChange(HealthRequest request) {
        base.OnBeforeTakeHpChange(request);
        m_effects.ForEach(e => e.BeforeTakeHpChange(request));
    }

    protected override bool OnBeforeSelfHpChange(HealthRequest request) {
        return base.OnBeforeSelfHpChange(request) || m_effects.Any(e => e.BeforeSelfHpChange(request));
    }

    protected override void OnAfterSelfHpChange(HealthRequest request) {
        base.OnAfterSelfHpChange(request);
        m_effects.ForEach(e => e.AfterSelfHpChange(request));
    }

    protected override void OnAfterTakeHpChange(HealthRequest request) {
        base.OnAfterTakeHpChange(request);
        m_effects.ForEach(e => e.AfterTakeHpChange(request));
    }

    protected override void OnBeforePlayCard(PlayCardRequest request) {
        base.OnBeforePlayCard(request);
        m_effects.ForEach(e => e.BeforePlayCard(request));
    }

    protected override void OnAfterPlayCard(PlayCardRequest request) {
        base.OnAfterPlayCard(request);
        m_effects.ForEach(e => e.AfterPlayCard(request));
    }

#endregion
}
}
