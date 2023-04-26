using System.Collections;
using Combat;
using UnityEngine;

namespace Controllers {
public abstract class CombatController : MonoBehaviour {
#region prefab配置

    // 战斗对象
    public CombatantComponent combatant;

#endregion

    // Todo! 测试使用剧本的流程正确性,修复剧本异常bug
    public IEnumerator OnDoAction() {
        var current = combatant.Judge.Script?.Current;
        var iter = current?.Execute(this);
        if (current != null) {
            combatant.Judge.Script?.Finish();
            yield return iter;
        }
        else {
            yield return OnUserInput();
        }
    }

    // 返回null表示当前线程直接执行
    public abstract IEnumerator OnUserInput();
}
}
