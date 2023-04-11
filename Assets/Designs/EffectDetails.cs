using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Effects;
using Combat.Effects.Templates;
using Combat.Enums;
using Combat.Requests.Details;
using UnityEngine;
using Utils;

namespace Designs {
// Todo 优化文档
public static class EffectDetails {
#region 元素联动

    /* Todo 确认效果
金+土：免疫：免疫下一次伤害（持续到下次伤害到来）
火+土：固定：令自身魔法护盾值增加20%（永久）
水+金：滋润：令自身物理护盾值增加20%（永久）
火+木：引燃：令敌方获得5层燃烧，每次攻击消耗一层燃烧对敌人造成敌人2%当前生命值额外物理伤害（额外伤害小于1时改为1，属性与触发燃烧效果的攻击相同）（永久）
水+木：宁静：本次出牌法力消耗减半
火+木+土：自燃：下次受到的伤害时敌人受到等量伤害（持续到下次伤害到来）
金+土+水：洞察：无效敌方下一次的伤害和控制效果（分两部分，分别持续到下次伤害到来和下次控制到来）
金+水+木：不竭：本次出牌法力消耗减半，抽一张牌
火+土+金：淬炼：为敌方施加5层淬炼效果，每回合开始时消耗一层淬炼对敌方造成（1%*淬炼层数）最大生命值物理伤害。（永久）
水+火+木：击碎：令敌方物理和魔法护盾值各减少20%
金+木+水+火+土：净化：直接击碎全部敌方法印，斩杀生命值低于10%的敌人。
     */

    private class EffectMetalEarth : Effect {
        private readonly EffectTrigger m_trigger;

        public EffectMetalEarth() {
            UiName        = "金土联动";
            UiDescription = "免疫：免疫下一次伤害";
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
            m_trigger = new EffectTrigger(this);
        }

        // 免疫伤害效果 Todo 可以加入到战斗属性中
        protected override bool OnBeforeSelfHpChange(RequestHpChange request) {
            var reject = base.OnBeforeSelfHpChange(request);
            if (m_trigger.InValid) return reject;
            return reject && m_trigger.Trigger(!request.IsHeal);
        }
    }

    // 金+土：免疫：免疫下一次伤害
    public static Effect Metal_Earth() {
        return new EffectMetalEarth();
    }


    // 火+土：固定：令自身魔法护盾值增加20%  
    public static Effect Fire_Earth() {
        return new EffectOnce {
            UiName        = "火土联动",
            UiDescription = "固定：令自身魔法护盾值增加20%",
            LgAction      = e => e.Target.State.MagicShield *= 1.2f,

            LgRemainingRounds = 2,
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            }
        };
    }

    // 水+金：滋润：令自身物理护盾值增加20%
    public static Effect Water_Metal() {
        return new EffectOnce {
            UiName        = "水金联动",
            UiDescription = "滋润：令自身物理护盾值增加20%",
            LgAction      = e => e.Target.State.PhysicalShield *= 1.2f,

            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            }
        };
    }

    private class EffectFireWood : Effect {
        private readonly EffectTrigger m_trigger;

        public EffectFireWood() {
            UiName        = "火木联动";
            UiDescription = "引燃：令敌方获得5层燃烧，每次攻击消耗一层燃烧对敌人造成敌人2%当前生命值额外物理伤害（额外伤害小于1时改为1）";
            LgTags = new HashSet<EffectTag> {
                EffectTag.DeBuff
            };
            m_trigger = new EffectTrigger(this) {
                MaxTriggerTimes = 5
            };
        }

        protected override void OnAfterTakeHpChange(RequestHpChange request) {
            base.OnAfterTakeHpChange(request);
            if (request.IsHeal || m_trigger.InValid) return;
            Target.Attack(request.Target, new RequestHpChange {
                Value = Mathf.Max(1, 0.02f * request.Target.State.Health),
                Type  = DamageType.Physical,
            });
            m_trigger.Trigger();
        }
    }

    // 火+木：引燃：令敌方获得5层燃烧，每次攻击消耗一层燃烧对敌人造成敌人2%当前生命值额外物理伤害（额外伤害小于1时改为1）
    public static Effect Fire_Wood() {
        return new EffectFireWood();
    }

    private class EffectWaterWood : Effect {
        public EffectWaterWood() {
            UiName        = "宁静";
            UiDescription = "本次出牌法力消耗减半";
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
        }

        protected override void OnAfterPreviewMana(RequestPlayBatchCard request) {
            base.OnAfterPreviewMana(request);

            request.TotalManaCost /= 2;
        }

        protected override void OnAfterPlayBatchCard(RequestPlayBatchCard request) {
            base.OnAfterPlayBatchCard(request);

            Remove();
        }
    }

    // 水+木：宁静：下次出牌法力消耗减半
    public static Effect Water_Wood() {
        return new EffectWaterWood();
    }

    private class EffectFireWoodEarth : Effect {
        public EffectFireWoodEarth() {
            UiName        = "火木土联动";
            UiDescription = "自燃：下次受到的伤害时敌人受到等量火元素伤害";
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
        }

        protected override void OnAfterSelfHpChange(RequestHpChange request) {
            base.OnAfterSelfHpChange(request);

            // 忽略治疗请求
            if (request.IsHeal) return;

            // 修改为火元素伤害
            request.Element = ElementType.Fire;

            Target.Attack(request.Causer, request);

            Remove();
        }
    }

    // 火+木+土：自燃：下次受到的伤害时敌人受到等量伤害
    public static Effect Fire_Wood_Earth() {
        return new EffectFireWoodEarth();
    }

    private class EffectMetalEarthWater : Effect {
        private readonly EffectTrigger m_trigger;

        public EffectMetalEarthWater() {
            UiName        = "金土水联动";
            UiDescription = "洞察：无效敌方下一次的伤害或控制效果";
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
            m_trigger = new EffectTrigger(this) {
                MaxTriggerTimes = 1
            };
        }

        // 免疫控制效果
        protected override bool OnBeforeAttach(Effect effect) {
            var reject = base.OnBeforeAttach(effect);
            if (m_trigger.InValid) return reject;
            return reject && m_trigger.Trigger(effect.LgTags.Contains(EffectTag.Control));
        }

        // 免疫伤害效果 Todo 可以使用复合buff进行复用
        protected override bool OnBeforeSelfHpChange(RequestHpChange request) {
            var reject = base.OnBeforeSelfHpChange(request);
            if (m_trigger.InValid) return reject;
            return reject && m_trigger.Trigger(!request.IsHeal);
        }
    }

    // 金+土+水：洞察：无效敌方下一次的伤害和控制效果
    public static Effect Metal_Earth_Water() {
        return new EffectMetalEarthWater();
    }

    private class EffectMetalWaterWood : Effect {
        public EffectMetalWaterWood() {
            UiName        = "金水木联动";
            UiDescription = "不竭：本次出牌法力消耗减半，抽一张牌";
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
        }

        protected override void OnAfterPreviewMana(RequestPlayBatchCard request) {
            base.OnAfterPreviewMana(request);

            request.TotalManaCost /= 2;
        }

        protected override void OnBeforePlayBatchCard(RequestPlayBatchCard request) {
            base.OnBeforePlayBatchCard(request);
            Target.GetCard(1);
            Remove();
        }
    }

    // 金+水+木：不竭：本次出牌法力消耗减半，抽一张牌
    public static Effect Metal_Water_Wood() {
        return new EffectMetalWaterWood();
    }

    // 火+土+金：淬炼：为敌方施加三层淬炼效果，每回合开始时消耗一层淬炼效果对敌方造成5%最大生命值物理伤害。
    private class EffectFireEarthMetal : Effect {
        public EffectFireEarthMetal() {
            UiName        = "火土金联动";
            UiDescription = "淬炼：为敌方施加三层淬炼效果，每回合开始时消耗一层淬炼效果对敌方造成5%最大生命值物理伤害";
            LgTags = new HashSet<EffectTag> {
                EffectTag.DeBuff
            };
            LgRemainingRounds = 3;
        }

        protected override void OnBeforeTurnStart() {
            base.OnBeforeTurnStart();

            Causer.Attack(Target, new RequestHpChange {
                Value = 0.05f * Target.State.HealthMax,
                Type  = DamageType.Physical,
            });
        }
    }

    public static Effect Fire_Earth_Metal() {
        return new EffectFireEarthMetal();
    }

    // 水+火+木：击碎：令敌方物理和魔法护盾值各减少20%
    public static Effect Water_Fire_Wood() {
        return new EffectOnce {
            UiName        = "水火木联动",
            UiDescription = "击碎：令敌方物理和魔法护盾值各减少20%",
            UiIconPath    = "",
            LgAction = e =>
            {
                var state = e.Target.State;
                state.PhysicalShield *= 0.8f;
                state.MagicShield    *= 0.8f;
            }
        };
    }

    // 金+木+水+火+土：净化：直接击碎全部敌方法印，斩杀生命值低于10%的敌人，不计入出牌次数。
    public static Effect Metal_Wood_Water_Fire_Earth() {
        return new EffectOnce {
            UiName        = "金木水火土联动",
            UiDescription = "净化：直接击碎全部敌方法印，斩杀生命值低于10%的敌人",
            UiIconPath    = "",
            LgAction = e =>
            {
                // 击碎全部法印 Todo 封装击碎接口
                e.Target.State.ElementAttach.Clear();

                // Todo 斩杀效果
            }
        };
    }

#endregion

#region 元素击碎

    private class EffectElementRecover : EffectTemporary {
        public ElementType LgElementType;

        protected override void OnAfterDetach() {
            base.OnAfterDetach();

            Target.State.ElementAttach[LgElementType] = Target.State.ElementMaxAttach[LgElementType];
        }
    }

    // 法印恢复
    public static Effect Element_Broken_Recover(ElementType elementType, int layer) {
        return new EffectElementRecover {
            UiName            = $"{elementType.ToDescription()}元素法印冷却",
            UiDescription     = $"{elementType.ToDescription()}元素法印冷却",
            UiIconPath        = "",
            LgRemainingRounds = layer,
            LgElementType     = elementType
        };
    }

    // 金：枯竭：令对方造成的物理伤害降低20%（具体数值可能受卡牌效果影响）
    public static Effect Metal_Broken(int layer) {
        return new EffectTemporary {
            UiName            = "枯竭",
            UiDescription     = "造成的物理伤害降低20%",
            UiIconPath        = "",
            LgRemainingRounds = layer,
            LgAddState = {
                PhysicalDamageAmplify = -20
            }
        };
    }

    // 水：破魔：令对方受到的魔法伤害增加20%（具体数值可能受卡牌效果影响）
    public static Effect Water_Broken(int layer) {
        return new EffectTemporary {
            UiName            = "破魔",
            UiDescription     = "受到的魔法伤害增加20%",
            UiIconPath        = "",
            LgRemainingRounds = layer,
            LgAddState = {
                MagicDamageReduce = -20
            },
        };
    }

    // 木：粉碎：对对方造成10%当前生命值击碎卡牌类型的伤害（具体数值可能受卡牌效果影响）
    public static Effect Wood_Broken(int layer) {
        return new EffectOnce {
            UiName            = "粉碎",
            UiDescription     = "对对方造成10%当前生命值金元素伤害",
            UiIconPath        = "",
            LgRemainingRounds = layer,
            LgAction = e =>
            {
                var damage = e.Target.State.Health * 0.1f;
                e.Target.Attack(e.Causer, new RequestHpChange {
                    Value   = damage,
                    Type    = DamageType.Physical,
                    Element = ElementType.Metal
                });
            }
        };
    }

    // 火：破甲：令对方受到的物理伤害增加20%（具体数值可能受卡牌效果影响）
    public static Effect Fire_Broken(int layer) {
        return new EffectTemporary {
            UiName            = "破甲",
            UiDescription     = "受到的物理伤害增加20%",
            UiIconPath        = "",
            LgRemainingRounds = layer,
            LgAddState = {
                PhysicalDamageReduce = -20
            }
        };
    }

    // 土：弱化：令对方造成的魔法伤害降低20%（具体数值可能受卡牌效果影响）
    public static Effect Earth_Broken(int layer) {
        return new EffectTemporary {
            UiName            = "弱化",
            UiDescription     = "造成的魔法伤害降低20%",
            UiIconPath        = "",
            LgRemainingRounds = layer,
            LgAddState = {
                MagicDamageAmplify = -20
            }
        };
    }

#endregion

#region 通用性

    // 物理护盾提升
    public static Effect AddPhysicalArmorExtra(float extra) {
        return new EffectOnce {
            UiHidde  = true,
            LgAction = e => e.Target.State.PhysicalShield *= 1 + extra / 100,
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            }
        };
    }

    // 魔法护盾(魔法护盾都拥有反伤效果，没减少一点魔法护盾值，对敌方造成1点土属性伤害)
    private class EffectMagicAntiArmor : Effect {
        public float Lg1AntiRate = 1;

        public EffectMagicAntiArmor() {
            UiName        = "魔法护盾";
            UiDescription = "令自身魔法护盾值增加1点，每减少1点魔法护盾值，对敌方造成1点土属性伤害";
            UiIconPath    = "";

            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
            LgMaxOverlay = 1000000;
            LgPriority   = 100;
        }

        protected override bool OnBeforeSelfHpChange(RequestHpChange request) {
            if (base.OnBeforeSelfHpChange(request)) return true;
            if (request.IsHeal) return false;
            if (request.Type != DamageType.Magical) return false;
            if (request.Value <= 1) return false;
            var value = Mathf.FloorToInt(Mathf.Min(request.Value, LgOverlay));
            LgOverlay -= value;
            var damage = value * Lg1AntiRate;
            Target.Attack(request.Causer, new RequestHpChange {
                Value   = damage,
                Type    = DamageType.Magical,
                Element = ElementType.Earth,
            });
            if (LgOverlay <= 0) {
                Remove();
            }

            return false;
        }
    }

    public static Effect MagicAntiArmor(int cnt, float rate = 1) {
        return new EffectMagicAntiArmor {
            LgOverlay   = cnt,
            Lg1AntiRate = rate
        };
    }

#endregion

    // Todo 策划填充更多
}

public static class EffectFactory {
    // 元素联动表
    private static readonly Dictionary<ElementType[], (Func<Effect>, bool)> Links = new() {
        {
            // 金+土
            new[] { ElementType.Metal, ElementType.Earth },
            (EffectDetails.Metal_Earth, true)
        }, {
            // 火+土
            new[] { ElementType.Fire, ElementType.Earth },
            (EffectDetails.Fire_Earth, true)
        }, {
            // 水+金
            new[] { ElementType.Water, ElementType.Metal },
            (EffectDetails.Water_Metal, true)
        }, {
            // 火+木
            new[] { ElementType.Fire, ElementType.Wood },
            (EffectDetails.Fire_Wood, false)
        }, {
            // 水+木
            new[] { ElementType.Water, ElementType.Wood },
            (EffectDetails.Water_Wood, true)
        }, {
            // 火+木+土
            new[] { ElementType.Fire, ElementType.Wood, ElementType.Earth },
            (EffectDetails.Fire_Wood_Earth, true)
        }, {
            // 金+土+水
            new[] { ElementType.Metal, ElementType.Earth, ElementType.Water },
            (EffectDetails.Metal_Earth_Water, true)
        }, {
            // 金+水+木
            new[] { ElementType.Metal, ElementType.Water, ElementType.Wood },
            (EffectDetails.Metal_Water_Wood, true)
        }, {
            // 火+土+金
            new[] { ElementType.Fire, ElementType.Earth, ElementType.Metal },
            (EffectDetails.Fire_Earth_Metal, false)
        }, {
            // 水+火+木
            new[] { ElementType.Water, ElementType.Fire, ElementType.Wood },
            (EffectDetails.Water_Fire_Wood, false)
        }, {
            // 金+木+水+火+土
            new[] { ElementType.Metal, ElementType.Wood, ElementType.Water, ElementType.Fire, ElementType.Earth },
            (EffectDetails.Metal_Wood_Water_Fire_Earth, false)
        }
    };

    private static readonly List<(ElementType[], Func<Effect>, bool)> LinkList = new();

    // 元素击碎表
    private static readonly Dictionary<ElementType, Func<int, Effect>> Broken = new() {
        {
            ElementType.Metal,
            EffectDetails.Metal_Broken
        }, {
            ElementType.Water,
            EffectDetails.Water_Broken
        }, {
            ElementType.Wood,
            EffectDetails.Wood_Broken
        }, {
            ElementType.Fire,
            EffectDetails.Fire_Broken
        }, {
            ElementType.Earth,
            EffectDetails.Earth_Broken
        }
    };

    static EffectFactory() {
        foreach (var link in Links) {
            var key = link.Key.OrderBy(t => (int)t).ToArray();
            LinkList.Add((key, link.Value.Item1, link.Value.Item2));
        }
    }

    // 元素联动效果
    public static (Effect, bool) ElementLink(ElementType[] types) {
        types = types.OrderBy(t => (int)t).ToArray();
        var index = LinkList.FindIndex(t => t.Item1.SequenceEqual(types));
        if (index == -1) return (null, false);
        var item = LinkList[index];
        return (item.Item2(), item.Item3);
    }

    // 元素击碎效果
    public static Effect ElementBroken(ElementType type, int layer) {
        return Broken.TryGetValue(type, out var func) ? func(layer) : null;
    }
}
}
