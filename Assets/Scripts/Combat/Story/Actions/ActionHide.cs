using System.Collections;
using System.Collections.Generic;
using Controllers;

namespace Combat.Story.Actions {
/*
@hide // 隐藏整个对话框
@hide left // 隐藏左侧贴图
@hide right // 隐藏右侧贴图
 */
public class ActionHide : StoryAction {
    private string m_side;

    public override StoryAction Build(IReadOnlyList<string> args) {
        if (args.Count > 0) {
            m_side = args[0];
        }
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        combatant.Judge.dialog.Hide(m_side);
        return null;
    }

    protected override string ToDescription() {
        return "隐藏对话框";
    }
}
}
