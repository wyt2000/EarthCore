﻿using System.Linq;
using GUIs.Cards;
using UnityEngine;

namespace GUIs.Else {
public class Star5View : MonoBehaviour {
#region prefab配置

    // 五边形半径
    public float radius = 80;

    // 图标大小
    public float iconSize = 50;

    // 五芒星
    [SerializeField]
    private RectTransform[] stars;

    // 自身矩形
    [SerializeField]
    private RectTransform rect;

    // 绑定玩家的slot Todo! 预览元素联动,给选中元素上个高亮特效
    [SerializeField]
    private CardSlotView slot;

#endregion

    private void Start() {
        FreshUI();
    }

    private void OnDrawGizmos() {
        if (!(stars != null && stars.All(c => c != null) && stars.Length == 5)) return;
        FreshUI();
    }

    private void FreshUI() {
        const float angle = 2 * Mathf.PI / 5;

        var offset = Vector3.up * radius;
        for (var i = 0; i < 5; i++) {
            stars[i].localPosition = Quaternion.AngleAxis(angle * i * Mathf.Rad2Deg, Vector3.forward) * offset;
            stars[i].sizeDelta     = Vector2.one * iconSize;
        }

        var cos = Mathf.Cos(angle / 2);
        var x = radius * (2 * Mathf.Sin(angle));
        // var y = radius * (1 + cos);

        rect.sizeDelta = new Vector2(x + iconSize, x + iconSize);

        rect.pivot = new Vector2(0.5f, (cos * radius + iconSize / 2) / ((1 + cos) * radius + iconSize));
    }
}
}
