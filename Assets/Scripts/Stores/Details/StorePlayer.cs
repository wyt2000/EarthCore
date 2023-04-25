﻿using Combat.Enums;
using Combat.States;

namespace Stores.Details {
public class StorePlayer : StoreCombatant {
    // 玩家名
    public string Name = "Player";

    // 玩家经验值
    public int Exp = 0;

    // 玩家关卡进度
    public int Process = 0;

    // 玩家等级
    public int Level => Exp / 100;

    // 玩家初始战斗状态
    protected override CombatState OnInitState() {
        var ret = new CombatState {
            HealthMaxBase = 1000,

            ManaMaxBase = 50,

            InitCardCnt = 3,
            GetCardCnt  = 2,
            MaxCardCnt  = 5,

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
