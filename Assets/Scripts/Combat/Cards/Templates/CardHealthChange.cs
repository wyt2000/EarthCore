using Combat.Requests;

namespace Combat.Cards.Templates {
// 伤害/治疗型卡牌
public class CardHealthChange : Card {
    public HealthRequest Request = new();

    public CardHealthChange() {
        OnPlay = req => { req.Causer.Attack(req.Target, Request); };
    }
}
}
