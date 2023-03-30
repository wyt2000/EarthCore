using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Cards;
using Combat.Effects;
using Combat.Requests;
using Designs;
using TMPro;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Combat {
// 最基础的战斗对象组件
public class CombatantComponent : MonoBehaviour {
    [NonSerialized]
    public CombatJudge Judge;

    public readonly CombatState State = new() {
        HealthMaxBase    = 1000,
        HealthMaxPercent = 0,
        HealthMaxExtra   = 0,

        ManaMaxBase    = 150,
        ManaMaxPercent = 0,
        ManaMaxExtra   = 0,

        PhysicalArmorBase    = 5,
        PhysicalArmorPercent = 0,
        PhysicalArmorExtra   = 0,

        MagicResistanceBase    = 5,
        MagicResistancePercent = 0,
        MagicResistanceExtra   = 0,

        Level = 1,
    };

    // 玩家的效果
    public readonly ISet<Effect> Effects = new SortedSet<Effect>();

    // 玩家的手牌
    public readonly IList<Card> Cards = new List<Card>();

    // 玩家的牌堆
    public readonly List<Card> AllCards = new List<Card>() {
        CardDetails.Awakening(),
        CardDetails.Probing(),
        CardDetails.Drain(),
        CardDetails.Thirst(),
        CardDetails.FireSuppression(),
        CardDetails.FireSummon(),
        CardDetails.MagicGuard(),
        CardDetails.ManaSurge(),
    };

    public TextMeshPro statusBoard;

    public void FreshUI() {
        var str = $"{name} status : \n";
        str += $"生命值 : {State.Health:F0}/{State.HealthMax} \n";
        str += $"魔法值 : {State.Mana:F0}/{State.ManaMax} \n";
        str += $"护甲 : {State.PhysicalArmor} \n";
        str += $"魔抗 : {State.MagicResistance} \n";
        str += "当前效果 : \n";
        str = Effects.Aggregate(str, (current, e) =>
            current +
            $" - {e.UiName} , 剩余回合 : {e.LgRemainingRounds} \n"
        );

        str += "当前手牌 : \n";
        str = Cards.Aggregate(str, (current, c) =>
            current +
            $" - [{c.LgElement?.ToDescription() ?? "无"}]{c.UiName}({c.GetManaCost()}) : {c.UiDescription} \n"
        );
        str += $"当前牌堆 : {AllCards.Count} \n";

        statusBoard.text = str;
    }

    private void Start() {
        State.Health = State.HealthMax / 2;
        State.Mana   = State.ManaMax / 2;
        while (AllCards.Count < 30) {
            AllCards.Add(AllCards[Random.Range(0, AllCards.Count)]);
        }
    }

    public void Attach(Effect effect) {
        effect.Attach(this);
    }

    public void Attack(CombatantComponent target, HealthRequest request) {
        request.Causer = this;
        request.Target = target;
        request.IsHeal = false;
        Judge.AddHealthTask(request);
    }

    public void HealSelf(HealthRequest request) {
        request.Causer = this;
        request.Target = this;
        request.IsHeal = true;
        Judge.AddHealthTask(request);
    }

    public void PlayCard(CombatantComponent target, PlayCardRequest request) {
        request.Causer = this;
        request.Target = target;
        Judge.AddPlayCardTask(request);
    }

    public void GetCard(GetCardRequest request) {
        request.Causer = this;
        Judge.AddGetCardTask(request);
    }

    public void BoardCast(Action<Effect> action) {
        Effects.ForEach(action);
    }

    public bool BoardCastAny(Func<Effect, bool> action) {
        return Effects.Any(action);
    }
}
}
