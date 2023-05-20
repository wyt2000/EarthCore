using System.Collections;
using Combat;
using UnityEngine;

namespace Controllers {
public abstract class CombatController : MonoBehaviour {
#region prefab配置

    // 战斗对象
    public CombatantComponent combatant;
    
    // 是否为弃牌阶段
    public bool isDiscardStage = false;

#endregion

    private void Finish() {
        combatant.Judge.Script?.Finish();
    }

    public IEnumerator OnDoAction() {
        var current = combatant.Judge.Script?.Current;
        var iter = current?.Execute(this);
        if (current != null) {
            yield return iter;
            Finish();
        } else {
            yield return OnUserInput();
        }
    }

    // 返回null表示当前线程直接执行
    public abstract IEnumerator OnUserInput();
}
}
