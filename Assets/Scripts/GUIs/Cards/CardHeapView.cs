using System;
using GUIs.Common;
using TMPro;
using UnityEngine;
using Utils;

namespace GUIs.Cards {
// 牌堆视图
public class CardHeapView : MonoBehaviour {
#region prefab配置

    public RectTransform center;

    public RectTransform list;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private CardView cardPrefab;

    public float offset = 2;

    public float maxDepth = 30;

    public int previewCount = 3;

#endregion

    [NonSerialized]
    public int Count;

    [NonSerialized]
    public int DisCount;

    private void Start() {
        FreshUI();
    }

    public void FreshUI() {
        var count = Math.Clamp(Count, 1, 10);
        var children = list.GetComponentsInChildren<CardView>();
        if (children.Length > count) {
            children.ForEach((view, i) =>
            {
                if (i < count) return;
                if (Application.isEditor) {
                    DestroyImmediate(view.gameObject);
                }
                else {
                    Destroy(view.gameObject);
                }
            });
        }
        else if (children.Length < count) {
            var cnt = count - children.Length;
            for (var i = 0; i < cnt; i++) Instantiate(cardPrefab, list);
        }
        if (children.Length != count) children = list.GetComponentsInChildren<CardView>();
        var realOffset = Math.Min(offset, maxDepth / (count + 1));
        children.ForEach((view, i) =>
        {
            view.Style = CardStyle.Other;

            view.GetComponent<Canvas>().SetOrder(SortOrder.Card, -1);

            var rect = view.GetComponent<RectTransform>();
            rect.pivot = rect.anchoredPosition = new Vector2(0.5f, 0.5f);

            var off = realOffset * (i - count / 2.0f);
            view.transform.localPosition = new Vector3(off, off);
        });
        {
            text.text = $"{Count}/{DisCount}";
        }
    }

    // 预览牌堆
    private void OnDrawGizmos() {
        if (Application.isEditor) {
            Count = previewCount;
        }
        FreshUI();
    }
}
}
