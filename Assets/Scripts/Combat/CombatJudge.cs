﻿using System.Collections;
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

    // 是否开始
    private bool m_start;

    // 当前回合
    private int m_round;

    // 请求队列
    public readonly CombatRequestList Requests = new();

    // 战斗双方
    public readonly CombatantComponent[] Players = new CombatantComponent[2];

    private CombatantComponent CurrentComp => Players[m_round % 2];

    private CombatantComponent NextComp => Players[(m_round + 1) % 2];

    public CombatJudge() {
        Requests.Judge = this;
    }

#region 纯逻辑

    // 设置战斗双方
    private void Init(CombatantComponent combatant1, CombatantComponent combatant2) {
        Players[0] = combatant1;
        Players[1] = combatant2;

        combatant1.Judge    = combatant2.Judge = this;
        combatant1.Opponent = combatant2;
        combatant2.Opponent = combatant1;

        m_round = 0;
    }

    private const int InitCardCount = 5;

    // 战斗开始事件
    private IEnumerator CombatStart() {
        logger.AddLog("游戏准备中...");

        m_round = 0;
        m_start = true;

        // Todo 开始动画
        yield return new WaitForSeconds(1);

        logger.AddLog("游戏开始");

        // 初始摸牌
        CurrentComp.GetCard(InitCardCount);
        NextComp.GetCard(InitCardCount);

        TurnStart();
    }

    // 战斗终止事件
    private IEnumerator CombatEnd() {
        // Todo 结束动画
        yield return new WaitForSeconds(1);

        logger.AddLog("游戏结束");
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

        yield return CombatStart();
    }

    private IEnumerator m_iter;
    private bool        m_iterFinish = true;

    private void Update() {
        if (!m_start || Requests.Running) return;
        if (CurrentComp.State.IsDead || NextComp.State.IsDead) {
            m_start = false;
            StartCoroutine(CombatEnd());
            return;
        }
        if (m_iterFinish) {
            m_iterFinish = false;

            m_iter = CurrentComp.GetComponent<CombatController>()?.OnUserInput();
        }
        if (m_iter == null || !m_iter.MoveNext()) {
            m_iterFinish = true;

            m_iter = null;
        }
        if (Requests.Count > 0) DealAllTask();
    }

#endregion
}
}
