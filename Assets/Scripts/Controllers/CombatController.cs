using System.Collections;
using Combat;
using UnityEngine;

namespace Controllers {
public abstract class CombatController : MonoBehaviour {
#region prefab配置

    // 战斗对象
    public CombatantComponent combatant;

#endregion

    // 返回null表示当前线程直接执行
    public abstract IEnumerator OnUserInput();
}
}
