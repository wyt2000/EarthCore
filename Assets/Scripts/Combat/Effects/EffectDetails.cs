using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Combat.Effects.Templates;
using Combat.Enums;
using Combat.Requests.Details;
using Utils;

// Todo 检查tag和causer&target的使用是否合理
// Todo 测试effect逻辑
namespace Combat.Effects {
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class EffectLinks {
    [AttributeUsage(AttributeTargets.Method)]
    private class ElementLinkAttribute : Attribute {
        // 联动类型
        public readonly ElementType[] Types;

        // 是否施加给对方
        public readonly bool ToOther;

        public ElementLinkAttribute(params ElementType[] types)
            : this(false, types) { }

        public ElementLinkAttribute(bool toOther, params ElementType[] types) {
            Types   = types.OrderBy(t => (int)t).ToArray();
            ToOther = toOther;
        }
    }

    private static readonly List<(ElementLinkAttribute, Func<Effect>)> Links = new();

    static EffectLinks() {
        // 用反射扫描所有方法
        var methods = typeof(EffectLinks).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var method in methods) {
            var attr = method.GetCustomAttribute<ElementLinkAttribute>();
            if (attr == null) continue;
            var func = (Func<Effect>)Delegate.CreateDelegate(typeof(Func<Effect>), method);
            Links.Add((attr, func));
        }
    }

    public static (Effect, bool) GetElementLink(IEnumerable<ElementType> types) {
        var arr = types.OrderBy(t => (int)t).ToArray();
        foreach (var (attr, func) in Links) {
            if (attr.Types.SequenceEqual(arr)) {
                return (func(), attr.ToOther);
            }
        }

        return (null, false);
    }

#region 元素联动细节实现

    [ElementLink(ElementType.Jin, ElementType.Tu)]
    private static Effect JinTu() {
        var trigger = new EffectTrigger();
        var effect = new Effect {
            UiName        = "免疫",
            UiDescription = "金土联动,免疫下一次伤害",
            LgTags        = { EffectTag.Buff },

            OnImpBeforeSelfHpChange = (_, req) => trigger.Trigger(!req.IsHeal)
        };
        return trigger.Bind(effect);
    }

    [ElementLink(ElementType.Huo, ElementType.Tu)]
    private static Effect HuoTu() {
        return new Effect {
            UiName        = "固定",
            UiDescription = "火土联动,令自身魔法护盾值增加20%",
            LgTags        = { EffectTag.Buff },

            OnImpAfterAttach = self =>
            {
                var state = self.Target.State;
                state.MagicShield *= 1 + 0.2f;
            }
        };
    }

    [ElementLink(ElementType.Shui, ElementType.Jin)]
    private static Effect ShuiJin() {
        return new Effect {
            UiName        = "滋润",
            UiDescription = "水金联动,令自身物理护盾值增加20%",
            LgTags        = { EffectTag.Buff },

            OnImpAfterAttach = self =>
            {
                var state = self.Target.State;
                state.PhysicalShield *= 1 + 0.2f;
            }
        };
    }

    [ElementLink(true, ElementType.Huo, ElementType.Mu)]
    private static Effect HuoMu() {
        return new Effect {
            UiName        = "引燃",
            UiDescription = "火木联动,令敌方获得5层燃烧,每次攻击消耗一层燃烧对敌人造成敌人2%当前生命值额外物理伤害（额外伤害小于1时改为1，属性与触发燃烧效果的攻击相同）（永久）",
            LgTags        = { EffectTag.Buff },
            LgOverlay     = 5,
            LgOpenMerge   = true,

            OnImpAfterTakeHpChange = (self, req) =>
            {
                if (req.IsHeal || req.Reason == "燃烧") return;
                var state = self.Target.State;
                var damage = state.Health * 0.02f;
                if (damage < 1) damage = 1;
                self.Target.Attack(req.Target, new RequestHpChange {
                    Value   = damage,
                    Type    = DamageType.Physical,
                    Element = req.Element,

                    Reason = "燃烧"
                });
                if (--self.LgOverlay == 0) self.Remove();
            }
        };
    }

    [ElementLink(ElementType.Shui, ElementType.Mu)]
    private static Effect ShuiMu() {
        return new Effect {
            UiName        = "宁静",
            UiDescription = "水木联动,本次出牌法力消耗减半",
            LgTags        = { EffectTag.Buff },

            OnImpAfterPreviewMana = (_, req) => req.TotalManaCost /= 2
        };
    }

    [ElementLink(ElementType.Huo, ElementType.Mu, ElementType.Tu)]
    private static Effect HuoMuTu() {
        return new Effect {
            UiName        = "自燃",
            UiDescription = "火木土联动,下次受到的伤害时敌人受到等量对应属性对应类型的伤害（持续到下次伤害到来）",
            LgTags        = { EffectTag.Buff },

            OnImpAfterSelfHpChange = (self, req) =>
            {
                if (req.IsHeal) return;
                var damage = req.Value;
                self.Target.Attack(self.Target, new RequestHpChange {
                    Value   = damage,
                    Type    = DamageType.Physical,
                    Element = req.Element,

                    Reason = "自然"
                });
                self.Remove();
            }
        };
    }

    // 金+土+水：洞察：无效敌方下一次的伤害和控制效果（分两部分，分别持续到下次伤害到来和下次控制到来）
    [ElementLink(ElementType.Jin, ElementType.Tu, ElementType.Shui)]
    private static Effect JinTuShui() {
        // Todo 分成两个独立buff实现
        var trigger = new EffectTrigger();
        return trigger.Bind(new Effect {
            UiName        = "洞察",
            UiDescription = "金土水联动,无效敌方下一次的伤害和控制效果（分两部分，分别持续到下次伤害到来和下次控制到来）",
            LgTags        = { EffectTag.Buff },

            OnImpBeforeSelfHpChange = (_, req) => trigger.Trigger(!req.IsHeal && req.Value > 0),
            OnImpRejectAttach       = (_, effect) => trigger.Trigger(effect.LgTags.Contains(EffectTag.Control))
        });
    }

    [ElementLink(ElementType.Jin, ElementType.Shui, ElementType.Mu)]
    private static Effect JinShuiMu() {
        return new Effect {
            UiName        = "不竭",
            UiDescription = "金水木联动,本次出牌法力消耗减半，抽一张牌",
            LgTags        = { EffectTag.Buff },

            OnImpAfterPreviewMana   = (_,    req) => req.TotalManaCost /= 2,
            OnImpAfterPlayBatchCard = (self, _) => self.Target.GetCard(1)
        };
    }

    //火+土+金：淬炼：为敌方施加5层淬炼效果，每回合开始时消耗一层淬炼对敌方造成（1%*淬炼层数）最大生命值物理伤害。（永久）
    [ElementLink(true, ElementType.Huo, ElementType.Tu, ElementType.Jin)]
    private static Effect HuoTuJin() {
        return EffectDetails.CuiLian(5);
    }

    // 水+火+木：击碎：令敌方物理和魔法护盾值各减少20%
    [ElementLink(ElementType.Shui, ElementType.Huo, ElementType.Mu)]
    private static Effect ShuiHuoMu() {
        return new EffectOnce {
            UiName        = "击碎",
            UiDescription = "水火木联动,令敌方物理和魔法护盾值各减少20%",
            UiHidde       = true,

            LgTags = { EffectTag.DeBuff },
            LgAction = self =>
            {
                var state = self.Target.State;
                state.MagicShield    *= 0.8f;
                state.PhysicalShield *= 0.8f;
            }
        };
    }

    // 金+木+水+火+土：净化：直接击碎全部敌方法印，斩杀生命值低于10%的敌人。
    [ElementLink(ElementType.Jin, ElementType.Mu, ElementType.Shui, ElementType.Huo, ElementType.Tu)]
    private static Effect JinMuShuiHuoTu() {
        return new EffectOnce {
            UiName        = "净化",
            UiDescription = "金木水火土联动,直接击碎全部敌方法印，斩杀生命值低于10%的敌人。",
            LgTags        = { EffectTag.Buff },

            LgAction = self =>
            {
                var causer = self.Causer;
                var target = self.Target;
                var state = target.State;
                // 元素击碎
                var keys = state.ElementAttach.Keys.ToArray();
                keys.ForEach(key => target.TryApplyElementBreak(causer, key, state.ElementMaxAttach[key]));
                causer.Judge.Requests.Add(new RequestPostLogic {
                    OnFinish = () =>
                    {
                        if (state.Health >= state.HealthMax * 0.1) return;
                        // Todo 实现斩杀效果
                        state.Health = 0;
                    }
                });
            }
        };
    }

#endregion
}

public static class EffectBroken {
    // 元素击碎表
    private static readonly Dictionary<ElementType, Func<int, Effect>> Broken = new() {
        { ElementType.Jin, Jin },
        { ElementType.Mu, Mu },
        { ElementType.Shui, Shui },
        { ElementType.Huo, Huo },
        { ElementType.Tu, Tu },
    };

    // 元素击碎效果
    public static Effect GetElementBroken(ElementType type, int layer) {
        return Broken.TryGetValue(type, out var func) ? func(layer) : null;
    }

    // 法印恢复效果
    public static Effect GetElementBrokenRecover(ElementType elementType, int layer) {
        return new EffectTemporary {
            UiName        = "法印冷却",
            UiDescription = $"{elementType.ToDescription()}元素法印冷却",
            UiIconPath    = "",

            LgRemainingRounds = layer,

            OnImpAfterDetach = self =>
            {
                var state = self.Target.State;
                state.ElementAttach[elementType] = state.ElementMaxAttach[elementType];
            }
        };
    }

#region 元素击碎细节实现

    private static Effect Jin(int layer) {
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

    private static Effect Mu(int layer) {
        return new EffectOnce {
            UiName            = "粉碎",
            UiDescription     = "对对方造成10%当前生命值金元素伤害",
            UiHidde           = true,
            LgRemainingRounds = layer,
            LgAction = e =>
            {
                var damage = e.Target.State.Health * 0.1f;
                e.Target.Attack(e.Causer, new RequestHpChange {
                    Value   = damage,
                    Type    = DamageType.Physical,
                    Element = ElementType.Jin,

                    Reason = "粉碎"
                });
            }
        };
    }

    private static Effect Shui(int layer) {
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

    private static Effect Huo(int layer) {
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

    private static Effect Tu(int layer) {
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
}

public static class EffectDetails {
    // 清算
    public static Effect QingSuan(int layer) {
        return new Effect {
            UiName        = "清算",
            UiDescription = "回合结束时对敌人造成等量清算值的水属性魔法伤害",
            UiIconPath    = "",

            LgRemainingRounds = 1,
            LgOverlay         = layer,
            LgOpenMerge       = true,

            OnImpAfterDetach = self =>
            {
                var damage = self.LgOverlay;
                self.Causer.Attack(self.Target, new RequestHpChange {
                    Value   = damage,
                    Type    = DamageType.Magical,
                    Element = ElementType.Shui,

                    Reason = "清算"
                });
            }
        };
    }

    // 淬炼
    public static Effect CuiLian(int layer) {
        return new Effect {
            UiName        = "淬炼",
            UiDescription = "每回合开始时消耗一层淬炼对敌方造成（1%*淬炼层数）最大生命值物理伤害。",

            LgTags      = { EffectTag.DeBuff },
            LgOverlay   = layer,
            LgOpenMerge = true,

            OnImpBeforeTurnStart = self =>
            {
                var state = self.Target.State;
                var damage = state.HealthMax * 0.01f * self.LgOverlay;
                self.Causer.Attack(self.Target, new RequestHpChange {
                    Value = damage,
                    Type  = DamageType.Physical,

                    Reason = "淬炼"
                });
                if (--self.LgOverlay == 0) self.Remove();
            }
        };
    }
}
}
