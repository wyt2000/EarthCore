using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Combat.Story {
public static class StoryCreator {
    // 解析段落代码,返回null表示忽略
    private static StoryAction Parser(string command, IReadOnlyList<string> args) {
        var action = StoryAction.GetAction(command);
        return action?.Build(args);
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
}
}
