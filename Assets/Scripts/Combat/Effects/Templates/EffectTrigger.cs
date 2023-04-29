using System.Linq;

namespace Combat.Effects.Templates {
// 触发器
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

// 多状态触发器
public class EffectMultipleTrigger {
    private Effect m_effect;

    private readonly int[] m_times;
    private readonly bool  m_all;

    private bool InValid => m_all ? m_times.All(t => t <= 0) : m_times.Any(t => t <= 0);

    public EffectMultipleTrigger(bool all, params int[] times) {
        m_all   = all;
        m_times = times;
    }

    public Effect Bind(Effect effect) {
        return m_effect = effect;
    }

    public bool Trigger(bool trigger, int index) {
        if (!trigger || index < 0 || index >= m_times.Length || InValid) return false;
        --m_times[index];
        if (InValid) m_effect.Remove();
        return true;
    }
}
}
