using System.Collections.Generic;
using Combat.Effects.Details;
using Combat.Requests;
using UnityEngine;
using Utils;

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
    public EffectRequest   Effect   = null;
    public HealthRequest   Health   = null;
    public PlayCardRequest PlayCard = null;
}

// 对局裁判,管理各种请求
public class CombatJudge : MonoBehaviour {
    [SerializeField]
    // 先手
    private CombatantComponent playerA;

    [SerializeField]
    // 后手
    private CombatantComponent playerB;

    // 战斗双方
    private readonly CombatantComponent[] m_combatants = new CombatantComponent[2];

    // 当前回合
    private int m_round = 0;

    // 任务队列
    private readonly Queue<RequestTask> m_taskQueue = new();

    private CombatantComponent CurrentComp => m_combatants[m_round % 2];

    private CombatantComponent NextComp => m_combatants[(m_round + 1) % 2];

#region 纯逻辑

    // 设置战斗双方
    private void Init(CombatantComponent combatant1, CombatantComponent combatant2) {
        m_combatants[0] = combatant1;
        m_combatants[1] = combatant2;

        combatant1.Judge = combatant2.Judge = this;

        m_round = 0;
    }

    // 战斗开始事件
    private void CombatStart() {
        m_round = 0;
    }

    // 切换回合
    private void SwitchTurn() {
        m_round++;
    }

    // 回合开始事件
    private void TurnStart() {
        CurrentComp.BoardCast(e => e.BeforeTurnStart());
    }

    // 回合结束事件
    private void TurnEnd() {
        CurrentComp.BoardCast(e => e.AfterTurnEnd());
    }

    // 下一回合
    private void NextTurn() {
        TurnEnd();
        SwitchTurn();
        TurnStart();
    }

#endregion

#region 请求系统

    // effect请求
    public void AddEffectTask(EffectRequest request) {
        m_taskQueue.Enqueue(new RequestTask { Effect = request });
    }

    // health请求
    public void AddHealthTask(HealthRequest request) {
        if (request.Target == null || request.Causer == null || request.Value < 0) {
            Debug.LogWarning($"Invalid request by {request.Causer.name}");
            return;
        }

        m_taskQueue.Enqueue(new RequestTask { Health = request });
    }

#region 请求系统具体逻辑

    private static void DealEffectTask(EffectRequest request) {
        var effect = request.Effect;
        var attach = request.Attach;
        if (attach) {
            effect.DoAttach();
        }
        else {
            effect.DoRemove();
        }
    }

    private static void DealHealthTask(HealthRequest request) {
        request.Target.State.ApplyHealthChange(request);
    }

    private void DealOneTask() {
        if (m_taskQueue.Count == 0) {
            return;
        }

        var task = m_taskQueue.Dequeue();
        if (task.Effect != null) {
            DealEffectTask(task.Effect);
        }
        else if (task.Health != null) {
            DealHealthTask(task.Health);
        }
        else {
            // Todo 添加其他逻辑
            Debug.LogError("Invalid task");
        }
    }

#endregion

#endregion

#region 脚本逻辑

    private void Start() {
        // 检查ab
        if (playerA == null || playerB == null) {
            Debug.LogError("PlayerA or PlayerB not set");
            return;
        }

        Init(playerA, playerB);
    }

    public TMPro.TextMeshPro text;

    private void Log() {
        var msg = "";
        msg += "S开始游戏,F处理一帧,L刷新日志,空格下一回合,A攻击,H治疗,E效果\n";
        msg += $"Current round: {m_round} \n";
        msg += $"Current task count: {m_taskQueue.Count} \n";
        foreach (var combatant in m_combatants) {
            var str = $"{combatant.name} status : \n";
            str += $"health : {combatant.State.Health} \n";
            str += $"armor : {combatant.State.MagicResistance} \n";
            str += $"effects : \n";
            combatant.Effects.ForEach(e => str += $" - {e.Name} , remain : {e.RemainingRounds} \n");
            msg += str;
        }

        text.text = msg;
    }

    private bool m_start;

    private void Update() {
        // start game
        if (!m_start) {
            CombatStart();
            TurnStart();
            Log();
            m_start = true;
        }

        // deal frame
        if (Input.GetKeyDown(KeyCode.F)) {
            DealOneTask();
            Log();
        }

        // log 
        if (Input.GetKeyDown(KeyCode.L)) {
            Log();
        }

        // next round
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (m_taskQueue.Count > 0) DealOneTask();
            else NextTurn();
            Log();
        }

        var current = CurrentComp;
        var next    = NextComp;

        // attack test
        if (Input.GetKeyDown(KeyCode.A)) {
            current.Attack(next, new HealthRequest {
                Value = 40,
                DamageParams = {
                    DamageType = DamageType.Magical
                }
            });
            Log();
        }

        // heal test
        if (Input.GetKeyDown(KeyCode.H)) {
            current.HealSelf(new HealthRequest {
                Value = 10,
            });
            Log();
        }

        // effect test
        if (Input.GetKeyDown(KeyCode.E)) {
            current.Attach(EffectDetails.Fire_Earth());
            Log();
        }
    }

#endregion
}
}
