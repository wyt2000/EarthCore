using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
// 解决冲突的策略
public enum AnimConflictPolicy {
    // 忽略,不执行新任务
    Ignore,

    // 覆盖,结束当前任务,然后执行新任务
    Overwrite,

    // 延迟,等到没有任务跑了,再执行新任务,按id排队
    Delay,
}

// 动画锁,一个锁管理一类动画 Todo 整合Animation到Coroutine,将locker和Coroutine绑定
public class AnimLocker {
    private int m_id = -1;

    private Queue<int> m_waits;

    public readonly AnimConflictPolicy Policy; // Todo 分离locker和policy?

    public AnimLocker(AnimConflictPolicy policy) {
        Policy = policy;
    }

    public bool CanRun(int id) {
        return m_id == -1 || m_id == id;
    }

    public void Wait(int id) {
        m_waits ??= new Queue<int>();
        m_waits.Enqueue(id);
    }

    public void Run(int id) {
        m_id = id;
    }

    public void Finish() {
        m_id = m_waits is { Count: > 0 } ? m_waits.Dequeue() : -1;
    }
}

public static class GAnimation {
    private static int ms_lerpCounter;

    // 用于创建动画的协程模板
    public static IEnumerator Lerp(AnimLocker locker, float duration, Action<float> action, Func<float, float> curve = null) {
        if (locker == null) {
            yield return Lerp(duration, action, curve);
            yield break;
        }

        var id = ms_lerpCounter++;

        if (!locker.CanRun(id)) {
            switch (locker.Policy) {
                case AnimConflictPolicy.Ignore:
                    // 不能跑,直接忽略新任务
                    yield break;
                case AnimConflictPolicy.Delay: {
                    // 等到能跑,再执行新任务,按id排队
                    locker.Wait(id);
                    while (!locker.CanRun(id)) {
                        yield return null;
                    }
                    break;
                }
                case AnimConflictPolicy.Overwrite:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // 切换当前任务
        locker.Run(id);

        if (duration <= 0) {
            action(1);
            yield break;
        }
        var startTime = Time.time;
        var endTime = startTime + duration;
        float current;
        do {
            // 有其他新任务开始执行
            if (locker.Policy == AnimConflictPolicy.Overwrite && !locker.CanRun(id)) {
                // 覆盖,结束当前任务
                break;
            }
            current = Time.time;
            var t = Mathf.Clamp01((current - startTime) / duration);
            action(curve?.Invoke(t) ?? t);
            yield return null;
        } while (current < endTime);

        // 任务结束
        locker.Finish();
    }

    // 无lock
    public static IEnumerator Lerp(float duration, Action<float> action, Func<float, float> curve = null) {
        if (duration <= 0) {
            action(1);
            yield break;
        }
        var startTime = Time.time;
        var endTime = startTime + duration;
        float current;
        do {
            current = Time.time;
            var t = Mathf.Clamp01((current - startTime) / duration);
            action(curve?.Invoke(t) ?? t);
            yield return null;
        } while (current < endTime);
    }

    // 等待
    public static IEnumerator Wait(float duration) {
        var target = Time.time + duration;
        while (Time.time < target) {
            yield return null;
        }
    }
}
}
