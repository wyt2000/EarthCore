using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Utils {
public static class LinqExtension {
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (var item in source) {
            action(item);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
        var i = 0;
        foreach (var item in source) {
            action(item, i);
            i++;
        }
    }

    // return (true, false)
    public static (IEnumerable<T>, IEnumerable<T>) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
        var list1 = new List<T>();
        var list2 = new List<T>();
        foreach (var item in source) {
            if (predicate(item)) {
                list1.Add(item);
            }
            else {
                list2.Add(item);
            }
        }
        return (list1, list2);
    }

    // 抽取对象
    public static List<T> Extract<T>(this List<T> source, Func<T, int, bool> predicate) {
        var ret = new List<T>();
        var i = 0;
        source.RemoveAll(s =>
        {
            if (!predicate(s, i++)) return false;
            ret.Add(s);
            return true;
        });
        return ret;
    }

    // 抽取对象
    public static List<T> Extract<T>(this List<T> source, Func<T, bool> predicate) {
        return source.Extract((s, _) => predicate(s));
    }
}

public static class EnumExtension {
    public static string ToDescription(this Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}
}
