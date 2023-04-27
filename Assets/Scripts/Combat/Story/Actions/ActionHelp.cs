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
        // Todo! 改成helpView
        var help = controller.combatant.Judge.dialog;
        yield return help.Say("教程", m_msg);
        while (!Input.GetMouseButtonDown(0)) yield return null;
        help.Hide();
    }

    protected override string ToDescription() {
        return "教程";
    }
}
}
