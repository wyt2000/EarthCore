using System.Collections;
using System.Collections.Generic;
using Controllers;

namespace Combat.Story.Actions {
/*
@start // 战斗开始
 */
public class ActionStart : StoryAction {
    public override StoryAction Build(IReadOnlyList<string> args) {
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        return combatant.Judge.CombatStart();
    }

    protected override string ToDescription() {
        return "战斗开始";
    }
}
}
