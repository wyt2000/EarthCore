using System.Linq;
using UnityEngine;

namespace Controllers {
public class PlayerController : CombatController {
    // 用户输入
    public override void OnUserInput() {
        // 摸牌
        if (Input.GetKeyDown(KeyCode.G)) {
            combatant.GetCard(1);
        }
        // 出牌
        if (Input.GetKeyDown(KeyCode.Space)) {
            var cards = combatant.Cards.Where(c => c.IsSelected);
            combatant.PlayCard(cards);
        }
        // 弃牌
        if (Input.GetKeyDown(KeyCode.Return)) {
            combatant.Discard();
        }
    }
}
}
