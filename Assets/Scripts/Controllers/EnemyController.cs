using System.Collections;
using GUIs.Animations;
using Utils;

namespace Controllers {
public class EnemyController : CombatController {
    // Todo 完善敌人AI
    public override IEnumerator OnUserInput() {
        var c = combatant;

        // 随机尝试出5次牌 
        for (var i = 0; i < 5 && c.Cards.Count > 0; ++i) {
            var card = c.Cards[GRandom.Range(0, c.Cards.Count)];
            c.PlayCard(card);
            yield return GAnimation.Wait(1.0f);
        }

        c.Discard();
    }
}
}
