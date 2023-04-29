using System;
using Combat;
using GUIs.Cards;
using GUIs.Effects;
using GUIs.Else;
using GUIs.States;
using UnityEngine;

namespace GUIs {
public class PlayerView : MonoBehaviour {
#region prefab配置

    public PaintingView painting; // 立绘
    public CardSlotView cardSlot; // 卡槽
    public CardHeapView cardHeap; // 卡堆
    public StateBarView stateBar; // 状态栏
    public EffectListView effectList; // 效果列表

#endregion

    public void Init(CombatantComponent combatant) {
        cardSlot.combatant = combatant;
        stateBar.combatant = combatant;
        effectList.combatant = combatant;
    }
}
}
