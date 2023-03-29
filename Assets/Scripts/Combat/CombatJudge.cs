using Combat.Effects;
using Combat.Requests;
using UnityEngine;

namespace Combat {
public class CombatJudge {
    // 战斗双方
    private readonly CombatantComponent[] m_combatants = new CombatantComponent[2];

    // 当前回合
    private int m_round = 0;

    // 设置战斗双方
    public void Init(CombatantComponent combatant1, CombatantComponent combatant2) {
        m_combatants[0] = combatant1;
        m_combatants[1] = combatant2;

        combatant1.Judge = combatant2.Judge = this;

        m_round = 0;
    }

    // 战斗开始
    public void CombatStart() {
        // 触发战斗回合开始事件
    }

    // Todo 加入事件队列机制,管理effect的迁移
    public void EnqueueEffectTask(Effect effect, bool attach) {
        if (attach) {
            effect.DoAttach();
        }
        else {
            effect.DoRemove();
        }
    }

    // Todo 加入事件队列机制,应该等待当前攻击动画执行完了再执行下一次伤害
    public void EnqueueHealthTask(HealthRequest request) {
        if (request.Target == null || request.Causer == null || request.Value < 0) {
            Debug.LogWarning($"Invalid request by {request.Causer.name}");
            return;
        }

        request.Target.State.ApplyHealthChange(request);
    }
}
}
