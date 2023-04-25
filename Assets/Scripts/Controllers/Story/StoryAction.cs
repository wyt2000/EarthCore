using System.Collections.Generic;
using System.Linq;
using Combat.States;

namespace Controllers.Story {
public class StoryAction {
    public StoryActionType Type;

#region start

    public CombatState Player; // 玩家初始状态
    public CombatState Enemy;  // 怪物初始状态

#endregion

#region say

    public string Character; // 说话的角色
    public string Content;   // 说的内容

#endregion

#region show

    public string ImageName; // 立绘的名字

#endregion

#region give/wait

    public bool     IsPlayer;  // 是否是玩家
    public string[] CardNames; // 牌的名字

#endregion
}

public class StoryActionFactory {
    protected static StoryAction Init(CombatState player, CombatState enemy) {
        return new StoryAction {
            Type   = StoryActionType.Init,
            Player = player,
            Enemy  = enemy,
        };
    }

    protected static StoryAction Start() {
        return new StoryAction {
            Type = StoryActionType.Start,
        };
    }

    protected static StoryAction Say(string character, string content) {
        return new StoryAction {
            Type      = StoryActionType.Say,
            Character = character.Trim(),
            Content   = content.Trim(),
        };
    }

    protected static StoryAction Show(string imageName) {
        return new StoryAction {
            Type      = StoryActionType.Show,
            ImageName = imageName.Trim(),
        };
    }

    protected static StoryAction Hide() {
        return new StoryAction {
            Type = StoryActionType.Hide,
        };
    }

    protected static StoryAction Give(bool isPlayer, IEnumerable<string> cardNames) {
        return new StoryAction {
            Type      = StoryActionType.Give,
            IsPlayer  = isPlayer,
            CardNames = cardNames.ToArray(),
        };
    }

    protected static StoryAction Wait(bool isPlayer, IEnumerable<string> cardNames) {
        return new StoryAction {
            Type      = StoryActionType.Wait,
            IsPlayer  = isPlayer,
            CardNames = cardNames.ToArray(),
        };
    }

    protected static StoryAction WaitEnd() {
        return new StoryAction {
            Type = StoryActionType.WaitEnd,
        };
    }
}
}
