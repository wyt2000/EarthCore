using System.Collections;
using GUIs.Animations.Lerp;
using UnityEngine;
using Utils;

namespace GUIs.Animations {
public static class MoveToAnim {
    // 直线动画
    public static IEnumerator MoveTo(this MonoBehaviour mono, AnimLocker locker, Vector3 target, float duration) {
        var trans = mono.transform;
        var start = trans.position;
        return GAnimation.Lerp(locker, duration, t => trans.position = Vector3.Lerp(start, target, t));
    }

    // 直线路径动画
    public static IEnumerator MoveByPath(this MonoBehaviour mono, AnimLocker locker, PathLerp path, float duration) {
        var transform = mono.transform;
        return GAnimation.Lerp(locker, duration, t => transform.position = path.Lerp(t));
    }
}
}
