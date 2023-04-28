using System.Collections;
using Combat.Requests;
using Combat.Requests.Details;
using Combat.Story;
using GUIs;
using GUIs.Animations;
using GUIs.Audios;
using GUIs.Globals;
using UnityEngine;
using Utils;

namespace Combat {
// 对局裁判,管理各种请求
public class CombatJudge : MonoBehaviour {
    [SerializeField]
    // 所有战斗场景
    private GameObject combat;

    // 日志
    public JudgeLogView logger;

    // 对话框 
    public DialogView dialog;

    // 帮助框
    public HelpView help;

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

    public CombatantComponent CurrentComp => Players[m_round % 2];

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

    // 战斗开始事件
    public IEnumerator CombatStart() {
        GAudio.StartBattleBGM();
        combat.SetActive(true);

        logger.AddLog("游戏准备中...");

        m_round = 0;
        m_start = true;

        // Todo 开始动画
        yield return GAnimation.Wait(1);

        logger.AddLog("游戏开始");

        // 初始摸牌
        CurrentComp.GetCard(CurrentComp.State.InitCardCnt);
        NextComp.GetCard(NextComp.State.InitCardCnt);

        TurnStart();
    }

    // 战斗终止事件
    public IEnumerator CombatEnd() {
        m_start = false;

        // Todo 结束动画
        yield return GAnimation.Wait(1);

        logger.AddLog("游戏结束");

        combat.SetActive(true);
        GAudio.StopBattleBGM();
    }

    // 切换回合
    private void SwitchTurn() {
        m_round++;
    }

    // 回合开始事件
    private void TurnStart() {
        var cur = CurrentComp;
        Requests.Add(new RequestLogic {
            Causer = cur,
            Logic  = () => cur.BoardCast(e => e.AfterTurnStart())
        });
    }

    // 回合结束事件
    private void TurnEnd() {
        var cur = CurrentComp;
        Requests.Add(new RequestLogic {
            Causer = cur,
            Logic  = () => cur.BoardCast(e => e.AfterTurnEnd())
        });
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
        Script = new StoryScript("Jin");

        // 检查ab
        if (playerA == null || playerB == null) {
            Debug.LogError("PlayerA or PlayerB not set");
            yield break;
        }

        Init(playerA, playerB);
    }

    private IEnumerator m_iter;

    private bool m_iterFinish = true;

    // 当前剧本
    public StoryScript Script;

    private void Update() {
        if (Requests.Running) return;
        if (m_iterFinish) {
            m_iterFinish = false;

            m_iter = CurrentComp.Controller.OnDoAction()?.Stack();
        }
        if (m_iter == null || !m_iter.MoveNext()) {
            m_iterFinish = true;

            m_iter = null;
        }
        if (m_start && Requests.Count > 0) DealAllTask();
        if (!m_start && m_iterFinish) {
            // Todo! 返回选关场景
        }
    }

#endregion
}
}
