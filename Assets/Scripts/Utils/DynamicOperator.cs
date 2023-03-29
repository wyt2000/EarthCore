using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utils {
public static class DynamicOperator {
    private static readonly ISet<MethodInfo> Cache = new HashSet<MethodInfo>();

    private static MethodInfo GetMethod(string op, Type ta, Type tb, Type tr) {
        var method = ta.GetMethod(op, new[] { ta, tb });
        if (Cache.Contains(method)) return method;
        // 可调用
        if (method == null || !method.IsStatic || !method.IsPublic) {
            throw new Exception($"{ta} 与 {tb} 不支持加法运算");
        }

        // 返回值兼容
        if (!method.ReturnType.IsAssignableFrom(tr)) {
            throw new Exception($"{ta} 与 {tb} 的加法运算返回值不兼容");
        }

        Cache.Add(method);
        return method;
    }

    private static T ForceBinaryOp<T>(string op, T a, T b) {
        // 从反射信息拿
        Type ta = a.GetType(), tb = b.GetType();
        // 拿到方法
        var method = GetMethod(op, ta, tb, typeof(T));
        // 执行运算
        var ret = (T)method?.Invoke(null, new object[] { a, b });
        // null 检查
        if (ret == null) {
            throw new Exception($"{ta} 与 {tb} 的加法运算返回值为 null");
        }

        return ret;
    }

    public static T ForceAdd<T>(T a, T b) {
        return ForceBinaryOp("op_Addition", a, b);
    }

    public static T ForceSub<T>(T a, T b) {
        return ForceBinaryOp("op_Subtraction", a, b);
    }
}
}
