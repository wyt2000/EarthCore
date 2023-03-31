using System;
using Combat.Cards;

namespace Combat.Requests.Details {
// 批量出牌请求
public class RequestPlayBatchCard : CombatRequest {
#region 配置项

    // 出牌目标(自己或对手)
    public CombatantComponent Target;

    // 本次请求所有打出的牌
    public Card[] Cards;

    // 本次出牌消耗的总法力
    public float ManaCost;

#endregion

    public override bool PreCheckValid() {
        throw new NotImplementedException();
    }

    public override void ExecuteLogic() {
        throw new NotImplementedException();
    }

    public override string ToString() {
        return $"批量出牌x{Cards.Length}";
    }
}
}
