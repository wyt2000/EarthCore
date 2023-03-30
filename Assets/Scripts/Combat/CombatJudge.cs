using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Combat.Requests;
using Designs;
using UnityEngine;
using Utils;

namespace Combat {
// 对局裁判,管理各种请求
public class CombatJudge : MonoBehaviour {
    /// <summary>
    /// 请求任务(每种请求都有动画呈现),包括:
    /// 1.效果附着/消失请求
    /// 2.生命值变化请求
    /// 3.出牌请求
    /// 4.摸牌请求
    /// 5.过牌请求
    /// </summary>
    private class RequestTask {
        public RequestType     Type;
        public EffectRequest   Effect;
        public HealthRequest   Health;
        public PlayCardRequest PlayCard;
        public GetCardRequest  GetCard;
    }

    private enum RequestType {
        [Description("效果")]
        Effect,

        [Description("生命")]
        Health,

        [Description("出牌")]
        PlayCard,

        [Description("摸牌")]
        GetCard,

        [Description("过牌")]
        PassCard,
    }

    [SerializeField]
    // 先手
    private CombatantComponent playerA;

    [SerializeField]
    // 后手
    private CombatantComponent playerB;

    // 战斗双方
    private readonly CombatantComponent[] m_combatants = new CombatantComponent[2];

    // 当前回合
    private int m_round;

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

#region 公开接口

    // effect请求
    public void AddEffectTask(EffectRequest request) {
        if (request.Effect == null || request.Effect.Target == null) {
            Debug.LogWarning("Invalid effect request");
            return;
        }

        m_taskQueue.Enqueue(new RequestTask {
            Type   = RequestType.Effect,
            Effect = request,
        });
    }

    // health请求
    public void AddHealthTask(HealthRequest request) {
        if (request.Target == null || request.Causer == null || request.Value < 0) {
            Debug.LogWarning("Invalid health request");
            return;
        }

        m_taskQueue.Enqueue(new RequestTask {
            Type   = RequestType.Health,
            Health = request,
        });
    }

    // playCard请求
    public void AddPlayCardTask(PlayCardRequest request) {
        if (m_taskQueue.Count > 0) {
            Debug.LogWarning("Cannot play card when there are tasks in queue");
            return;
        }

        if (request.Causer == null || request.Cards == null || request.Cards.Length == 0) {
            Debug.LogWarning("Invalid card request");
            return;
        }

        request.Cards.ForEach(c => c.LgManaCost = c.GetManaCost());
        request.ManaCost = request.Cards.Sum(c => c.LgManaCost);
        request.Causer.BoardCast(e => e.BeforePlayCard(request));

        var manaCost = request.ManaCost;
        if (manaCost > request.Causer.State.Mana) {
            Debug.LogWarning("Not enough mana");
            return;
        }

        // 扣除法力值
        request.Causer.State.Mana -= manaCost;

        // Todo 计算元素联动

        foreach (var card in request.Cards) {
            m_taskQueue.Enqueue(new RequestTask {
                Type = RequestType.PlayCard,
                PlayCard = new PlayCardRequest {
                    Causer   = request.Causer,
                    Target   = request.Target,
                    Cards    = request.Cards,
                    ManaCost = manaCost,
                    Current  = card,
                },
            });
        }

        AddLogicTask(() => request.Causer.BoardCast(e => e.AfterPlayCard(request)));
    }

    // getCard请求
    public void AddGetCardTask(GetCardRequest request) {
        if (request.Causer == null) {
            Debug.LogWarning("Invalid card request");
            return;
        }

        m_taskQueue.Enqueue(new RequestTask {
            Type    = RequestType.GetCard,
            GetCard = request,
        });
    }

#endregion

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

    private static void DealPlayCardTask(PlayCardRequest request) {
        request.Current.PlayCard(request);
    }

    private static void DealGetCardTask(GetCardRequest request) {
        var owner = request.Causer;
        var heap = owner.AllCards;
        var cards = owner.Cards;
        var count = request.Count;
        var filter = request.Filter;
        var selector = request.SelectIndex;
        var valid = heap.Where(filter).ToList();
        // Todo 优化摸牌算法到O(n)
        while (count-- > 0) {
            var index = selector(valid.Count);
            index = Math.Clamp(0, index, valid.Count);
            if (index == valid.Count) continue;
            var card = valid[index];
            card.Owner = owner;
            cards.Add(card);
            valid.RemoveAt(index);
            heap.Remove(card);
        }
    }

    private void DealOneTask() {
        if (m_taskQueue.Count == 0) {
            return;
        }

        var task = m_taskQueue.Dequeue();
        switch (task.Type) {
            case RequestType.Effect:
                DealEffectTask(task.Effect);
                break;
            case RequestType.Health:
                DealHealthTask(task.Health);
                break;
            case RequestType.PlayCard:
                DealPlayCardTask(task.PlayCard);
                break;
            case RequestType.GetCard:
                DealGetCardTask(task.GetCard);
                break;
            case RequestType.PassCard:
                break;
            default:
                Debug.LogError("Invalid task");
                break;
        }
    }

#endregion

#endregion

    // Todo 加入摸牌出牌逻辑

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
        msg += "S开始,F一帧,L日志,空格切人,A攻击,H治疗,E效果,M摸牌,1~n出牌\n";
        msg += $"当前轮次: {m_round / 2} , 轮到{CurrentComp.name} \n";
        msg += $"剩余{m_taskQueue.Count}个task : ";
        msg += string.Join(", ", m_taskQueue.Select(v => v.Type.ToDescription()));
        m_combatants.ForEach(v => v.FreshUI());

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
        var next = NextComp;

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

        // get card test
        if (Input.GetKeyDown(KeyCode.M)) {
            current.GetCard(new GetCardRequest {
                Count = 1,
            });
            Log();
        }

        // play card test
        var keys = new[] {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
        for (var i = 0; i < keys.Length; i++) {
            if (i >= current.Cards.Count) continue;
            if (!Input.GetKeyDown(keys[i])) continue;
            current.PlayCard(next, new PlayCardRequest {
                Cards = new[] { current.Cards[i] }
            });
            Log();
        }
    }

#endregion
}
}
