using Combat.Cards;

namespace Combat.Requests {
// 出牌请求
public class PlayCardRequest {
    // 出牌者
    public CombatantComponent Causer;

    // 出牌目标(自己或对手)
    public CombatantComponent Target;

    // 打出的牌
    public Card[] Cards;

    // 本次出牌的消耗法力
    public float ManaCost;

    // 即将打出的牌
    public Card Current;
}
}
