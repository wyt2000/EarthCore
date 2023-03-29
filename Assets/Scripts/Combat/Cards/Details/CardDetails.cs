using Combat.Effects.Details;
using Combat.Requests;

namespace Combat.Cards.Details {
public static class CardDetails {
    /*
    试探
    {
    元素属性：金
    法力消耗：无
    类型：物理
    伤害值：25
    效果：为自身添加25物理护盾值(2回合)
    }
     */
    public static Card Probing() {
        return new Card {
            Name        = "试探",
            Description = "为自身添加25物理护盾值",
            ImagePath   = "", // Todo 
            ManaCost    = 0,
            Element     = ElementType.Metal,
            OnPlay = req =>
            {
                req.Causer.Attach(EffectDetails.AddPhysicalArmorExtra(25, 2));
                req.Causer.Attack(req.Target, new HealthRequest {
                    Value = 25,
                    DamageParams = {
                        DamageType = DamageType.Physical,
                        Element    = ElementType.Metal,
                    }
                });
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
            Name         = "法力狂涌",
            Description  = "50*（100%+50%当前法力值）",
            ImagePath    = "", // Todo 
            ManaCostFunc = req => req.Causer.State.Mana * 0.5f,
            Element      = ElementType.Water,
            OnPlay = req =>
            {
                req.Causer.Attack(req.Target, new HealthRequest {
                    Value = 50 * (1 + 0.5f * req.Causer.State.Mana / 100),
                    DamageParams = {
                        DamageType = DamageType.Magical,
                        Element    = ElementType.Water,
                    }
                });
            }
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
            Name        = "汲取",
            Description = "回复本次总伤害50%的生命值",
            ImagePath   = "", // Todo 
            ManaCost    = 10,
            Element     = ElementType.Wood,
            OnPlay = req =>
            {
                var realDamage = 0f;
                req.Causer.Attack(req.Target, new HealthRequest {
                    Value = 10,
                    DamageParams = {
                        DamageType = DamageType.Magical,
                        Element    = ElementType.Wood,
                    },
                    OnFinish = res => realDamage = res.Value
                });
                req.Causer.HealSelf(new HealthRequest {
                    ValueFunc = _ => realDamage * 0.5f,
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
            Name        = "清醒",
            Description = "恢复10点法力",
            ImagePath   = "", // Todo 
            ManaCost    = 0,
            Element     = ElementType.Wood,
            OnPlay      = req => { req.Causer.State.Mana += 10; }
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
            Name        = "火力压制",
            Description = "5*10",
            ImagePath   = "", // Todo 
            ManaCost    = 0,
            Element     = ElementType.Fire,
            OnPlay = req =>
            {
                for (var i = 0; i < 10; i++) {
                    req.Causer.Attack(req.Target, new HealthRequest {
                        Value = 5,
                        DamageParams = {
                            DamageType = DamageType.Physical,
                            Element    = ElementType.Fire,
                        }
                    });
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
            Name        = "魔法守护",
            Description = "浸染，获得50点魔法护盾",
            ImagePath   = "", // Todo 
            ManaCost    = 10,
            Element     = ElementType.Earth,
            OnPlay = req =>
            {
                // Todo 添加反甲buff
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
            Name        = "渴望",
            Description = "唯一，不计入出牌次数，抽两张牌",
            ImagePath   = "", // Todo 
            ManaCost    = 10,
            OnPlay = req =>
            {
                // Todo 发起摸牌请求
            }
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
            Name        = "火之召唤",
            Description = "唯一，不计入出牌次数，从牌组随机抽一张火元素卡牌",
            ImagePath   = "", // Todo 
            ManaCost    = 10,
            OnPlay = req =>
            {
                // Todo 发起摸牌请求
            }
        };
    }
}
}
