using Combat.Effects;
using Combat.Requests;
using UnityEngine;

namespace Combat {
/// <summary>
/// 请求任务(每种请求都有动画呈现),包括:
/// 1.效果附着/消失请求
/// 2.生命值变化请求
/// 3.出牌请求
/// 4.摸牌请求
/// 5.过牌请求
/// </summary>
public class RequestTask {
    public readonly EffectRequest   Effect   = null;
    public readonly HealthRequest   Health   = null;
    public readonly PlayCardRequest PlayCard = null;
}

// 对局裁判,管理各种请求
public class CombatJudge {
    // 战斗双方
    private readonly CombatantComponent[] m_combatants = new CombatantComponent[2];

    // 当前回合
    private int m_round = 0;

#region 纯逻辑

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

#endregion

#region 请求系统

    // Todo 加入事件队列机制,管理effect的迁移
    public void EnqueueEffectTask(EffectRequest request) {
        var effect = request.Effect;
        var attach = request.Attach;
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

#endregion
}
}
