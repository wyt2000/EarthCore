using System.Collections.Generic;
using Combat.Effects;
using Combat.Effects.Templates;
using Combat.Requests;
using UnityEngine;

namespace Designs {
// 本文件只填配置相关的信息,有关计算的都放到Templates里面去
public static class EffectDetails {
#region 元素联动

    // 金+土：免疫：免疫下一次伤害
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
        protected override bool OnBeforeSelfHpChange(HealthRequest request) {
            var reject = base.OnBeforeSelfHpChange(request);
            if (m_trigger.InValid) return reject;
            return reject && m_trigger.Trigger(!request.IsHeal);
        }
    }

    public static Effect Metal_Earth() {
        return new EffectMetalEarth();
    }


    /// <summary>
    /// 火+土：固定：令自身魔法护盾值增加20% 
    /// </summary>
    /// <returns></returns>
    public static Effect Fire_Earth() {
        return new EffectTemporary() {
            UiName        = "火土联动",
            UiDescription = "固定：令自身魔法护盾值增加20%",
            LgAddState = {
                MagicResistancePercent = 20
            },
            LgRemainingRounds = 2,
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            }
        };
    }

    // 水+金：滋润：令自身物理护盾值增加20%
    public static Effect Water_Metal() {
        return new EffectTemporary() {
            UiName        = "水金联动",
            UiDescription = "滋润：令自身物理护盾值增加20%",
            LgAddState = {
                PhysicalArmorPercent = 20
            },
            LgRemainingRounds = 2,
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

        protected override void OnAfterTakeHpChange(HealthRequest request) {
            base.OnAfterTakeHpChange(request);
            if (request.IsHeal || m_trigger.InValid) return;
            Target.Attack(request.Target, new HealthRequest {
                Value = Mathf.Max(1, 0.02f * request.Target.State.Health),
                DamageParams = {
                    DamageType = DamageType.Physical,
                }
            });
            m_trigger.Trigger();
        }
    }

    // 火+木：引燃：令敌方获得5层燃烧，每次攻击消耗一层燃烧对敌人造成敌人2%当前生命值额外物理伤害（额外伤害小于1时改为1）
    public static Effect Fire_Wood() {
        return new EffectFireWood();
    }

    // Todo 水+木：宁静：本次出牌法力消耗减半
    /*
    protected override void OnBeforePlayCard(Card card) {
        base.OnBeforePlayCard(card);

        card.ManaCost /= 2;
        
        Remove();
    }
     */

    private class EffectFireWoodEarth : Effect {
        public EffectFireWoodEarth() {
            UiName        = "火木土联动";
            UiDescription = "自燃：下次受到的伤害时敌人受到等量火元素伤害";
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
        }

        protected override void OnAfterSelfHpChange(HealthRequest request) {
            base.OnAfterSelfHpChange(request);

            // 忽略治疗请求
            if (request.IsHeal) return;

            // 修改为火元素伤害
            request.DamageParams.Element = ElementType.Fire;

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
        protected override bool OnBeforeSelfHpChange(HealthRequest request) {
            var reject = base.OnBeforeSelfHpChange(request);
            if (m_trigger.InValid) return reject;
            return reject && m_trigger.Trigger(!request.IsHeal);
        }
    }

    // 金+土+水：洞察：无效敌方下一次的伤害和控制效果
    public static Effect Metal_Earth_Water() {
        return new EffectMetalEarthWater();
    }

    // Todo 金+水+木：不竭：本次出牌法力消耗减半，抽一张牌

    // Todo 火+土+金：淬炼：为敌方施加三层淬炼效果，每回合开始时消耗一层淬炼效果对敌方造成5%最大生命值物理伤害。

    // Todo 水+火+木：击碎：令敌方物理和魔法护盾值各减少20%
    public static Effect JiSui() {
        return new EffectTemporary() {
            UiName            = "水火木联动",
            UiDescription     = "",
            UiIconPath        = "",
            LgRemainingRounds = 3,
            LgAddState = {
                PhysicalArmorPercent   = -20,
                MagicResistancePercent = -20,
            }
        };
    }

    //Todo  金+木+水+火+土：净化：直接击碎全部敌方法印，斩杀生命值低于10%的敌人，不计入出牌次数。

#endregion

#region 通用性

    // 物理护盾百分比提升
    public static Effect AddPhysicalArmorPercent(float percent, int round) {
        return new EffectTemporary {
            UiName        = "物理护盾提升",
            UiDescription = $"令自身物理护盾值增加{percent}%",
            UiIconPath    = "", // Todo
            LgAddState = {
                PhysicalArmorPercent = percent
            },
            LgRemainingRounds = round,
            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            }
        };
    }

    // 物理护盾额外提升
    public static Effect AddPhysicalArmorExtra(float extra, int round) {
        return new EffectTemporary {
            UiName        = "物理护盾提升",
            UiDescription = $"令自身物理护盾值增加{extra}",
            UiIconPath    = "", // Todo
            LgAddState = {
                PhysicalArmorExtra = extra
            },
            LgRemainingRounds = round,
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
            UiIconPath    = ""; // Todo

            LgTags = new HashSet<EffectTag> {
                EffectTag.Buff
            };
            LgMaxOverlay = 1000000;
            LgPriority   = 100;
        }

        protected override bool OnBeforeSelfHpChange(HealthRequest request) {
            if (base.OnBeforeSelfHpChange(request)) return true;
            if (request.IsHeal) return false;
            if (request.DamageParams.DamageType != DamageType.Magical) return false;
            if (request.Value <= 1) return false;
            var value = Mathf.FloorToInt(Mathf.Min(request.Value, LgOverlay));
            LgOverlay -= value;
            var damage = value * Lg1AntiRate;
            Target.Attack(request.Causer, new HealthRequest {
                Value = damage,
                DamageParams = {
                    DamageType = DamageType.Magical,
                    Element    = ElementType.Earth,
                },
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
}
