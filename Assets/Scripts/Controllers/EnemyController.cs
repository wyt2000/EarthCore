using System.Collections;
using GUIs.Animations;
using Utils;

namespace Controllers {
public class EnemyController : CombatController {
    // Todo! 完善敌人AI
    public override IEnumerator OnUserInput() {
        var enemy = combatant;
        // 随机尝试出5次牌 
        for (var i = 0; i < 5 && enemy.Cards.Count > 0; ++i) {
            var card = enemy.Cards[GRandom.Range(0, enemy.Cards.Count)];
            enemy.PlayCard(card);
            yield return GAnimation.Wait(1.0f);
        }

        enemy.Discard();
    }
}
}
