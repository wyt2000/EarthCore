using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Effects;
using Combat.Requests;
using UnityEngine;
using Utils;

namespace Combat {
// 最基础的战斗对象组件 Todo
public class CombatantComponent : MonoBehaviour {
    [NonSerialized]
    public CombatJudge Judge;

    public readonly CombatState State = new() {
        HealthMaxBase    = 100,
        HealthMaxPercent = 0,
        HealthMaxExtra   = 0,

        ManaMaxBase    = 100,
        ManaMaxPercent = 0,
        ManaMaxExtra   = 0,

        PhysicalArmorBase    = 10,
        PhysicalArmorPercent = 0,
        PhysicalArmorExtra   = 0,

        MagicResistanceBase    = 10,
        MagicResistancePercent = 0,
        MagicResistanceExtra   = 0,
    };

    public readonly IList<Effect> Effects = new List<Effect>();

    private void Start() {
        State.Health = State.HealthMax;
        State.Mana   = State.ManaMax;
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

    public void BoardCast(Action<Effect> action) {
        // Todo 同优先级是否保持顺序?
        Effects.OrderBy(v => v.Priority).ForEach(action);
    }

    public bool BoardCastAny(Func<Effect, bool> action) {
        return Effects.OrderBy(v => v.Priority).Any(action);
    }
}
}
