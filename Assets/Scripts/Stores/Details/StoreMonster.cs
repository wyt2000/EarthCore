using Combat.Cards;
using Combat.Enums;
using Combat.States;

namespace Stores.Details {
// 怪物状态 
public class StoreMonster : StoreCombatant {
    // 怪物名
    public string Name = "Monster";

    protected override CombatState OnInitState() {
        var ret = new CombatState {
            HealthMaxBase = 2000,

            ManaMaxBase = 100,

            InitCardCnt = 3,
            GetCardCnt  = 3,
            MaxCardCnt  = int.MaxValue,

            ElementMaxAttach = {
                { ElementType.Jin, 2 },
                { ElementType.Mu, 2 },
                { ElementType.Shui, 2 },
                { ElementType.Huo, 2 },
                { ElementType.Tu, 2 },
            },
        };
        
        return ret;
    }
}
}
