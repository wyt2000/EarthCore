using System;
using System.Collections.Generic;

namespace Combat.States {
// 紧凑字段,自动忽略默认value
public class CompactDict<TK, TV> : Dictionary<TK, TV>
where TV : struct, IComparable, IConvertible, IEquatable<TV>, IFormattable {
    public static CompactDict<TK, TV> operator +(CompactDict<TK, TV> a, CompactDict<TK, TV> b) {
        var result = new CompactDict<TK, TV>();
        foreach (var (key, value) in a) {
            result[key] = value;
        }

        foreach (var (key, value) in b) {
            if (result.ContainsKey(key)) {
                result[key] = DynamicOperator.ForceAdd(result[key], value);
            }
            else {
                result[key] = value;
            }
        }

        return result;
    }

    public static CompactDict<TK, TV> operator -(CompactDict<TK, TV> a, CompactDict<TK, TV> b) {
        var result = new CompactDict<TK, TV>();
        foreach (var (key, value) in a) {
            result[key] = value;
        }

        foreach (var (key, value) in b) {
            if (result.ContainsKey(key)) {
                result[key] = DynamicOperator.ForceSub(result[key], value);
            }
            else {
                result[key] = DynamicOperator.ForceSub(default, value);
            }
        }

        return result;
    }

    public new TV this[TK key] {
        get => ContainsKey(key) ? base[key] : default;
        set
        {
            if (value.Equals(default)) {
                Remove(key);
            }
            else {
                base[key] = value;
            }
        }
    }
}
}
