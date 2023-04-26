using System.Collections;
using System.Collections.Generic;
using Controllers;

namespace Combat.Story.Actions {
/*
// 某角色说一句话,content是长句,line是短句,content不能包含""",line不能有换行,空格,和:号
@say {name} """
{content}
"""

@say {name}:{line}

// 旁白说一句话
@say """
{content}
"""

@say {line}
 */
public class ActionSay : StoryAction {
    private string m_character; // 说话的角色
    private string m_content;   // 说的内容

    private StoryAction Build(string name, string content) {
        m_character = name;
        m_content   = content;
        return this;
    }

    public override StoryAction Build(IReadOnlyList<string> args) {
        if (args.Count == 1) args = args[0].Split(':');
        return args.Count == 2 ? Build(args[0], args[1]) : Build("旁白", args[0]);
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        var dialog = combatant.Judge.dialog;
        return dialog.Say(m_character, m_content);
    }

    protected override string ToDescription() {
        return $"{m_character}说:{m_content}";
    }
}
}
