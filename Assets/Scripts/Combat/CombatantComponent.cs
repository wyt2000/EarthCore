using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Cards;
using Combat.Effects;
using Combat.Requests.Details;
using Combat.States;
using GUIs;
using GUIs.Cards;
using GUIs.Effects;
using UnityEngine;
using Utils;

namespace Combat {
// 最基础的战斗对象组件
public class CombatantComponent : MonoBehaviour {
#region prefab配置

    public CardSlotView   cardSlot;   // 卡槽
    public CardHeapView   cardHeap;   // 卡堆
    public StateBarView   stateBar;   // 状态栏
    public EffectListView effectList; // 效果列表

#endregion

    [NonSerialized]
    public CombatJudge Judge;

    [NonSerialized]
    public CombatantComponent Opponent;

    public readonly CombatState State = new() {
        HealthMaxBase    = 1000,
        HealthMaxPercent = 0,
        HealthMaxExtra   = 0,

        ManaMaxBase    = 150,
        ManaMaxPercent = 0,
        ManaMaxExtra   = 0,

        PhysicalShield        = 0,
        PhysicalDamageAmplify = 0,
        PhysicalDamageReduce  = 0,

        MagicShield        = 0,
        MagicDamageAmplify = 0,
        MagicDamageReduce  = 0,
    };

    // 玩家的效果
    public readonly ISet<Effect> Effects = new SortedSet<Effect>();

    // 玩家的手牌
    public readonly List<Card> Cards = new();

    // 玩家的牌堆
    public readonly CardHeap Heap = new();

    private void Start() {
        State.Health = State.HealthMax / 2;
        State.Mana   = State.ManaMax / 2;
    }

    public void Attach(Effect effect) {
        AttachTo(effect, this);
    }

    public void AttachTo(Effect effect, CombatantComponent target) {
        effect.Causer = this;
        effect.Target = target;
        effect.Attach(target);
    }

    public void Attack(CombatantComponent target, RequestHpChange request) {
        request.Causer = this;
        request.Target = target;
        request.IsHeal = false;
        Judge.Requests.Add(request);
    }

    public void HealSelf(RequestHpChange request) {
        request.Causer = this;
        request.Target = this;
        request.IsHeal = true;
        Judge.Requests.Add(request);
    }

    public void PlayCard(CombatantComponent target, RequestPlayBatchCard request) {
        request.Causer = this;
        request.Target = target;
        Judge.Requests.Add(request);
    }

    public void GetCard(RequestGetCard request) {
        request.Causer = this;
        Judge.Requests.Add(request);
    }

    public void GetCard(int cnt) {
        GetCard(new RequestGetCard {
            Count = cnt
        });
    }

    public void BoardCast(Action<Effect> action) {
        Effects.ForEach(action);
    }

    public bool BoardCastAny(Func<Effect, bool> action) {
        return Effects.Any(action);
    }
}
}
