using System.Collections.Generic;
using System.Linq;
using Combat.States;

namespace Combat.Story {
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
}
