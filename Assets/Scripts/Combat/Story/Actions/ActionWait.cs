using System.Collections;
using System.Collections.Generic;
using Controllers;

namespace Combat.Story.Actions {
/*
@wait // 等待战斗结束
 */
public class ActionWait : StoryAction {
    public override StoryAction Build(IReadOnlyList<string> args) {
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var a = controller.combatant;
        var b = a.Opponent;
        while (!(a.State.IsDead || b.State.IsDead)) {
            yield return controller.OnUserInput();
        }
        yield return a.Judge.CombatEnd();
    }

    protected override string ToDescription() {
        return "等待战斗结束";
    }
}
}
