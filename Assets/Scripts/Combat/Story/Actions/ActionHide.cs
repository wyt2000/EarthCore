using System.Collections;
using System.Collections.Generic;
using Controllers;

namespace Combat.Story.Actions {
// Todo! 完善hide left/right指令 
/*
@hide // 隐藏整个对话框
@hide left // 隐藏左侧贴图
@hide right // 隐藏右侧贴图
 */
public class ActionHide : StoryAction {
    public override StoryAction Build(IReadOnlyList<string> args) {
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        combatant.Judge.dialog.Hide();
        return null;
    }

    protected override string ToDescription() {
        return "隐藏对话框";
    }
}
}
