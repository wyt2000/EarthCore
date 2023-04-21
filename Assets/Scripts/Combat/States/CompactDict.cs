using System;
using System.Collections.Generic;

namespace Combat.States {
// 紧凑字段,自动忽略默认value
public class CompactDict<TK, TV> : Dictionary<TK, TV>
where TV : struct, IComparable, IConvertible, IEquatable<TV>, IFormattable {
    public new TV this[TK key] {
        get => ContainsKey(key) ? base[key] : default;
        private set
        {
            if (value.Equals(default)) {
                Remove(key);
            }
            else {
                base[key] = value;
            }
        }
    }

    public CompactDict<TK, TV> Clone() {
        var result = new CompactDict<TK, TV>();
        foreach (var (key, value) in this) {
            result[key] = value;
        }

        return result;
    }

    public void Add(CompactDict<TK, TV> rhs) {
        foreach (var (key, value) in rhs) {
            this[key] = DynamicOperator.ForceAdd(this[key], value);
        }
    }

    public void Sub(CompactDict<TK, TV> rhs) {
        foreach (var (key, value) in rhs) {
            this[key] = DynamicOperator.ForceSub(this[key], value);
        }
    }

    public static CompactDict<TK, TV> operator +(CompactDict<TK, TV> a, CompactDict<TK, TV> b) {
        var result = a.Clone();
        result.Add(b);
        return result;
    }

    public static CompactDict<TK, TV> operator -(CompactDict<TK, TV> a, CompactDict<TK, TV> b) {
        var result = a.Clone();
        result.Sub(b);
        return result;
    }
}
}
