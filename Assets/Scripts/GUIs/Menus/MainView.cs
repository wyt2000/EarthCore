using System.Collections;
using Combat.Enums;
using DG.Tweening;
using GUIs.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GUIs.Menus {
// 主界面ui
public class MainView : MonoBehaviour {
#region prefab配置

    [SerializeField]
    private PageController page;

    [SerializeField]
    private Image fadeMask;

#endregion

    private enum PageType {
        入口,
        模式选择,
        存档选择,
        游戏设置,
    };

    private void Start() {
        // Todo 加载存档
        DontDestroyOnLoad(transform.parent);
    }

#region 入口按钮

    // 开始游戏
    public void OnStartGame() {
        page.Select(PageType.模式选择);
    }

    // 读取存档
    public void OnLoadGame() {
        page.Select(PageType.存档选择);
    }

    // 游戏设置
    public void OnGameSettings() {
        page.Select(PageType.游戏设置);
    }

    // 退出游戏
    public void OnExitGame() {
    #if UNITY_STANDALONE
        Application.Quit();
    #endif
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

#endregion

#region 模式选择

    // Todo 给场景传参
    private IEnumerator ImpEnterGameWith(GameModeType mode) {
        const float duration = 2.0f;
        yield return fadeMask.DOFade(1.0f, duration).WaitForCompletion();
        gameObject.SetActive(false);
        SceneManager.LoadScene("CombatScene");
        yield return fadeMask.DOFade(0.0f, duration).WaitForCompletion();
    }

    private void EnterGameWith(GameModeType mode) {
        StartCoroutine(ImpEnterGameWith(mode));
    }

    // 开始教学模式
    public void OnEnterTutorial() {
        EnterGameWith(GameModeType.Tutorial);
    }

    // 开始无尽模式
    public void OnEnterEndless() {
        EnterGameWith(GameModeType.Endless);
    }

    // 开始pvp模式
    public void OnEnterPvp() {
        EnterGameWith(GameModeType.PvP);
    }

    // 返回主界面
    public void OnBackToMain() {
        page.Select(PageType.入口);
    }

#endregion

#region 存档选择

    // 新建存档/覆盖存档 , -1为新建
    public void OnSaveTo(int index) {
        // Todo 实现存档机制
    }

#endregion
}
}
