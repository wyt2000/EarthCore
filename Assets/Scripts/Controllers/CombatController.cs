using Combat;
using UnityEngine;

namespace Controllers {
public abstract class CombatController : MonoBehaviour {
#region prefab配置

    // 战斗对象
    public CombatantComponent combatant;

#endregion

    // Todo 优化接口抽象
    public abstract void OnUserInput();
}
}
