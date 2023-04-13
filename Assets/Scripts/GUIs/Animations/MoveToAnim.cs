using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // 直线路径动画 Todo 改成无冲突的
    public static IEnumerator MoveByPath(this MonoBehaviour mono, AnimLocker locker, IEnumerable<Vector3> path, float duration) {
        var nodes = path.ToArray();
        if (nodes.Length == 0) yield break;
        var transform = mono.transform;
        var sum = Vector3.Distance(transform.position, nodes[0]);
        for (var i = 1; i < nodes.Length; i++) {
            sum += Vector3.Distance(nodes[i - 1], nodes[i]);
        }
        if (sum < float.Epsilon) yield break;
        var pre = transform.position;
        foreach (var node in nodes) {
            var dis = Vector3.Distance(pre, node);
            if (dis < float.Epsilon) continue;
            var cost = dis / sum * duration;
            yield return mono.MoveTo(locker, node, cost);
            pre = node;
        }
    }
}
}
