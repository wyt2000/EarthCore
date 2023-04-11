using System;
using System.Collections.Generic;


namespace Utils {
public class AddableDict<TK, TV> : Dictionary<TK, TV>
where TV : struct, IComparable, IConvertible, IEquatable<TV>, IFormattable {
    private void RemoveDefault() {
        var keys = new List<TK>();
        foreach (var (key, value) in this) {
            if (value.Equals(default)) {
                keys.Add(key);
            }
        }

        foreach (var key in keys) {
            Remove(key);
        }
    }

    public static AddableDict<TK, TV> operator +(AddableDict<TK, TV> a, AddableDict<TK, TV> b) {
        var result = new AddableDict<TK, TV>();
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

        result.RemoveDefault();
        return result;
    }

    public static AddableDict<TK, TV> operator -(AddableDict<TK, TV> a, AddableDict<TK, TV> b) {
        var result = new AddableDict<TK, TV>();
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

        result.RemoveDefault();
        return result;
    }
}

public static class DynamicOperator {
    public static T ForceAdd<T>(T a, T b) {
        return (dynamic)a + (dynamic)b;
    }

    public static T ForceSub<T>(T a, T b) {
        return (dynamic)a - (dynamic)b;
    }
}
}
