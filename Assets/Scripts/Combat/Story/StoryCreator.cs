using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    private static StoryAction ParserSay(IReadOnlyList<string> args) {
        if (args.Count == 1) args = args[0].Split(':');
        return args.Count == 2 ? Say(args[0], args[1]) : Say("旁白", args[0]);
    }

    // Todo! 解析段落代码,返回null表示忽略
    private static StoryAction Parser(string command, IReadOnlyList<string> args) {
        return command switch {
            "comment" => null,
            "start"   => Start(),
            "say"     => ParserSay(args),
            // Todo!

            _ => null
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
