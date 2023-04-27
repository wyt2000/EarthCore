using System.Collections;
using GUIs.Animations;
using Utils;

namespace Controllers {
public class EnemyController : CombatController {
    // Todo! 完善敌人AI
    public override IEnumerator OnUserInput() {
        var enemy = combatant;
        // 随机尝试出5次牌 
        for (var i = 0; i < 5; ++i) {
            var playable = enemy.Cards.FindAll(card =>
            {
                card.IsSelected = true;
                var result = enemy.PreviewBatch.CanEnqueue();
                card.IsSelected = false;
                return result;
            });
            if (playable.Count == 0) break;
            var card = playable[GRandom.Range(0, playable.Count)];
            enemy.PlayCard(card);
            yield return GAnimation.Wait(1.0f);
        }

        enemy.Discard();
    }
}
}
