using Combat.Cards;

namespace Combat.Requests {
// 出牌请求 Todo
public class PlayCardRequest {
    // 出牌者
    public CombatantComponent Causer;

    // 出牌目标(自己或对手)
    public CombatantComponent Target;

    // 打出的牌
    public Card[] Cards;

    // 即将打出的牌
    public Card Current;
}
}
