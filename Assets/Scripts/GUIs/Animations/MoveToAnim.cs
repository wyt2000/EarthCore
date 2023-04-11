using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GUIs.Animations {
// Todo 处理重复调用
public static class MoveToAnim {
    // private static readonly Dictionary<int, int> ms_refs = new();


    // 直线动画
    public static IEnumerator MoveTo(this MonoBehaviour mono, Vector3 target, float duration) {
        var id = mono.GetInstanceID();
        var transform = mono.transform;
        var startPosition = transform.position;
        var startTime = Time.time;
        var endTime = startTime + duration;
        do {
            var current = Time.time;
            var t = Mathf.Clamp01((current - startTime) / duration);
            transform.position = Vector3.Lerp(startPosition, target, t);
            yield return null;
        } while (Time.time < endTime);
        transform.position = target;
    }

    // 直线路径动画
    public static IEnumerator MoveByPath(this MonoBehaviour mono, IEnumerable<Vector3> path, float duration) {
        var nodes = path.ToArray();
        if (nodes.Length == 0) yield break;
        var transform = mono.transform;
        var sum = Vector3.Distance(transform.position, nodes[0]);
        for (var i = 1; i < nodes.Length; i++) {
            sum += Vector3.Distance(nodes[i - 1], nodes[i]);
        }
        var pre = transform.position;
        foreach (var node in nodes) {
            yield return mono.MoveTo(node, Vector3.Distance(pre, node) / sum * duration);
            pre = node;
        }
    }
}
}
