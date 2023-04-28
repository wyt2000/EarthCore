using System.Collections;
using Combat.Enums;
using DG.Tweening;
using GUIs.Common;
using Stores;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace GUIs.Globals {
// 主界面ui
public class MainView : MonoBehaviour {
#region prefab配置

    [SerializeField]
    private PageController page;

    [SerializeField]
    private Image fadeMask;

    [SerializeField]
    private Button saveButton;

    [SerializeField]
    private RectTransform saveList;

#endregion

    private enum PageType {
        入口,
        模式选择,
        存档选择,
        游戏设置,
    }

    private void Start() {
        DontDestroyOnLoad(transform.parent);
        // 加载当前存档
        StoreManager.All(10).ForEach(AddSaveButton);
    }

    private void AddSaveButton(int index) {
        var button = Instantiate(saveButton, saveList);
        button.onClick.AddListener(() => OnSaveTo(index));
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"存档{index + 1}";
        button.transform.SetAsFirstSibling();
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
        SceneManager.LoadScene("Scenes/CombatScene");
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

    // Todo 存档读写界面分离
    // Todo 特化存档列表界面(根据时间戳排序)

    // 新建存档/覆盖存档 , -1为新建
    public void OnSaveTo(int index) {
        if (index == -1) {
            index = StoreManager.NewIndex(10);
            if (index == -1) {
                Debug.LogError("存档已满");
                return;
            }
            AddSaveButton(index);
        }
        var success = StoreManager.SaveCurrent(index);
        Debug.Log(success ? "写入存档成功" : "写入存档失败");
    }

    // 加载存档
    public void OnLoadFrom(int index) {
        // Todo 加载存档
        Debug.Log(StoreManager.LoadCurrent(index) ? "加载存档成功" : "加载存档失败");
    }

#endregion
}
}
