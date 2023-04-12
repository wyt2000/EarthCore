using System;

namespace Utils {
// 基础工具类
public static class GTools {
    // 提取器 转 比较器
    public static Comparison<T> ExtractorToComparer<T>(Func<T, IComparable> extractor) {
        return (a, b) => extractor(a).CompareTo(extractor(b));
    }
}
}
