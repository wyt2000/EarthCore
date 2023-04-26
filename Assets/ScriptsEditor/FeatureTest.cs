#if UNITY_EDITOR
using System.Collections;
using Combat.Story;
using UnityEditor;
using UnityEngine;
using Utils;

namespace ScriptsEditor {
public static class CoroutineTest {
#region 协程测试

    [MenuItem("Tests/测试协程锁")]
    // 测试resolvePolicy
    private static void TestCoroutine() {
        // ignore 测试
        {
            Debug.Log("ignore 测试");
            var locker = new CoroutineLocker(ResolvePolicy.Ignore);
            var task1 = Task(1, 5).Lock(locker);
            task1.MoveNext();
            var task12 = Task(2, 5).Lock(locker);
            Run(GCoroutine.Parallel(task1, task12));
        }
        // overwrite 测试
        {
            Debug.Log("overwrite 测试");
            var locker = new CoroutineLocker(ResolvePolicy.Overwrite);
            var task1 = Task(1, 5).Lock(locker);
            task1.MoveNext();
            var task12 = Task(2, 5).Lock(locker);
            Run(GCoroutine.Parallel(task1, task12));
        }
        // delay 测试
        {
            Debug.Log("delay 测试");
            var locker = new CoroutineLocker(ResolvePolicy.Delay);
            var task1 = Task(1, 5).Lock(locker);
            task1.MoveNext();
            var task12 = Task(2, 5).Lock(locker);
            Run(GCoroutine.Parallel(task1, task12));
        }
    }

    private static void Run(IEnumerator task) {
        do { } while (task.MoveNext());
    }

    private static IEnumerator Task(int id, int cnt) {
        for (var i = 0; i < cnt; i++) {
            Debug.Log($"Task {id} : {i}");
            yield return null;
        }
    }

#endregion

#region 剧本测试

    [MenuItem("Tests/剧本加载测试")]
    private static void TestStory() {
        Debug.Log(StoryCreator.Load("Jin").Length);
    }

#endregion
}
}
#endif
