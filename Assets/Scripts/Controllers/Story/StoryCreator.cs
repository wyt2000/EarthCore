using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controllers.Story {
public class StoryCreator : StoryActionFactory {
    // Todo! 解析剧本
    private static IEnumerable<StoryAction> Parser(string text) {
        var lines = text.Split("\n\n");

        var ret = new List<StoryAction>();
        foreach (var line in lines.Select(v => v.Trim()).Where(v => !string.IsNullOrWhiteSpace(v))) {
            if (line == "$") {
                ret.Add(null);
                continue;
            }
            var parts = line.Trim().Split(":");
            ret.Add(parts.Length == 2 ? Say(parts[0], parts[1]) : Say("", line));
        }
        return ret;
    }

    public static StoryAction[] Level1() {
        return Parser(Resources.Load<TextAsset>("Stories/Jin").text).ToArray();
    }
}
}
