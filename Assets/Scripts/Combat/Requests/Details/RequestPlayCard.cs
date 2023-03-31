using System;
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

    public override bool PreCheckValid() {
        throw new NotImplementedException();
    }

    public override void ExecuteLogic() {
        // request.Current.PlayCard(request);
        throw new NotImplementedException();
    }

    public override string ToString() {
        return $"出牌";
    }
}
}
