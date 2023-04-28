using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Cards;
using Combat.Enums;
using Combat.States;
using Controllers;

namespace Combat.Story.Actions {
/*
// 设置初始状态,设置牌堆可以在顶部额外放置指定的牌
@init {player|enemy} """
名字={value};
主属性={value};
生命值={value};
法力值={value};
初始手牌={value};
回合抽牌={value};
最大手牌={value};
[金木水火土]法印={value};
牌堆={card_name,}*;
"""
 */
public class ActionInit : StoryAction {
    private CombatState m_player, m_enemy;

    public override StoryAction Build(IReadOnlyList<string> args) {
        CombatState state = new();
        var lines = args[1].Split(';');
        foreach (var line in lines) {
            var assignment = line.Trim().Split('=');
            if (assignment.Length != 2) {
                continue;
            }
            var attr = assignment[0];
            var value = assignment[1];
            switch (attr) {
                case "名字":
                    state.SpritePath = $"Textures/Chars/{value}";
                    break;
                case "主属性":
                    state.SpriteColor = value[0] switch {
                        '金' => ElementType.Jin,
                        '木' => ElementType.Mu,
                        '水' => ElementType.Shui,
                        '火' => ElementType.Huo,
                        '土' => ElementType.Tu,
                        _   => null
                    };
                    break;
                case "生命值":
                    state.HealthMaxBase = float.Parse(value);
                    break;
                case "法力值":
                    state.ManaMaxBase = float.Parse(value);
                    break;
                case "初始手牌":
                    state.InitCardCnt = int.Parse(value);
                    break;
                case "回合抽牌":
                    state.GetCardCnt = int.Parse(value);
                    break;
                case "最大手牌":
                    state.MaxCardCnt = int.Parse(value);
                    break;
                case "牌堆": {
                    var cardNames = value.Split(',');
                    foreach (var cardName in cardNames.Reverse()) {
                        if (CardDetails.NameToCard.TryGetValue(cardName, out var card)) {
                            state.Heap.AddCard(card);
                        }
                    }
                    state.Heap.AllCards.Reverse();
                    break;
                }
                default: {
                    if (attr[1..3] != "法印") continue;
                    var elem = attr[0] switch {
                        '金' => ElementType.Jin,
                        '木' => ElementType.Mu,
                        '水' => ElementType.Shui,
                        '火' => ElementType.Huo,
                        '土' => ElementType.Tu,
                        _   => throw new ArgumentOutOfRangeException()
                    };
                    state.ElementMaxAttach.Add(elem, int.Parse(value));
                    break;
                }
            }
        }

        switch (args[0]) {
            case "player":
                m_player = state;
                return this;
            case "enemy":
                m_enemy = state;
                return this;
            default:
                return null;
        }
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant.Judge.Players.First(c => c.isOtherPlayer == (m_enemy != null));
        var data = combatant.isOtherPlayer ? m_enemy : m_player;
        if (data != null) combatant.InitState(data);
        return null;
    }

    protected override string ToDescription() {
        var name = m_player != null ? "玩家" : "怪物";
        return $"初始化{name}战斗状态";
    }
}
}
