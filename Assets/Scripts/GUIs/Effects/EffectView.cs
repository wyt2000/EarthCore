using System;
using System.Collections;
using Combat.Effects;
using GUIs.Animations;
using TMPro;
using UnityEngine;

namespace GUIs.Effects {
// Todo 效果图标,加tooltip
public class EffectView : MonoBehaviour {
#region prefab配置

    public TextMeshProUGUI effectName;

    public TextMeshProUGUI remainTurns;

    public TextMeshProUGUI layerCount;

#endregion

    [NonSerialized]
    public EffectListView Container;

    public Effect Data;

    [NonSerialized]
    public int IndexVertical; // 行

    [NonSerialized]
    public int IndexHorizontal; // 列

    private void Start() {
        effectName.text = Data.UiName;
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
        return this.MoveTo(TargetPosition(), duration);
    }
}
}
