using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Combat.Cards;
using Combat.Effects.Templates;
using Combat.Enums;
using Combat.Requests.Details;
using Utils;

// Todo 检查tag和causer&target的使用是否合理
// Todo 测试effect逻辑
namespace Combat.Effects {
[SuppressMessage("ReSharper", "UnusedMember.Local")]
// 元素联动相关效果
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

    public static (Effect, bool) GetElementLink(IEnumerable<Card> cards) {
        var types = cards.Select(c => c.LgElement).Where(t => t != null).Cast<ElementType>().ToHashSet();
        return GetElementLink(types);
    }

    private static (Effect, bool) GetElementLink(IEnumerable<ElementType> types) {
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
        return trigger.Bind(new Effect {
            UiName        = "免疫",
            UiDescription = "金土联动,免疫下一次伤害",
            LgTags        = { EffectTag.Buff },

            OnImpBeforeSelfHpChange = (_, req) => trigger.Trigger(!req.IsHeal)
        });
    }

    [ElementLink(ElementType.Huo, ElementType.Tu)]
    private static Effect HuoTu() {
        return new EffectOnce {
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
            LgTags        = { EffectTag.DeBuff },
            LgOverlay     = 5,
            LgOpenMerge   = true,

            OnImpAfterTakeHpChange = (self, req) =>
            {
                if (req.IsHeal || req.Reason == "燃烧") return;
                var state = self.Target.State;
                var damage = state.Health * 0.02f;
                if (damage < 1) damage = 1;
                self.Causer.Attack(new RequestHpChange {
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
        return new EffectBatch {
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
                self.Target.Attack(new RequestHpChange {
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
        var trigger = new EffectMultipleTrigger(true, 1, 1);
        return trigger.Bind(new Effect {
            UiName        = "洞察",
            UiDescription = "金土水联动,无效敌方下一次的伤害和控制效果",
            LgTags        = { EffectTag.Buff },

            OnImpBeforeSelfHpChange = (_, req) => trigger.Trigger(!req.IsHeal && req.Value > 0,                 0),
            OnImpRejectAttach       = (_, effect) => trigger.Trigger(effect.LgTags.Contains(EffectTag.Control), 1)
        });
    }

    [ElementLink(ElementType.Jin, ElementType.Shui, ElementType.Mu)]
    private static Effect JinShuiMu() {
        return new EffectBatch {
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
        return EffectPrefabs.CuiLian(5);
    }

    // 水+火+木：击碎：令敌方物理和魔法护盾值各减少20%
    [ElementLink(true, ElementType.Shui, ElementType.Huo, ElementType.Mu)]
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
    [ElementLink(true, ElementType.Jin, ElementType.Mu, ElementType.Shui, ElementType.Huo, ElementType.Tu)]
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
                causer.AddPost(() =>
                {
                    var damage = state.HealthMax * 0.1f;
                    if (state.Health > damage) return;
                    // 实现斩杀效果 
                    causer.Attack(new RequestHpChange {
                        Value  = state.HealthMax,
                        IsReal = true,
                        Reason = "元素联动斩杀"
                    });
                });
            }
        };
    }

#endregion
}

// 元素击碎相关效果
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
                e.Causer.Attack(new RequestHpChange {
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

// 通用效果的预制体
public static class EffectPrefabs {
    // 清算
    public static Effect QingSuan(int layer) {
        return new Effect {
            UiName        = "清算",
            UiDescription = "回合结束时对敌人造成等量清算值的水属性魔法伤害",
            UiIconPath    = "",

            LgTags      = { EffectTag.Buff },
            LgOverlay   = layer,
            LgOpenMerge = true,

            OnImpAfterTurnEnd = self =>
            {
                var damage = self.LgOverlay;
                self.Causer.Attack(new RequestHpChange {
                    Value   = damage,
                    Type    = DamageType.Magical,
                    Element = ElementType.Shui,

                    Reason = "清算"
                });
                self.Remove();
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

            OnImpAfterTurnStart = self =>
            {
                var state = self.Target.State;
                var damage = state.HealthMax * 0.01f * self.LgOverlay;
                self.Causer.Attack(new RequestHpChange {
                    Value = damage,
                    Type  = DamageType.Physical,

                    Reason = "淬炼"
                });
                if (--self.LgOverlay == 0) self.Remove();
            }
        };
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Local")]
// 固有效果
public static class EffectFixed {
    private static readonly List<Func<Effect>> All = new();

    static EffectFixed() {
        var type = typeof(EffectFixed);
        var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var method in methods) {
            if (method.ReturnType != typeof(Effect) || method.GetParameters().Length != 0) continue;
            All.Add(() => (Effect)method.Invoke(null, null));
        }
    }

    public static IEnumerable<Effect> GetAll(CombatantComponent target) {
        return All.Select(func =>
        {
            var ret = func();
            ret.Causer = target;
            ret.Target = target;

            ret.LgTags = new HashSet<EffectTag> { EffectTag.Fixed };

            ret.LgRemainingRounds = 0;

            ret.LgPriority = int.MinValue;

            return ret;
        });
    }

#region 效果细节

    // 摸牌
    private static Effect MoPai() => new() {
        UiHidde             = true,
        OnImpAfterTurnStart = self => self.Target.GetCard(2)
    };

    // 回复法力25%
    private static Effect HuiFuFaLi() => new() {
        UiHidde             = true,
        OnImpAfterTurnStart = self => self.Target.State.Mana += self.Target.State.ManaMax * 0.25f
    };

    // 护甲
    private static Effect HuJia() => new() {
        UiName            = "护甲",
        UiDescription     = "每回合开始时,若护甲值小于物理护甲值,则护甲值增加至物理护甲值",
        OnImpBeforeRender = (self, view) => view.effectName.text = $"{self.UiName}x{(int)self.Target.State.PhysicalArmor}",
        OnImpAfterTurnStart = self =>
        {
            var state = self.Target.State;
            state.PhysicalShield = Math.Max(state.PhysicalArmor, state.PhysicalShield);
        }
    };

    // 魔法护盾 
    private static Effect MoFaHuDun() => new() {
        UiName        = "魔力屏障",
        UiDescription = "使用魔法护盾抵消魔法伤害时,对敌人造成等量土属性物理伤害",
        OnImpAfterSelfHpChange = (self, req) =>
        {
            if (req.IsHeal || req.OutShieldChange <= 0) return;
            self.Target.Attack(new RequestHpChange {
                Value   = req.OutShieldChange,
                Type    = DamageType.Physical,
                Element = ElementType.Tu,
                Reason  = "魔力屏障"
            });
        }
    };

#endregion
}
}
