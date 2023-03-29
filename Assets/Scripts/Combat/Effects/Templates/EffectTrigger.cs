namespace Combat.Effects.Templates {
// 触发性buff
public class EffectTrigger {
    public int  MaxTriggerTimes = 1;
    public bool InValid => MaxTriggerTimes <= 0;

    private readonly Effect m_effect;

    public EffectTrigger(Effect effect) {
        m_effect = effect;
    }

    public bool Trigger(bool trigger = true) {
        if (!trigger || InValid) return false;
        --MaxTriggerTimes;
        if (InValid) m_effect.Remove();
        return true;
    }
}
}
