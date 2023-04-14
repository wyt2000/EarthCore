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

        MaxCardCnt = 10,

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

        // 加载固有buff
        foreach (var effect in EffectFixed.GetAll(this)) {
            effect.DoAttach();
        }
    }

#region 发起快捷请求

    // 广播效果回调
    public void BoardCast(Action<Effect> action) {
        Effects.ForEach(action);
    }

    // 广播效果回调直到有一个返回true
    public bool BoardCastAny(Func<Effect, bool> action) {
        return Effects.Any(action);
    }

    // 给自己上buff
    public void AddBuff(Effect effect) {
        AddBuffFrom(effect, this);
    }

    // 给敌人上buff
    public void AddOpBuff(Effect effect) {
        Opponent.AddBuffFrom(effect, this);
    }

    // 上buff来自
    public void AddBuffFrom(Effect effect, CombatantComponent causer) {
        effect.Causer = causer;
        effect.Target = this;
        effect.Attach(this);
    }

    // 治疗自己
    public void Heal(RequestHpChange request) {
        request.Causer = this;
        request.Target = this;
        request.IsHeal = true;
        Judge.Requests.Add(request);
    }

    // 攻击敌人
    public void Attack(RequestHpChange request) {
        request.Causer = this;
        request.Target = Opponent;
        request.IsHeal = false;
        Judge.Requests.Add(request);
    }

    // 对敌人出牌
    public void PlayCard(params Card[] cards) {
        PlayCard((IEnumerable<Card>)cards);
    }

    public void PlayCard(IEnumerable<Card> cards) {
        var arr = cards.ToArray();
        Judge.Requests.Add(new RequestPlayBatchCard {
            Causer = this,
            Target = Opponent,
            Cards  = arr
        });
    }

    // 摸牌 
    public void GetCard(RequestGetCard request) {
        request.Causer = this;
        Judge.Requests.Add(request);
    }

    // 摸牌
    public void GetCard(int cnt) {
        GetCard(new RequestGetCard {
            Count = cnt
        });
    }

    // 能否选择一张牌预备出牌
    public bool CanSelectCardToPlay(Card card) {
        if (State.BlockTags.ContainsKey(CombatBlockTag.BlockPlayCard)) return false;
        var selected = Cards.Where(c => c.IsSelected).ToList();
        if (card.LgUnique && selected.Count > 0) return false;
        if (selected.Any(s => s.LgUnique)) return false;
        if (!card.LgElement.HasValue) return true;
        selected.Add(card);
        // 要么只有<=1种元素,要么能触发元素联动
        var types = selected.Where(t => t.LgElement.HasValue).Select(c => c.LgElement.Value).ToHashSet();
        return types.Count <= 1 || EffectLinks.GetElementLink(types).Item1 != null;
    }

    // 取消选择一张牌
    public void UnSelectAllCardToPlay() {
        Cards.ForEach(c => c.IsSelected = false);
        // Todo 按原有顺序重新尝试选择card
    }

    // 弃牌
    public void Discard() {
        // 选中的弃掉
        var card = Cards.Extract(c => c.IsSelected);
        // 超过上限的也弃掉 Todo 优化自动弃牌逻辑 
        card.AddRange(Cards.Extract((_, i) => i >= State.MaxCardCnt));
        Judge.Requests.Add(new RequestAnimation {
            Anim = () => cardSlot.Discards(card)
        });
        Judge.Requests.Add(new RequestPostLogic {
            OnFinish = () => Judge.NextTurn()
        });
    }

    // 尝试施加元素击碎
    public void TryApplyElementBreak(CombatantComponent causer, ElementType attack, int layer) {
        if (layer <= 0) return;
        var next = attack.Next();
        if (!State.ElementAttach.ContainsKey(next)) return;
        var cur = State.ElementAttach[next];
        layer = Math.Min(layer, cur);
        State.ElementAttach -= new CompactDict<ElementType, int> {
            { next, layer },
        };
        if (State.ElementAttach.ContainsKey(next)) return;
        // 施加元素击碎效果
        layer = State.ElementMaxAttach[next];
        var broken = EffectBroken.GetElementBroken(next, layer);
        var recover = EffectBroken.GetElementBrokenRecover(next, layer);
        AddBuffFrom(broken,  causer);
        AddBuffFrom(recover, causer);
    }

#endregion
}
}
