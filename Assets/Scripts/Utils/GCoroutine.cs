using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
// 协程工具类
public static class GCoroutine {
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

        public void Reset() {
            throw new System.NotImplementedException();
        }

        public object Current => null;
    }

    private class ParallelEnumerator : IEnumerator {
        private readonly StackEnumerator[] m_enumerators;

        private ParallelEnumerator m_next;

        public ParallelEnumerator(IEnumerable<IEnumerator> coroutines) {
            m_enumerators = coroutines.Select(c => new StackEnumerator(c)).ToArray();
        }

        public bool MoveNext() {
            var allDone = true;
            foreach (var enumerator in m_enumerators) {
                if (enumerator.MoveNext()) allDone = false;
            }

            return !allDone;
        }

        public void Reset() {
            throw new System.NotImplementedException();
        }

        public object Current => null;
    }

    // 并行运行多个协程
    public static IEnumerator Parallel(IEnumerable<IEnumerator> coroutines) {
        return new ParallelEnumerator(coroutines);
    }

    // 顺序运行多个协程
    public static IEnumerator Sequence(params IEnumerator[] coroutines) {
        return coroutines.GetEnumerator();
    }
}
}
