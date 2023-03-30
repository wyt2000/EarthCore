using System;
using Combat.Cards;

namespace Combat.Requests {
public class GetCardRequest {
    // 发起人
    public CombatantComponent Causer;

    // 需要几张牌
    public int Count = 1;

    // 满足要求的卡牌
    public Func<Card, bool> Filter = delegate { return true; };

    // 给定候选长度如何选index
    public Func<int, int> SelectIndex = delegate { return 0; };
}
}
