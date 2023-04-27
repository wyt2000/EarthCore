using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Combat.Story.Actions {
/*
help line
help """
content
"""
 */
public class ActionHelp : StoryAction {
    private string m_msg;

    public override StoryAction Build(IReadOnlyList<string> args) {
        m_msg = args[0];
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var help = controller.combatant.Judge.help;
        yield return help.Show(m_msg);
    }

    protected override string ToDescription() {
        return "教程";
    }
}
}
