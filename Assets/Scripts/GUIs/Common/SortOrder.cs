using System;
using UnityEngine;

namespace GUIs.Common {
public enum SortOrder {
    Default,
    Card,
    Tooltip,
}

public static class SortOrderExtension {
    private static readonly int Capacity = short.MaxValue / Enum.GetValues(typeof(SortOrder)).Length;

    public static void SetOrder(this Canvas canvas, SortOrder order, int localOrder = 0) {
        canvas.overrideSorting = true;
        canvas.sortingOrder    = (int)order * Capacity + localOrder;
    }
}
}
