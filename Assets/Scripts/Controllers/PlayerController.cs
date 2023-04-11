using System.Linq;
using Combat;
using Combat.Requests.Details;
using UnityEngine;

namespace Controllers {
public class PlayerController : MonoBehaviour {
#region prefab配置

    // 战斗对象
    public CombatantComponent combatant;

#endregion

    // 用户输入
    public void OnUserInput() {
        // 摸牌
        if (Input.GetKeyDown(KeyCode.S)) {
            combatant.GetCard(1);
        }
        // 出牌
        if (Input.GetKeyDown(KeyCode.Space)) {
            var card = combatant.Cards.Where(c => c.IsSelected);
            combatant.PlayCard(combatant.Opponent, new RequestPlayBatchCard {
                Cards = card.ToArray()
            });
        }
        // 弃牌
        if (Input.GetKeyDown(KeyCode.Return)) {
            // Todo 
        }
    }
}
}
