using Combat.Requests.Details;
using Utils;

namespace Controllers {
public class EnemyController : CombatController {
    // 随机尝试出5次牌 
    private int m_times = 5;

    // Todo 完善敌人AI
    public override void OnUserInput() {
        if (m_times == 6) {
            combatant.GetCard(6);
            --m_times;
            return;
        }

        if (m_times-- <= 0) {
            combatant.Discard();
            m_times = 6;
            return;
        }

        var card = combatant.Cards[GRandom.Range(0, combatant.Cards.Count)];
        combatant.PlayCard(card);
    }
}
}
