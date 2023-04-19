using System.Collections;
using Combat;
using GUIs.Animations;
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

    // 播放scale+滚动动画
    private IEnumerator PlayShakeAnimation(float duration) {
        return duration <= 0 ? null : text.GetComponent<Animation>().Wait();
    }

    // 刷新血条 Todo 血条滚动特效
    public IEnumerator FreshHealthBar(float duration, float old, float cur, float oldLimit, float curLimit) {
        if (duration > 0 && Mathf.Approximately(old, cur) && Mathf.Approximately(oldLimit, curLimit)) return null;
        return GCoroutine.Parallel(
            GAnimation.Lerp(duration, t =>
            {
                FreshUI($"{Mathf.Lerp(old, cur, t):F0}/{Mathf.Lerp(oldLimit, curLimit, t):F0}");
            }),
            PlayShakeAnimation(duration)
        );
    }

    // 刷新蓝条 Todo 蓝条滚动特效
    public IEnumerator FreshManaBar(float duration, float old, float cur, float oldLimit, float curLimit, CombatantComponent combatant) {
        if (duration > 0 && Mathf.Approximately(old, cur) && Mathf.Approximately(oldLimit, curLimit)) return null;
        var batch = combatant.PreviewBatch;
        var cost = batch.PreviewManaCost();
        var sign = cost >= 0 ? '-' : '+';
        var costStr = batch.Cards.Length == 0 ? "" : $"({sign}{Mathf.Abs(cost):F0})";
        return GCoroutine.Parallel(
            GAnimation.Lerp(duration, t =>
            {
                FreshUI($"{Mathf.Lerp(old, cur, t):F0}{costStr}/{Mathf.Lerp(oldLimit, curLimit, t):F0}");
            }),
            PlayShakeAnimation(duration)
        );
    }

    // 刷新小字段
    public IEnumerator FreshField(float duration, float old, float cur) {
        if (duration > 0 && Mathf.Approximately(old, cur)) return null;
        return GCoroutine.Parallel(
            GAnimation.Lerp(duration, t =>
            {
                FreshUI($"{Mathf.Lerp(old, cur, t):F0}");
            }),
            PlayShakeAnimation(duration)
        );
    }

    // 刷新法印 Todo 法印破碎/重置特效
    public IEnumerator FreshSeal(float duration, int old, int cur) {
        if (duration > 0 && old == cur) yield break;
        FreshUI($"{cur}");
        // Todo icon加个单独的shake动画和粒子
        // yield return icon.GetComponent<Animation>().Wait();
    }
}
}
