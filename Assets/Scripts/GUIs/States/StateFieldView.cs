using System;
using System.Collections;
using Combat;
using DG.Tweening;
using GUIs.Animations;
using GUIs.Audios;
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

    // 播放字段修改动画
    private IEnumerator PlayShakeAnimation(float duration, bool isAdd) {
        if (duration <= 0) return null;
        return isAdd
            ? null // Todo 实现此特化
            : GCoroutine.Parallel(
                text.transform.DOShakePosition(duration, new Vector3(10f, 10f)).Wait()
            );
    }

    // 刷新血条 Todo 血条滚动特效
    public IEnumerator FreshHealthBar(float duration, float old, float cur, float oldLimit, float curLimit) {
        if (duration > 0 && Mathf.Approximately(old, cur) && Mathf.Approximately(oldLimit, curLimit)) return null;
        return GCoroutine.Parallel(
            GAnimation.Lerp(duration, t =>
            {
                FreshUI($"{Mathf.Lerp(old, cur, t):F0}/{Mathf.Lerp(oldLimit, curLimit, t):F0}");
            }),
            PlayShakeAnimation(duration, cur > old)
        );
    }

    // 刷新蓝条 Todo 蓝条滚动特效
    public IEnumerator FreshManaBar(float duration, float old, float cur, float oldLimit, float curLimit, CombatantComponent combatant) {
        if (duration > 0 && Mathf.Approximately(old, cur) && Mathf.Approximately(oldLimit, curLimit)) return null;
        return GCoroutine.Parallel(
            GAnimation.Lerp(duration, t =>
            {
                var batch = combatant.PreviewBatch;
                var cost = batch.PreviewManaCost();
                var sign = cost >= 0 ? '-' : '+';
                var costStr = batch.Cards.Length == 0 ? "" : $"({sign}{Mathf.Abs(cost):F0})";
                FreshUI($"{Mathf.Lerp(old, cur, t):F0}{costStr}/{Mathf.Lerp(oldLimit, curLimit, t):F0}");
            }),
            PlayShakeAnimation(duration, cur > old)
        );
    }

    // 刷新小字段
    public IEnumerator FreshField(float duration, float old, float cur, Func<float, string> formatter) {
        if (duration > 0 && Mathf.Approximately(old, cur)) return null;
        return GCoroutine.Parallel(
            GAnimation.Lerp(duration, t =>
            {
                FreshUI($"{formatter(Mathf.Lerp(old, cur, t))}");
            }),
            PlayShakeAnimation(duration, cur > old)
        );
    }

    // 刷新法印
    public IEnumerator FreshSeal(float duration, int old, int cur) {
        if (duration > 0 && old == cur) return null;
        FreshUI($"{cur}");
        if (old == cur) return null;
        // Todo 加破碎/重置的粒子特效
        var recover = old < cur;
        return recover
            // 法印重置
            ? GCoroutine.Parallel(
                GAudio.PlaySealRecover(),
                icon.transform.DOShakeScale(duration, new Vector3(0.5f, 0.5f)).Wait()
            )
            // 法印破碎
            : GCoroutine.Parallel(
                GAudio.PlaySealBreak(),
                icon.transform.DOShakePosition(duration, new Vector3(10f, 10f)).Wait()
            );
    }
}
}
