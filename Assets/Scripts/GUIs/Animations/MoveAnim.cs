using System.Collections;
using GUIs.Animations.Lerp;
using UnityEngine;

namespace GUIs.Animations {
public static class MoveToAnim {
    // 直线动画 固定时间
    public static IEnumerator MoveWithDuration(this MonoBehaviour mono, Vector3 target, float duration) {
        var trans = mono.transform;
        var start = trans.position;
        return GAnimation.Lerp(duration, t => trans.position = Vector3.Lerp(start, target, t));
    }

    // 直线动画 固定速度
    public static IEnumerator MoveWithSpeed(this MonoBehaviour mono, Vector3 target, float speed) {
        var trans = mono.transform;
        var start = trans.position;
        var distance = Vector3.Distance(start, target);
        var duration = distance / speed;
        return GAnimation.Lerp(duration, t => trans.position = Vector3.Lerp(start, target, t));
    }

    // 直线路径动画
    public static IEnumerator MoveByPathWithDuration(this MonoBehaviour mono, PathLerp path, float duration) {
        var transform = mono.transform;
        return GAnimation.Lerp(duration, t => transform.position = path.Lerp(t));
    }

    // 直线路径动画
    public static IEnumerator MoveByPathWithSpeed(this MonoBehaviour mono, PathLerp path, float speed) {
        var transform = mono.transform;
        var distance = path.PredictDistance();
        var time = distance / speed;
        return GAnimation.Lerp(time, t => transform.position = path.Lerp(t));
    }
}
}
