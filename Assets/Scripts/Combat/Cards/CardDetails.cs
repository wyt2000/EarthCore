using Combat.Enums;
using Combat.Requests.Details;
using UnityEngine;

namespace Combat.Cards {
public static class CardDetails {
    /*
    试探
    {
    元素属性：金
    法力消耗：无
    类型：物理
    伤害值：25
    效果：为自身添加25物理护盾值
    }
     */
    public static Card Probing() {
        return new Card {
            Clone         = Probing,
            UiName        = "试探",
            UiDescription = "为自身添加25物理护盾值",
            UiImagePath   = "Textures/Cards/试探",
            LgManaCost    = 0,
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
    法力狂涌
    {
    元素属性：水
    法力消耗：50%当前法力值
    类型：魔法
    伤害值：50*（100%+50%当前法力值）
    //伤害增益只对本张卡牌生效（不会对搭配一起出的卡牌生效）
    //法力消耗可被宁静效果减少，但增益伤害不会被影响，伤害增益只与当前法力值有关
    }
     */
    public static Card ManaSurge() {
        return new Card {
            Clone          = ManaSurge,
            UiName         = "咒能狂涌",
            UiDescription  = "50*（100%+50%当前法力值）",
            UiImagePath    = "Textures/Cards/咒能狂涌",
            LgManaCostFunc = card => card.Owner.State.Mana * 0.5f,
            LgDamageFunc   = card => 50 * (1 + 0.5f * card.Owner.State.Mana / 100),
            LgDamageType   = DamageType.Magical,
            LgElement      = ElementType.Shui,
            OnExecute      = req => req.TakeDamage()
        };
    }

    /*
    汲取
    {
    元素属性：木
    法力消耗：10
    类型：魔法
    伤害值：10
    效果：回复本次总伤害50%的生命值
    }
     */
    public static Card Drain() {
        return new Card {
            Clone         = Drain,
            UiName        = "生命汲取",
            UiDescription = "回复本次总伤害50%的生命值",
            UiImagePath   = "Textures/Cards/生命汲取",
            LgManaCost    = 10,
            LgElement     = ElementType.Mu,
            LgDamage      = 10,
            LgDamageType  = DamageType.Magical,
            OnExecute     = req => req.TakeDamage(),
            OnAfterPlayBatchCard = req =>
            {
                req.Causer.HealSelf(new RequestHpChange {
                    Value = req.TotalDamage * 0.5f,
                });
            }
        };
    }

    /*
    清醒
    {
    元素属性：木
    法力消耗：无
    类型：无
    伤害值：无
    效果：恢复10点法力
    }
     */
    public static Card Awakening() {
        return new Card {
            Clone         = Awakening,
            UiName        = "清醒",
            UiDescription = "恢复10点法力",
            UiImagePath   = "Textures/Cards/清醒",
            LgManaCost    = 0,
            LgElement     = ElementType.Mu,
            OnExecute     = req => req.Causer.State.Mana += 10
        };
    }

    /*
    火力压制
    {
    元素属性：火
    法力消耗：无
    类型：物理
    伤害值：5*10
    效果：无
    }
     */
    public static Card FireSuppression() {
        return new Card {
            Clone         = FireSuppression,
            UiName        = "火焰连击",
            UiDescription = "造成五次伤害,每次10点",
            UiImagePath   = "Textures/Cards/火焰连击",
            LgManaCost    = 0,
            LgElement     = ElementType.Huo,
            LgDamage      = 10,
            OnExecute = req =>
            {
                for (var i = 0; i < 5; i++) {
                    req.TakeDamage();
                }
            }
        };
    }

    /*
    魔法守护
    {
    元素属性：土
    法力消耗：10
    类型：无
    伤害值：无
    效果：浸染，获得50点魔法护盾//（魔法护盾都拥有反伤效果，没减少一点魔法护盾值，对敌方造成1点土属性伤害）
    }
     */
    public static Card MagicGuard() {
        return new Card {
            Clone         = MagicGuard,
            UiName        = "魔法守护",
            UiDescription = "浸染，获得50点魔法护盾",
            UiImagePath   = "Textures/Cards/魔法守护",
            LgManaCost    = 10,
            LgElement     = ElementType.Tu,
            LgInfect      = true,
            OnExecute = req =>
            {
                req.Causer.State.MagicShield += 50;
                // Todo 实现反甲效果
                // req.Causer.Attach(EffectDetails.MagicAntiArmor(50));
            }
        };
    }

    /*
    渴望
    {
    元素属性：无
    法力消耗：10
    类型：无
    伤害值：无
    效果：唯一，不计入出牌次数，抽两张牌
    }
     */
    public static Card Thirst() {
        return new Card {
            Clone         = Thirst,
            UiName        = "渴望",
            UiDescription = "唯一，不计入出牌次数，抽两张牌",
            UiImagePath   = "Textures/Cards/渴望",
            LgManaCost    = 10,
            LgUnique      = true,
            OnExecute     = req => req.Causer.GetCard(2)
        };
    }

    /*
    火之召唤
    {
    元素属性：无
    法力消耗：10
    类型：无
    伤害值：无
    效果：唯一，不计入出牌次数，从牌组随机抽一张火元素卡牌
    }
    */
    public static Card FireSummon() {
        return new Card {
            Clone         = FireSummon,
            UiName        = "火之召唤",
            UiDescription = "唯一，不计入出牌次数，从牌组随机抽一张火元素卡牌",
            UiImagePath   = "Textures/Cards/火之召唤",
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
}
}
