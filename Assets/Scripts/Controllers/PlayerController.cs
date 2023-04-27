using System.Collections;
using UnityEngine;

namespace Controllers {
public class PlayerController : CombatController {
    // 用户输入
    public override IEnumerator OnUserInput() {
#if UNITY_EDITOR

        // GM摸牌
        if (Input.GetKeyDown(KeyCode.G)) {
            combatant.GetCard(1);
        }

#endif
        // Todo! 改成按钮
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
