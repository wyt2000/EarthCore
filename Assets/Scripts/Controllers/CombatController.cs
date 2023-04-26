using System.Collections;
using Combat;
using UnityEngine;

namespace Controllers {
public abstract class CombatController : MonoBehaviour {
#region prefab配置

    // 战斗对象
    public CombatantComponent combatant;

#endregion

    // Todo! 测试使用剧本的流程正确性
    public IEnumerator OnDoAction() {
        var current = combatant.Judge.Script?.Current;
        var iter = current?.Execute(this);
        if (current != null) {
            yield return iter;
            combatant.Judge.Script?.Finish();
        }
        else {
            yield return OnUserInput();
        }
    }

    // 返回null表示当前线程直接执行
    protected abstract IEnumerator OnUserInput();
}
}
