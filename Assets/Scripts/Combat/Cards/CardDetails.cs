using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Combat.Effects;
using Combat.Enums;
using Combat.Requests.Details;
using UnityEngine;

// Todo 测试卡牌逻辑
namespace Combat.Cards {
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class CardDetails {
    private static readonly Card[] All;

    static CardDetails() {
        // 反射获取所有card
        var type = typeof(CardDetails);
        var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(
            m => m.ReturnType == typeof(Card) && m.GetParameters().Length == 0
        ).ToArray();
        All = new Card[methods.Length];
        for (var i = 0; i < methods.Length; i++) {
            All[i] = (Card)methods[i].Invoke(null, null);
        }
    }

    public static IEnumerable<Card> CloneAll() {
        return All.Select(v => v.Clone());
    }

#region 卡牌实现细节

#region 金属性

    /*
    | 试探   |              |
    |------|--------------|
    | 元素属性 | 金            |
    | 法力消耗 | 无            |
    | 伤害类型 | 物理           |
    | 伤害值  | 25           |
    | 效果   | 为自身添加25物理护盾 |
     */
    private static Card ShiTan() {
        return new Card {
            Clone         = ShiTan,
            UiName        = "试探",
            UiDescription = "为自身添加25物理护盾并造成25点物理伤害",
            UiImagePath   = "Textures/Card/Details/试探",
            LgDamage      = 25,
            LgDamageType  = DamageType.Physical,
            LgElement     = ElementType.Jin,
            OnExecute = req =>
            {
                req.Causer.State.PhysicalShield += 25;
                req.TakeDamage();
            }
        };
    }

    /*
    | 格挡  |           |
    |------|-----------|
    | 元素属性 | 金         |
    | 法力消耗 | 无         |
    | 伤害类型 | 无         |
    | 伤害值  | 无         |
    | 效果   | 为自身添加15护甲 |
     */
    private static Card GeDang() {
        return new Card {
            Clone         = GeDang,
            UiName        = "格挡",
            UiDescription = "为自身添加15护甲",
            UiImagePath   = "Textures/Card/Details/格挡",
            LgElement     = ElementType.Jin,
            OnExecute     = req => req.Causer.State.PhysicalArmor += 15
        };
    }

    /*
    | 重击   |    |
    |------|----|
    | 元素属性 | 金  |
    | 法力消耗 | 无  |
    | 伤害类型 | 物理 |
    | 伤害值  | 50 |
    | 效果   | 无  |
     */
    private static Card ZhongJi() {
        return new Card {
            Clone         = ZhongJi,
            UiName        = "重击",
            UiDescription = "造成50点物理伤害",
            UiImagePath   = "Textures/Card/Details/重击",
            LgDamage      = 50,
            LgElement     = ElementType.Jin,
            OnExecute     = req => req.TakeDamage()
        };
    }

    /*
    | 盾击   |            |
    |------|------------|
    | 元素属性 | 金          |
    | 法力消耗 | 无          |
    | 伤害类型 | 物理         |
    | 伤害值  | 50%自身物理护盾值 |
    | 效果   | 无          |
     */
    private static Card DunJi() {
        return new Card {
            Clone         = DunJi,
            UiName        = "盾击",
            UiDescription = "造成50%自身物理护盾值的物理伤害",
            UiImagePath   = "Textures/Card/Details/盾击",
            LgDamageFunc  = c => c.Owner.State.PhysicalShield / 2,
            LgElement     = ElementType.Jin,
            OnExecute     = req => req.TakeDamage()
        };
    }

#endregion

#region 木属性

    /*
    | 汲取   |                |
    |------|----------------|
    | 元素属性 | 木              |
    | 法力消耗 | 10             |
    | 伤害类型 | 魔法             |
    | 伤害值  | 10             |
    | 效果   | 回复本次总伤害50%的生命值 |
     */
    private static Card JiQu() {
        return new Card {
            Clone         = JiQu,
            UiName        = "汲取",
            UiDescription = "回复本次总伤害50%的生命值",
            UiImagePath   = "Textures/Card/Details/汲取",
            LgManaCost    = 10,
            LgElement     = ElementType.Mu,
            LgDamage      = 10,
            LgDamageType  = DamageType.Magical,
            OnExecute     = req => req.TakeDamage(),
            OnAfterPlayBatchCard = req =>
            {
                req.Causer.Heal(new RequestHpChange {
                    Value  = req.TotalDamage * 0.5f,
                    Reason = "生命汲取"
                });
            }
        };
    }

    /*
    | 清醒   |         |
    |------|---------|
    | 元素属性 | 木       |
    | 法力消耗 | 无       |
    | 伤害类型 | 无       |
    | 伤害值  | 无       |
    | 效果   | 恢复10点法力 |
     */
    private static Card QingXing() {
        return new Card {
            Clone         = QingXing,
            UiName        = "清醒",
            UiDescription = "恢复10点法力",
            UiImagePath   = "Textures/Card/Details/清醒",
            LgManaCost    = 0,
            LgElement     = ElementType.Mu,
            OnExecute     = req => req.Causer.State.Mana += 10
        };
    }

    /*
    | 包扎   |          |
    |------|----------|
    | 元素属性 | 木        |
    | 法力消耗 | 5        |
    | 伤害类型 | 无        |
    | 伤害值  | 无        |
    | 效果   | 回复50点生命值 |
     */
    private static Card BaoZha() {
        return new Card {
            Clone         = BaoZha,
            UiName        = "包扎",
            UiDescription = "回复50点生命值",
            UiImagePath   = "Textures/Card/Details/包扎",
            LgManaCost    = 5,
            LgElement     = ElementType.Mu,
            OnExecute = req => req.Causer.Heal(new RequestHpChange {
                Value  = 50,
                Reason = "包扎"
            })
        };
    }

    /*
    | 良药   |                                                      |
    |------|------------------------------------------------------|
    | 元素属性 | 木                                                    |
    | 法力消耗 | 10                                                   |
    | 伤害类型 | 无                                                    |
    | 伤害值  | 无                                                    |
    | 效果   | 为自身施加三层【疗养】效果：OnAfterTurnStart:回复50点生命和5点法力，削减一层自身。 |
     */
    private static Card LiangYao() {
        var effect = new Effect {
            UiName        = "疗养",
            UiDescription = "回合开始时,回复50点生命和5点法力.",
            LgTags        = { EffectTag.Buff },
            LgOverlay     = 3,
            LgOpenMerge   = true,

            OnImpAfterTurnStart = self =>
            {
                self.Causer.Heal(new RequestHpChange {
                    Value  = 50,
                    Reason = "良药"
                });
                self.Causer.State.Mana += 5;
                if (--self.LgOverlay <= 0) self.Remove();
            }
        };
        return new Card {
            Clone         = LiangYao,
            UiName        = "良药",
            UiDescription = "为自身施加【疗养】效果,持续3回合.",
            UiImagePath   = "Textures/Card/Details/良药",
            LgManaCost    = 10,
            LgElement     = ElementType.Mu,
            OnExecute     = req => req.Causer.AddBuff(effect)
        };
    }

#endregion

#region 水属性

    /*
    | 法力狂涌 |                                                                           |
    |------|---------------------------------------------------------------------------|
    | 元素属性 | 水                                                                         |
    | 法力消耗 | 50%当前法力值                                                                  |
    | 伤害类型 | 魔法                                                                        |
    | 伤害值  | 50*（100%+50%当前法力值）                                                        |
    | 效果   | 无                                                                         |
    | 备注   | 1.伤害增益只对本张卡牌生效（不会对搭配一起出的卡牌生效）<br/>2.法力消耗可被宁静效果减少，但增益伤害不会被影响，伤害增益只与当前法力值有关 |
     */
    private static Card FaLiKuangYong() {
        return new Card {
            Clone          = FaLiKuangYong,
            UiName         = "法力狂涌",
            UiDescription  = "造成50*(100%+50%当前法力值)伤害",
            UiImagePath    = "Textures/Card/Details/法力狂涌",
            LgManaCostFunc = card => card.Owner.State.Mana * 0.5f,
            LgDamageFunc   = card => 50 * (1 + 0.5f * card.Owner.State.Mana / 100),
            LgDamageType   = DamageType.Magical,
            LgElement      = ElementType.Shui,
            OnExecute      = req => req.TakeDamage()
        };
    }

    /*
    | 积蓄   |                   |
    |------|-------------------|
    | 元素属性 | 水                 |
    | 法力消耗 | 8                 |
    | 伤害类型 | 无                 |
    | 伤害值  | 无                 |
    | 效果   | 获得等于本次出牌总法力消耗的清算值 |
     */
    private static Card JiXv() {
        return new Card {
            Clone         = JiXv,
            UiName        = "积蓄",
            UiDescription = "获得等于本次出牌总法力消耗的清算值",
            UiImagePath   = "Textures/Card/Details/积蓄",

            LgManaCost = 8,
            LgElement  = ElementType.Shui,

            OnAfterPlayBatchCard = req => req.Causer.AddOpBuff(EffectPrefabs.QingSuan((int)req.TotalManaCost))
        };
    }

    /*
    | 施法   |    |
    |------|----|
    | 元素属性 | 水  |
    | 法力消耗 | 5  |
    | 伤害类型 | 魔法 |
    | 伤害值  | 60 |
    | 效果   | 无  |
     */
    private static Card ShiFa() {
        return new Card {
            Clone         = ShiFa,
            UiName        = "施法",
            UiDescription = "造成60点魔法伤害",
            UiImagePath   = "Textures/Card/Details/施法",
            LgManaCost    = 5,
            LgDamage      = 60,
            LgElement     = ElementType.Shui,
            OnExecute     = req => req.TakeDamage()
        };
    }

    /*
    | 暗潮涌动 |            |
    |------|------------|
    | 元素属性 | 水          |
    | 法力消耗 | 10         |
    | 伤害类型 | 魔法         |
    | 伤害值  | 0          |
    | 效果   | 获得70点【清算】值 |
     */
    private static Card AnChaoYongDong() {
        return new Card {
            Clone         = AnChaoYongDong,
            UiName        = "暗潮涌动",
            UiDescription = "获得70点【清算】值",
            UiImagePath   = "Textures/Card/Details/暗潮涌动",
            LgManaCost    = 10,
            LgElement     = ElementType.Shui,
            OnExecute     = req => req.Causer.AddOpBuff(EffectPrefabs.QingSuan(70))
        };
    }

#endregion

#region 火属性

    /*
    | 火力压制 |      |
    |------|------|
    | 元素属性 | 火    |
    | 法力消耗 | 无    |
    | 伤害类型 | 物理   |
    | 伤害值  | 5*10 |
    | 效果   | 无    |
     */
    private static Card HuoLiYaZhi() {
        return new Card {
            Clone         = HuoLiYaZhi,
            UiName        = "火力压制",
            UiDescription = "造成5*10点物理伤害",
            UiImagePath   = "Textures/Card/Details/火力压制",
            LgDamage      = 10,
            LgElement     = ElementType.Huo,
            OnExecute = req =>
            {
                for (var i = 0; i < 5; ++i) req.TakeDamage();
            }
        };
    }

    /*
    | 熔铁之刃 |                                    |
    |------|------------------------------------|
    | 元素属性 | 火                                  |
    | 法力消耗 | 无                                  |
    | 伤害类型 | 物理                                 |
    | 伤害值  | 25                                 |
    | 效果   | 减少敌方10点护甲值，若敌方护甲值不足10点，造成溢出部分两倍的伤害 |
     */
    private static Card YuShiJuFen() {
        return new Card {
            Clone         = YuShiJuFen,
            UiName        = "熔铁之刃",
            UiDescription = "造成25点物理伤害，减少敌方10点护甲值，若敌方护甲值不足10点，造成溢出部分两倍的伤害",
            UiImagePath   = "Textures/Card/Details/熔铁之刃",
            LgDamage      = 25,
            LgElement     = ElementType.Huo,
            OnExecute = req =>
            {
                var state = req.Batch.Target.State;
                var armor = state.PhysicalArmor;
                if (armor >= 10) {
                    state.PhysicalArmor -= 10;
                }
                else {
                    req.Current.LgDamage += 2 * (10 - armor);
                    state.PhysicalArmor  =  0;
                }
                req.TakeDamage();
            }
        };
    }

    /*
    | 穿刺   |    |
    |------|----|
    | 元素属性 | 火  |
    | 法力消耗 | 无  |
    | 伤害类型 | 物理 |
    | 伤害值  | 35 |
    | 效果   | 穿透 |
     */
    private static Card ChuanCi() {
        return new Card {
            Clone         = ChuanCi,
            UiName        = "穿刺",
            UiDescription = "造成35点物理伤害，穿透",
            UiImagePath   = "Textures/Card/Details/穿刺",
            LgDamage      = 35,
            LgElement     = ElementType.Huo,
            OnExecute     = req => req.TakeDamage(true)
        };
    }

    /*
    | 焚烧   |               |
    |------|---------------|
    | 元素属性 | 火             |
    | 法力消耗 | 无             |
    | 伤害类型 | 物理            |
    | 伤害值  | 无             |
    | 效果   | 为敌人施加3层【淬炼】效果 |
     */
    private static Card FenShao() {
        return new Card {
            Clone         = FenShao,
            UiName        = "焚烧",
            UiDescription = "为敌人施加3层【淬炼】效果",
            UiImagePath   = "Textures/Card/Details/焚烧",
            LgElement     = ElementType.Huo,
            OnExecute     = req => req.Causer.AddOpBuff(EffectPrefabs.CuiLian(3))
        };
    }

#endregion

#region 土属性

    /*
    | 魔法守护 |              |
    |------|--------------|
    | 元素属性 | 土            |
    | 法力消耗 | 10           |
    | 伤害类型 | 无            |
    | 伤害值  | 无            |
    | 效果   | 浸染，获得50点魔法护盾 |
     */
    private static Card MoFaShouHu() {
        return new Card {
            Clone         = MoFaShouHu,
            UiName        = "魔法守护",
            UiDescription = "浸染，获得50点魔法护盾",
            UiImagePath   = "Textures/Card/Details/魔法守护",
            LgManaCost    = 10,
            LgElement     = ElementType.Tu,
            LgInfect      = true,
            OnExecute     = req => req.Causer.State.MagicShield += 50
        };
    }

    /*
    | 污泥之躯 |                        |
    |------|------------------------|
    | 元素属性 | 土                      |
    | 法力消耗 | 5                      |
    | 伤害类型 | 无                      |
    | 伤害值  | 无                      |
    | 效果   | 消耗15%当前生命值，获得两倍于其的魔法护盾 |
     */
    private static Card WuNiZhiQu() {
        return new Card {
            Clone         = WuNiZhiQu,
            UiName        = "污泥之躯",
            UiDescription = "消耗15%当前生命值，获得两倍于其的魔法护盾",
            UiImagePath   = "Textures/Card/Details/污泥之躯",
            LgManaCost    = 5,
            LgElement     = ElementType.Tu,
            OnExecute = req =>
            {
                var state = req.Causer.State;
                var cost = state.Health * 0.15f;
                state.Health      -= cost;
                state.MagicShield += cost * 2;
            }
        };
    }

    /*
    | 厄运   |                                             |
    |------|---------------------------------------------|
    | 元素属性 | 土                                           |
    | 法力消耗 | 7                                           |
    | 伤害类型 | 魔法                                          |
    | 伤害值  | 100                                         |
    | 效果   | 为敌方施加厄运buff，三回合后生效造成100点土属性魔法伤害（生效于敌方回合开始时） |
     */
    private static Card EYun() {
        var effect = new Effect {
            UiName            = "厄运",
            UiDescription     = "三回合后生效造成100点土属性魔法伤害（生效于敌方回合开始时）",
            UiIconPath        = "",
            LgRemainingRounds = 3,
            OnImpAfterTurnStart = self =>
            {
                if (self.LgRemainingRounds > 1) return;
                self.Causer.Attack(new RequestHpChange {
                    Value   = 100,
                    Element = ElementType.Tu,
                    Type    = DamageType.Magical,
                });
                self.Remove();
            }
        };
        return new Card {
            Clone         = EYun,
            UiName        = "厄运",
            UiDescription = "为敌方施加厄运buff，三回合后生效造成100点土属性魔法伤害（生效于敌方回合开始时）",
            UiImagePath   = "Textures/Card/Details/厄运",
            LgManaCost    = 7,
            LgDamage      = 100,
            LgElement     = ElementType.Tu,
            OnExecute     = req => req.Causer.AddOpBuff(effect)
        };
    }

#endregion

#region 无属性

    /*
    | 渴望   |         |
    |------|---------|
    | 元素属性 | 无       |
    | 法力消耗 | 10      |
    | 伤害类型 | 无       |
    | 伤害值  | 无       |
    | 效果   | 唯一，抽两张牌 |
     */
    private static Card KeWang() {
        return new Card {
            Clone         = KeWang,
            UiName        = "渴望",
            UiDescription = "唯一，不计入出牌次数，抽两张牌",
            UiImagePath   = "Textures/Card/Details/渴望",
            LgManaCost    = 10,
            LgUnique      = true,
            OnExecute     = req => req.Causer.GetCard(2)
        };
    }

    /*
    | 火之召唤 |                 |
    |------|-----------------|
    | 元素属性 | 无               |
    | 法力消耗 | 10              |
    | 伤害类型 | 无               |
    | 伤害值  | 无               |
    | 效果   | 唯一，从牌组中抽一张火属性卡牌 |
    */
    private static Card HuoZhiZhaoHuang() {
        return new Card {
            Clone         = HuoZhiZhaoHuang,
            UiName        = "火之召唤",
            UiDescription = "唯一，不计入出牌次数，从牌组随机抽一张火元素卡牌",
            UiImagePath   = "Textures/Card/Details/火之召唤",
            LgManaCost    = 10,
            LgUnique      = true,
            OnExecute = req =>
            {
                // 发起摸牌请求
                req.Causer.GetCard(new RequestGetCard {
                    Count       = 1,
                    Filter      = c => c.LgElement == ElementType.Huo,
                    SelectIndex = n => Random.Range(0, n)
                });
            }
        };
    }

    /*
    | 铁甲加固 |                                                                                          |
    |------|------------------------------------------------------------------------------------------|
    | 元素属性 | 无                                                                                        |
    | 法力消耗 | 20                                                                                       |
    | 伤害类型 | 无                                                                                        |
    | 伤害值  | 无                                                                                        |
    | 效果   | 唯一，本场战斗中，每增加一次护盾，对敌人造成30%本次增加护盾值的金属性物理伤害<br/>OnAfterSelfHpChange:对敌人造成本次获得30%护盾值的金属性物理伤害 |
     */
    private static Card TieJiaJiaGu() {
        // Todo 铁甲加固效果
        var effect = new Effect {
            UiName        = "铁甲加固",
            UiDescription = "唯一，本场战斗中，每增加一次护盾，对敌人造成30%本次增加护盾值的金属性物理伤害",
            UiIconPath    = "",
            // Todo 监听状态修改事件
            // OnImpStateChange = (req, delta) =>
            // {
            //     if (delta > 0) {
            //         var damage = delta * 0.3f;
            //        // req.Batch.Target.TakeDamage(damage, ElementType.Jin, DamageType.Physical);
            //     }
            // }
        };
        return new Card {
            Clone         = TieJiaJiaGu,
            UiName        = "铁甲加固",
            UiDescription = "唯一，本场战斗中，每增加一次护盾，对敌人造成30%本次增加护盾值的金属性物理伤害",
            UiImagePath   = "Textures/Card/Details/铁甲加固",
            LgManaCost    = 20,
            OnExecute     = req => req.Causer.AddBuff(effect)
        };
    }

#endregion

#endregion
}
}
