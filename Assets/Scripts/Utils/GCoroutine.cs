using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
// 解决协程冲突的策略
public enum ResolvePolicy {
    // 忽略,不执行新任务
    Ignore,

    // 覆盖,结束当前任务,然后执行新任务
    Overwrite,

    // 延迟,等到没有任务跑了,再执行新任务,按id排队
    Delay,
}

// 协程锁,一个锁管理一类协程
public class CoroutineLocker {
    private int m_id = -1;

    private Queue<int> m_waits;

    public readonly ResolvePolicy Policy;

    public CoroutineLocker(ResolvePolicy policy) {
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

// 协程工具类
public static class GCoroutine {
    private static int ms_counter;

    private class StackEnumerator : IEnumerator {
        private readonly Stack<IEnumerator> m_stack = new();

        public StackEnumerator(IEnumerator enumerator) {
            m_stack.Push(enumerator);
        }

        public bool MoveNext() {
            var i = m_stack.Peek();
            if (i.MoveNext()) {
                if (i.Current is IEnumerator result) m_stack.Push(result);
                return true;
            }

            if (m_stack.Count <= 1) return false;
            m_stack.Pop();
            return true;
        }

        public void Reset() { }

        public object Current => null;
    }

    private class ParallelEnumerator : IEnumerator {
        private readonly StackEnumerator[] m_enumerators;

        private ParallelEnumerator m_next;

        public ParallelEnumerator(IEnumerable<IEnumerator> coroutines) {
            m_enumerators = coroutines.Where(c => c != null).Select(c => new StackEnumerator(c)).ToArray();
        }

        public bool MoveNext() {
            var allDone = true;
            foreach (var enumerator in m_enumerators) {
                if (enumerator.MoveNext()) allDone = false;
            }

            return !allDone;
        }

        public void Reset() { }

        public object Current => null;
    }

    // 并行运行多个协程 Todo! 支持YieldInstruction
    public static IEnumerator Parallel(IEnumerable<IEnumerator> coroutines) {
        return new ParallelEnumerator(coroutines);
    }

    // 并行运行多个协程
    public static IEnumerator Parallel(params IEnumerator[] coroutines) {
        return new ParallelEnumerator(coroutines);
    }

    // 顺序运行多个协程
    public static IEnumerator Sequence(params IEnumerator[] coroutines) {
        return coroutines.GetEnumerator();
    }

    // 带锁协程
    public static IEnumerator Lock(this IEnumerator coroutine, CoroutineLocker locker) {
        if (locker == null) {
            yield return coroutine;
            yield break;
        }

        var id = ms_counter++;

        if (!locker.CanRun(id)) {
            switch (locker.Policy) {
                case ResolvePolicy.Ignore:
                    // 不能跑,直接忽略新任务
                    yield break;
                case ResolvePolicy.Delay: {
                    // 等到能跑,再执行新任务,按id排队
                    locker.Wait(id);
                    while (!locker.CanRun(id)) {
                        yield return null;
                    }
                    break;
                }
                case ResolvePolicy.Overwrite:
                    // 直接覆盖当前任务
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // 切换当前任务
        locker.Run(id);
        do {
            // 有其他新任务开始执行
            if (locker.Policy == ResolvePolicy.Overwrite && !locker.CanRun(id)) {
                // 覆盖,结束当前任务
                break;
            }
            yield return coroutine.Current;
        } while (coroutine.MoveNext());
        locker.Finish();
    }
}
}
