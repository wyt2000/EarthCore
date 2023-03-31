using System;

namespace Utils {
// Todo 使用lazy value代替Value+ValueFunc
public class LazyValue<TS, TV> {
    private readonly Func<TS, TV> m_func;

    private readonly bool m_isFunc;

    private TS m_state;

    private TV m_value;

    public LazyValue(TV value) {
        m_value = value;
    }

    public LazyValue(Func<TS, TV> func) {
        m_func   = func ?? throw new ArgumentNullException(nameof(func));
        m_isFunc = true;
    }

    public LazyValue<TS, TV> Bind(TS state) {
        m_state = state;
        return this;
    }

    public static implicit operator LazyValue<TS, TV>(TV value) {
        return new LazyValue<TS, TV>(value);
    }

    public static implicit operator LazyValue<TS, TV>(Func<TS, TV> func) {
        return new LazyValue<TS, TV>(func);
    }

    public static implicit operator TV(LazyValue<TS, TV> value) {
        if (!value.m_isFunc) return value.m_value;
        if (value.m_state == null) throw new Exception("LazyValue is not bind");
        value.m_value = value.m_func(value.m_state);
        return value.m_value;
    }
}
}
