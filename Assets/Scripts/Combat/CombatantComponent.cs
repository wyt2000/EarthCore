using System.Collections.Generic;
using Combat.Effects;
using Combat.Requests;
using UnityEngine;

namespace Combat {
// 最基础的战斗对象组件 Todo
public class CombatantComponent : MonoBehaviour {
    public CombatJudge Judge;

    public readonly CombatState   State   = new();
    public readonly IList<Effect> Effects = new List<Effect>();

    public void Attach(Effect effect) {
        effect.Attach(this);
    }

    public void Attack(CombatantComponent target, HealthRequest request) {
        request.Causer = this;
        request.Target = target;
        request.IsHeal = false;
        Judge.EnqueueHealthTask(request);
    }

    public void HealSelf(HealthRequest request) {
        request.Causer = this;
        request.Target = this;
        request.IsHeal = true;
        Judge.EnqueueHealthTask(request);
    }
}
}
