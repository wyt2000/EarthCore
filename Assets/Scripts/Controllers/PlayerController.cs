using System.Collections;
using UnityEngine;

namespace Controllers {
public class PlayerController : CombatController {
    // 用户输入 Todo! 实现超时后自动弃牌
    public override IEnumerator OnUserInput() {
        // 摸牌
        if (Input.GetKeyDown(KeyCode.G)) {
            combatant.GetCard(1);
        }
        // 出牌
        if (Input.GetKeyDown(KeyCode.Space)) {
            combatant.PlaySelectedCard();
        }
        // 弃牌
        if (Input.GetKeyDown(KeyCode.Return)) {
            combatant.Discard();
        }

        return null;
    }
}
}
