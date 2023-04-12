using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Cards;
using Combat.Effects;
using Combat.Enums;
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

        ElementMaxAttach = {
            { ElementType.Jin, 2 },
            { ElementType.Mu, 2 },
            { ElementType.Shui, 2 },
            { ElementType.Huo, 2 },
            { ElementType.Tu, 2 },
        }
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

        State.ElementAttach += State.ElementMaxAttach;
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


    // 尝试施加元素击碎
    public void TryApplyElementBreak(CombatantComponent causer, ElementType attack, int layer) {
        if (layer <= 0) return;
        var next = attack.Next();
        if (!State.ElementAttach.ContainsKey(next)) return;
        var cur = State.ElementAttach[next];
        layer = Math.Min(layer, cur);
        State.ElementAttach -= new AddableDict<ElementType, int> {
            { next, layer },
        };
        if (State.ElementAttach.ContainsKey(next)) return;
        // 施加元素击碎效果
        layer = State.ElementMaxAttach[next];
        var broken = EffectBroken.GetElementBroken(next, layer);
        var recover = EffectBroken.GetElementBrokenRecover(next, layer);
        causer.AttachTo(broken,  this);
        causer.AttachTo(recover, this);
    }
}
}
