using System;
using System.ComponentModel;
using System.Reflection;

namespace Utils {
public static class EnumExtension {
    public static string ToDescription(this Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}
}
