using System.Collections;
using Combat;
using GUIs.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GUIs.States {
public class StateFieldView : MonoBehaviour {
#region prefab绑定

    [SerializeField]
    public TooltipHandler tooltip;

    [SerializeField]
    public Image icon;

    [SerializeField]
    private TextMeshProUGUI text;

#endregion

    private void FreshUI(string str) {
        text.text = str;
    }

    // 刷新血条 Todo 血条滚动特效
    public IEnumerator FreshHealthBar(float duration, float old, float cur, float oldLimit, float curLimit) {
        return GAnimation.Lerp(duration, t =>
        {
            FreshUI($"{Mathf.Lerp(old, cur, t):F0}/{Mathf.Lerp(oldLimit, curLimit, t):F0}");
        });
    }

    // 刷新蓝条 Todo 蓝条滚动特效
    public IEnumerator FreshManaBar(float duration, float old, float cur, float oldLimit, float curLimit, CombatantComponent combatant) {
        var batch = combatant.PreviewBatch;
        var cost = batch.PreviewManaCost();
        var sign = cost >= 0 ? '-' : '+';
        var costStr = batch.Cards.Length == 0 ? "" : $"({sign}{Mathf.Abs(cost):F0})";
        yield return GAnimation.Lerp(duration, t =>
        {
            FreshUI($"{Mathf.Lerp(old, cur, t):F0}{costStr}/{Mathf.Lerp(oldLimit, curLimit, t):F0}");
        });
    }

    // 刷新小字段 Todo 数字滚动特效
    public IEnumerator FreshField(float duration, float old, float cur) {
        return GAnimation.Lerp(duration, t =>
        {
            FreshUI($"{Mathf.Lerp(old, cur, t):F0}");
        });
    }

    // 刷新法印 Todo 法印破碎/重置特效
    public IEnumerator FreshSeal(float duration, int old, int cur) {
        FreshUI($"{cur}");
        yield break;
    }
}
}
