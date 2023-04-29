using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace Utils {
// 基础工具类
public static class GTools {
    // 提取器 转 比较器
    public static Comparison<T> ExtractorToComparer<T>(Func<T, IComparable> extractor) {
        return (a, b) => extractor(a).CompareTo(extractor(b));
    }

    // 事件穿透
    public static bool PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        var current = data.pointerCurrentRaycast.gameObject;
        foreach (var go in results.Select(t => t.gameObject).Where(t => current != t)) {
            ExecuteEvents.Execute(go, data, function);
        }
        return results
            .Select(t => t.gameObject)
            .Where(t => current != t)
            .Any(go => ExecuteEvents.Execute(go, data, function));
    }
}
}
