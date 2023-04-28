using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Cards;
using Combat.Effects;
using Combat.Enums;
using Combat.Requests.Details;
using Combat.States;
using Controllers;
using GUIs;
using GUIs.Audios;
using UnityEngine;
using Utils;

namespace Combat {
// 最基础的战斗对象组件
[RequireComponent(typeof(CombatController))]
public class CombatantComponent : MonoBehaviour {
#region prefab配置

    public PlayerView view;

    // 是否为对面的玩家
    public bool isOtherPlayer;

#endregion

    [NonSerialized]
    public CombatJudge Judge;

    [NonSerialized]
    public CombatantComponent Opponent;

    public CombatState State;

    public SortedSet<Effect> Effects => State.Effects;

    public List<Card> Cards => State.Cards;

    public CardHeap Heap => State.Heap;

    [NonSerialized]
    public CombatController Controller;

    public void InitState(CombatState state) {
        gameObject.SetActive(true);

        state.Health = state.HealthMax;
        state.Mana   = state.ManaMax;

        state.ElementAttach += state.ElementMaxAttach;

        State = state;

        // 加载固有buff
        foreach (var effect in EffectFixed.GetAll(this)) {
            AddBuff(effect);
        }

        view.stateBar.Init();

        State.OnStateChange += (_, _, delta) =>
        {
            BoardCast(effect => effect.AfterStateChange(delta));
        };

        view.painting.FreshUI(State);
        FreshHeap();
    }

    private void Start() {
        view.Init(this);
        gameObject.SetActive(false);
        Controller = GetComponent<CombatController>();
    }

    public void FreshHeap() {
        var heap = view.cardHeap;
        heap.Count = Heap.AllCards.Count;
        heap.DisCount = Heap.DiscardCount;
        heap.FreshUI();
    }

#region 发起快捷请求

    public void AddPost(Action action) {
        Judge.Requests.AddPost(new RequestPostLogic {
            Causer   = this,
            OnFinish = action
        });
    }

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

    // 出选中的牌
    public void PlaySelectedCard() {
        PlayCard(Cards.Where(c => c.IsSelected));
    }

    // 对敌人出牌
    public void PlayCard(params Card[] cards) {
        PlayCard((IEnumerable<Card>)cards);
    }

    public void PlayCard(IEnumerable<Card> cards) {
        var arr = cards.ToArray();
        var batch = new RequestPlayBatchCard {
            Causer = this,
            Target = Opponent,
            Judge  = Judge,
            Cards  = arr
        };
        if (!isOtherPlayer && !batch.CanEnqueue()) {
            GAudio.PlayInvalidCard();
        }
        else {
            Judge.Requests.Add(batch);
        }
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
        card.IsSelected = true;
        var ret = PreviewBatch.EvaluateState();
        card.IsSelected = false;
        return ret != BatchCardState.CannotSelect;
    }

    // 弃牌
    public void Discard() {
        // 选中的弃掉
        var card = Cards.Extract(c => c.IsSelected);
        // 超过上限的也弃掉
        card.AddRange(Cards.Extract((_, i) => i >= State.MaxCardCnt));
        GAudio.PlayDiscardCard();
        Judge.Requests.Add(new RequestAnimation {
            Causer = this,
            Anim   = () => view.cardSlot.Discards(card)
        });
        AddPost(() => Judge.NextTurn());
    }

    // 尝试施加元素击碎
    public void TryApplyElementBreak(CombatantComponent causer, ElementType type, int layer) {
        if (layer <= 0) return;
        ref var attach = ref State.ElementAttach;
        if (!attach.ContainsKey(type)) return;
        var cur = attach[type];
        layer = Math.Min(layer, cur);
        // 移除元素层数
        attach -= new CompactDict<ElementType, int> { { type, layer } };
        if (attach.ContainsKey(type)) return;
        // 施加元素击碎效果
        layer = State.ElementMaxAttach[type];
        var broken = EffectBroken.GetElementBroken(type, layer);
        var recover = EffectBroken.GetElementBrokenRecover(type, layer);
        AddBuffFrom(broken,  causer);
        AddBuffFrom(recover, causer);
    }

#endregion

    public RequestPlayBatchCard PreviewBatch => new() {
        Causer = this,
        Target = Opponent,
        Judge  = Judge,
        Cards  = Cards.Where(c => c.IsSelected).ToArray()
    };
}
}
