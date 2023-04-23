using UnityEngine;

namespace Utils {
// 场景路由器
public class SceneStore : MonoBehaviour {
    private object m_data;

    private static SceneStore ms_instance;

    private void Start() {
        if (ms_instance != null) {
            Destroy(gameObject);
            return;
        }
        ms_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Store(object data) {
        ms_instance.m_data = data;
    }

    public static T Get<T>() {
        return (T)ms_instance.m_data;
    }
}
}
