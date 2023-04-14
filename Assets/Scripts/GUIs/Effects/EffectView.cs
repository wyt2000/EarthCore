using System;
using System.Collections;
using Combat.Effects;
using GUIs.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace GUIs.Effects {
public class EffectView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
#region prefab配置

    // 效果图标
    public Image effectIcon;

    // 效果名称
    public TextMeshProUGUI effectName;

    // 剩余回合
    public TextMeshProUGUI remainTurns;

    // 叠加层数
    public TextMeshProUGUI layerCount;

    // 提示栏根组件
    public Canvas tipView;

    // 提升文本
    public TextMeshProUGUI tipText;

#endregion

    [NonSerialized]
    public EffectListView Container;

    public Effect Data;

    [NonSerialized]
    public int IndexVertical; // 行

    [NonSerialized]
    public int IndexHorizontal; // 列

    private readonly AnimLocker m_locker = new(AnimConflictPolicy.Overwrite);

    private void Start() {
        effectName.text   = Data.UiName;
        effectIcon.sprite = Resources.Load<Sprite>(Data.UiIconPath);
    }

    private void Update() {
        remainTurns.text = Data.LgRemainingRounds.ToString();
        layerCount.text  = Data.LgOverlay.ToString();
    }

    // 目标位置
    private Vector3 TargetPosition() {
        var target =
            Vector3.down * (IndexVertical * 60) +
            Vector3.right * (IndexHorizontal * 60);

        return Container.transform.TransformPoint(target);
    }

    public IEnumerator MoveToTarget(float duration) {
        return this.MoveTo(m_locker, TargetPosition(), duration);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        tipView.gameObject.SetActive(true);
        tipView.SetRenderOrder(100);
        tipText.text = Data.UiDescription;
    }

    public void OnPointerExit(PointerEventData eventData) {
        tipView.gameObject.SetActive(false);
    }
}
}
