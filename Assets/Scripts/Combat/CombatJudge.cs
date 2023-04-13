using System.Collections;
using Combat.Requests;
using Controllers;
using GUIs;
using UnityEngine;

namespace Combat {
// 对局裁判,管理各种请求
public class CombatJudge : MonoBehaviour {
    // 日志
    public JudgeLogView logger;

    // 先手
    [SerializeField]
    private CombatantComponent playerA;

    // 后手
    [SerializeField]
    private CombatantComponent playerB;

    // 战斗双方
    private readonly CombatantComponent[] m_combatants = new CombatantComponent[2];

    // 是否开始
    private bool m_start;

    // 当前回合
    private int m_round;

    private CombatantComponent CurrentComp => m_combatants[m_round % 2];

    private CombatantComponent NextComp => m_combatants[(m_round + 1) % 2];

    public readonly CombatRequestList Requests = new();

    public CombatJudge() {
        Requests.Judge = this;
    }

#region 纯逻辑

    // 设置战斗双方
    private void Init(CombatantComponent combatant1, CombatantComponent combatant2) {
        m_combatants[0] = combatant1;
        m_combatants[1] = combatant2;

        combatant1.Judge    = combatant2.Judge = this;
        combatant1.Opponent = combatant2;
        combatant2.Opponent = combatant1;

        m_round = 0;
    }

    private const int InitCardCount = 5;

    // 战斗开始事件
    private void CombatStart() {
        m_round = 0;
        m_start = true;

        // 初始摸牌
        CurrentComp.GetCard(InitCardCount);
        NextComp.GetCard(InitCardCount);

        TurnStart();
    }

    // 切换回合
    private void SwitchTurn() {
        m_round++;
    }

    // 回合开始事件
    private void TurnStart() {
        CurrentComp.BoardCast(e => e.AfterTurnStart());
    }

    // 回合结束事件
    private void TurnEnd() {
        CurrentComp.BoardCast(e => e.AfterTurnEnd());
    }

    // 下一回合
    public void NextTurn() {
        TurnEnd();
        SwitchTurn();
        TurnStart();
    }

    // 处理所有请求
    private void DealAllTask() {
        StartCoroutine(Requests.RunAll());
    }

#endregion

#region 脚本逻辑

    private IEnumerator Start() {
        // 检查ab
        if (playerA == null || playerB == null) {
            Debug.LogError("PlayerA or PlayerB not set");
            yield break;
        }

        Init(playerA, playerB);

        // Todo 开始动画
        yield return new WaitForSeconds(1);

        CombatStart();
    }

    private void Update() {
        if (!m_start || Requests.Running) return;
        var current = CurrentComp;
        current.GetComponent<CombatController>()?.OnUserInput();
        if (Requests.Count > 0) DealAllTask();
        if (current.State.IsDead || NextComp.State.IsDead) {
            // Todo 结束动画
            m_start = false;
        }
    }

#endregion
}
}
