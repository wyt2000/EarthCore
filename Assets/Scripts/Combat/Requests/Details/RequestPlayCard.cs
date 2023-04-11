using System.Collections;
using Combat.Cards;
using UnityEngine;

namespace Combat.Requests.Details {
// 批量出牌请求指派的单个出牌请求
public class RequestPlayCard : CombatRequest {
#region 配置项

    // 所在的批量出牌请求
    public RequestPlayBatchCard Batch;

    // 即将生效的牌
    public Card Current;

#endregion

    public override bool CanEnqueue() {
        if (Batch != null && Current != null) return true;
        Debug.LogError($"批量出牌请求指派的单个出牌请求的Batch或Current为空");
        return false;
    }

    // Todo 加动画
    public override IEnumerator Execute() {
        Current.OnExecute(this);
        yield break;
    }

    public override string Description() {
        return $"出牌:{Current.UiName}";
    }

#region 公开函数

    public void TakeDamage() {
        Current.TakeDamage(this);
    }

#endregion
}
}
