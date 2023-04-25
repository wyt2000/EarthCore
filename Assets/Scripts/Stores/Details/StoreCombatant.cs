using Combat;
using Combat.Effects;
using Combat.States;

namespace Stores.Details {
public abstract class StoreCombatant {
    public void InitState(CombatantComponent combatant) {
        var ret = OnInitState();
        ret.Health = ret.HealthMax;
        ret.Mana   = ret.ManaMax;

        ret.ElementAttach += ret.ElementMaxAttach;

        combatant.State = ret;
        // 加载固有buff
        foreach (var effect in EffectFixed.GetAll(combatant)) {
            combatant.AddBuff(effect);
        }
    }

    protected abstract CombatState OnInitState();
}
}
