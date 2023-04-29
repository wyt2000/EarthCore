using System.Collections;
using System.Collections.Generic;
using Controllers;

namespace Combat.Story.Actions {
/*
@next // 结束回合
 */
public class ActionNext : StoryAction {
    public override StoryAction Build(IReadOnlyList<string> args) {
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        combatant.Discard();
        return null;
    }

    protected override string ToDescription() {
        return "结束回合";
    }
}
}
