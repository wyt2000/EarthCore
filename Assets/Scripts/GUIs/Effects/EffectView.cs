using System;
using System.Collections;
using Combat.Effects;
using GUIs.Animations;
using GUIs.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GUIs.Effects {
public class EffectView : MonoBehaviour {
#region prefab配置

    // 效果图标
    public Image effectIcon;

    // 效果名称
    public TextMeshProUGUI effectName;

    // 剩余回合
    public TextMeshProUGUI remainTurns;

    // 叠加层数
    public TextMeshProUGUI layerCount;

    // 提示栏
    public TooltipHandler tooltip;

#endregion

    [NonSerialized]
    public EffectListView Container;

    public Effect Data;

    [NonSerialized]
    public int IndexVertical; // 行

    [NonSerialized]
    public int IndexHorizontal; // 列

    private readonly CoroutineLocker m_locker = new(ResolvePolicy.Overwrite);

    private void Start() {
        effectIcon.sprite = Resources.Load<Sprite>(Data.UiIconPath);
        tooltip.OnShow    = () => $"{Data.UiName}:{Data.UiDescription}";
    }

    private void FreshUI() {
        Data.ViewRender(this);
    }

    private void Update() {
        FreshUI();
    }

    // 目标位置
    private Vector3 TargetPosition() {
        var target =
            Vector3.down * (IndexVertical * 60) +
            Vector3.right * (IndexHorizontal * 60);

        return Container.transform.TransformPoint(target);
    }

    public IEnumerator MoveToTarget(float duration) {
        return this.MoveWithDuration(TargetPosition(), duration).Lock(m_locker);
    }
}
}
