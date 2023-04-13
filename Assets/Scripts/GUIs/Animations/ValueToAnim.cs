using System.Collections;
using TMPro;
using UnityEngine;
using Utils;

namespace GUIs.Animations {
// 数值平滑动画(浮点属性用)
public static class ValueToAnim {
    public static IEnumerator SetTo(this TextMeshProUGUI text, AnimLocker locker, float old, float target, float duration) {
        return GAnimation.Lerp(locker, duration, t => text.text = $"{Mathf.Lerp(old, target, t):F2}");
    }
}
}
