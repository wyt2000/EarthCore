using System.Collections;
using Combat.Cards;

namespace Combat.Requests.Details {
// 批量出牌请求指派的单个出牌请求
public class RequestPlayCard : CombatRequest {
#region 配置项

    // 所在的批量出牌请求
    public RequestPlayBatchCard Batch;

    // 即将生效的牌
    public Card Current;

#endregion

#region 中间项

    public float Scale => Batch.Scale;

#endregion

    public override bool CanEnqueue() {
        return Require(
            Batch != null && Current != null,
            "批量出牌请求指派的单个出牌请求的Batch或Current为空"
        );
    }

    // Todo 加动画
    public override IEnumerator Execute() {
        Current.OnExecute(this);
        // 元素浸染,施加一次元素打击
        if (Current.LgInfect) TakeDamage(true);
        yield break;
    }

    public override string Description() {
        return $"出牌:{Current.UiName}";
    }

#region 公开函数

    public void TakeDamage(bool real = false) {
        Current.TakeDamage(this, real);
    }

#endregion
}
}
