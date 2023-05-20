using System.Collections;
using UnityEngine;

namespace Controllers {
public class PlayerController : CombatController {
    public PlayerButton buttons;
    // 用户输入
    public override IEnumerator OnUserInput() {
    #if UNITY_EDITOR

        // GM摸牌
        if (Input.GetKeyDown(KeyCode.G)) {
            combatant.GetCard(1);
        }

    #endif
        // Todo 改成按钮
        // 出牌
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isDiscardStage) {
                combatant.Discard();
            }
            else {
                combatant.PlaySelectedCard();
            }
        }
        // 回合结束
        if (Input.GetKeyDown(KeyCode.Q)) {
            combatant.EndTurn();
        }

        return null;
    }
}
}
