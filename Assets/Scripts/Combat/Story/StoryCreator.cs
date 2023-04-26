using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Combat.Cards;
using Combat.Enums;
using Combat.States;
using UnityEditor;
using UnityEngine;

namespace Combat.Story {
public static class StoryCreator {
#region 快捷构造

    private static StoryAction Init(CombatState player, CombatState enemy) {
        return new StoryAction {
            Type   = StoryActionType.Init,
            Player = player,
            Enemy  = enemy,
        };
    }

    private static StoryAction Start() {
        return new StoryAction {
            Type = StoryActionType.Start,
        };
    }

    private static StoryAction Say(string character, string content) {
        return new StoryAction {
            Type      = StoryActionType.Say,
            Character = character.Trim(),
            Content   = content.Trim(),
        };
    }

    private static StoryAction Show(string imageName) {
        return new StoryAction {
            Type      = StoryActionType.Show,
            ImageName = imageName.Trim(),
        };
    }

    private static StoryAction Hide() {
        return new StoryAction {
            Type = StoryActionType.Hide,
        };
    }

    private static StoryAction Give(bool isPlayer, IEnumerable<string> cardNames) {
        return new StoryAction {
            Type      = StoryActionType.Give,
            IsPlayer  = isPlayer,
            CardNames = cardNames.ToArray(),
        };
    }

    private static StoryAction Wait(bool isPlayer, IEnumerable<string> cardNames) {
        return new StoryAction {
            Type      = StoryActionType.Wait,
            IsPlayer  = isPlayer,
            CardNames = cardNames.ToArray(),
        };
    }

    private static StoryAction Next() {
        return new StoryAction {
            Type = StoryActionType.Next,
        };
    }

    private static StoryAction WaitEnd() {
        return new StoryAction {
            Type = StoryActionType.WaitEnd,
        };
    }

#endregion

    private static StoryAction ParserInit(IReadOnlyList<string> args) {
        CombatState combatState = new();
        var lines = args[1].Split(';');
        foreach (var line in lines) {
            var assignment = line.Split('=');
            if (assignment.Length != 2) {
                continue;
            }
            var attr = assignment[0];
            var value = assignment[1];
            switch (attr) {
                case "生命值":
                    combatState.HealthMaxBase = float.Parse(value);
                    break;
                case "法力值":
                    combatState.ManaMaxBase = float.Parse(value);
                    break;
                case "初始手牌":
                    combatState.InitCardCnt = int.Parse(value);
                    break;
                case "回合抽牌":
                    combatState.GetCardCnt = int.Parse(value);
                    break;
                case "最大手牌":
                    combatState.MaxCardCnt = int.Parse(value);
                    break;
                case "牌堆": {
                    var cardNames = value.Split(',');
                    foreach (var cardName in cardNames) {
                        if (CardDetails.NameToCard.TryGetValue(cardName, out var card)) {
                            combatState.Heap.AddCard(card);
                        }
                    }

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
                    combatState.ElementMaxAttach.Add(elem, int.Parse(value));
                    break;
                }
            }
        }
        return args[0] switch {
            "player" => Init(combatState, null),
            "enemy"  => Init(null,        combatState),
            _        => null
        };
    }

    private static StoryAction ParserSay(IReadOnlyList<string> args) {
        if (args.Count == 1) args = args[0].Split(':');
        return args.Count == 2 ? Say(args[0], args[1]) : Say("旁白", args[0]);
    }

    // 解析段落代码,返回null表示忽略
    private static StoryAction Parser(string command, IReadOnlyList<string> args) {
        return command switch {
            "comment"  => null,
            "init"     => ParserInit(args),
            "start"    => Start(),
            "say"      => ParserSay(args),
            "show"     => Show(args[0]),
            "hide"     => Hide(),
            "give"     => Give(args[0] == "player", args[1].Split(',')),
            "wait"     => Wait(args[0] == "player", args[1].Split(',')),
            "next"     => Next(),
            "wait_end" => WaitEnd(),
            _          => null
        };
    }

    // 剧本解析算法
    private static IEnumerable<StoryAction> Parser(string text) {
        var ret = new List<StoryAction>();
        var lines = text.Split('\n');
        var section = "";
        var mutStr = "";
        var mut = false;
        for (int i = 0, n = lines.Length; i < n; ++i) {
            var cur = lines[i].Trim();
            if (string.IsNullOrEmpty(cur)) continue;
            var change = cur.EndsWith("\"\"\"");
            if (change) cur = cur.Remove(cur.Length - 3, 3);

            if (mut) mutStr += cur;
            else section    += cur;

            if (change) mut ^= true;

            if (mut) continue;
            var parts = Regex.Split(section, @"\s+");
            var command = parts[0];
            if (command[0] == '@') {
                ret.Add(Parser(command[1..], parts.Skip(1).Append(mutStr).Where(v => !string.IsNullOrWhiteSpace(v)).ToArray()));
            }
            section = "";
            mutStr  = "";
        }

        return ret.Where(c => c != null);
    }

    public static StoryAction[] Load(string name) {
        try {
            return Parser(Resources.Load<TextAsset>($"Stories/{name}").text).ToArray();
        }
        catch (Exception e) {
            Debug.LogException(e);
            return new StoryAction[] { };
        }
    }

    [MenuItem("Tests/剧本加载测试")]
    public static void Test() {
        // Debug.Log(Parser(@"
        //     @say 神秘文字 """"""
        //     *%￥#）@(^#@$*^驭素*#$(@^))%*@……
        //     （%￥&（￥）%元素……#
        //     @￥%……@诅咒*@￥*￥
        //     ）#%多疑（%￥￥（
        //     """"""
        // ").Count());
        Debug.Log(Load("Jin").Length);
    }
}
}
