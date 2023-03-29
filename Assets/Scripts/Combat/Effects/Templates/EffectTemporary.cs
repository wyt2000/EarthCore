namespace Combat.Effects.Templates {
// 所有临时性增益/减益效果,失效前后不改变原有状态
public class EffectTemporary : Effect {
    public readonly CombatAddableState AddState = new();

    protected override void OnAfterAttach() {
        base.OnAfterAttach();

        Target.State.Add(AddState);
    }

    protected override void OnLeaveAttach() {
        base.OnLeaveAttach();

        Target.State.Sub(AddState);
    }
}
}
