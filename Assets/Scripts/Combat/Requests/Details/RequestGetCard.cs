using System;
using System.Collections;
using System.Linq;
using Combat.Cards;

namespace Combat.Requests.Details {
// 摸牌请求
public class RequestGetCard : CombatRequest {
#region 配置项

    // 需要几张牌
    public int Count = 1;

    // 满足要求的卡牌
    public Func<Card, bool> Filter = delegate { return true; };

    // 给定候选长度如何选index
    public Func<int, int> SelectIndex = delegate { return 0; };

#endregion

    public override bool CanEnqueue() {
        return Require(
            Causer != null && Count > 0,
            "无效的摸牌请求"
        );
    }

    public override IEnumerator Execute() {
        var cards = Causer.Heap.GetCards(this);
        Causer.Cards.AddRange(cards);
        return cards.Select(card => Causer.cardSlot.AddCard(card)).GetEnumerator();
    }

    public override string Description() {
        return $"{Causer.name}摸{Count}张牌";
    }
}
}
