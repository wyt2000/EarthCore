namespace Combat.Effects.Templates {
// 触发性buff
public class EffectTrigger {
    private bool InValid => m_times <= 0;

    private Effect m_effect;
    private int    m_times;

    public EffectTrigger(int times = 1) {
        m_times = times;
    }

    public Effect Bind(Effect effect) {
        return m_effect = effect;
    }

    public bool Trigger(bool trigger = true) {
        if (!trigger || InValid) return false;
        --m_times;
        if (InValid) m_effect.Remove();
        return true;
    }
}
}
