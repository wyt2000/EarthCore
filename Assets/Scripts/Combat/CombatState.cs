using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Combat.Requests;
using Utils;

namespace Combat {
public class AddableDict<T> : Dictionary<string, T>
where T : struct, IComparable, IConvertible, IEquatable<T>, IFormattable {
    private void RemoveDefault() {
        var keys = new List<string>();
        foreach (var (key, value) in this) {
            if (value.Equals(default)) {
                keys.Add(key);
            }
        }

        foreach (var key in keys) {
            Remove(key);
        }
    }

    public static AddableDict<T> operator +(AddableDict<T> a, AddableDict<T> b) {
        var result = new AddableDict<T>();
        foreach (var (key, value) in a) {
            result[key] = value;
        }

        foreach (var (key, value) in b) {
            if (result.ContainsKey(key)) {
                result[key] = DynamicOperator.ForceAdd(result[key], value);
            }
            else {
                result[key] = value;
            }
        }

        result.RemoveDefault();
        return result;
    }

    public static AddableDict<T> operator -(AddableDict<T> a, AddableDict<T> b) {
        var result = new AddableDict<T>();
        foreach (var (key, value) in a) {
            result[key] = value;
        }

        foreach (var (key, value) in b) {
            if (result.ContainsKey(key)) {
                result[key] = DynamicOperator.ForceSub(result[key], value);
            }
            else {
                result[key] = DynamicOperator.ForceSub(default, value);
            }
        }

        result.RemoveDefault();
        return result;
    }
}

// 满足可加性的战斗属性
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
public class CombatAddableState {
#region 可加性属性

    // 玩家最大生命值
    public float HealthMaxBase;
    public float HealthMaxPercent;
    public float HealthMaxExtra;

    // 玩家最大法力值
    public float ManaMaxBase;
    public float ManaMaxPercent;
    public float ManaMaxExtra;

    // 玩家物理护甲
    public float PhysicalArmorBase;
    public float PhysicalArmorPercent;
    public float PhysicalArmorExtra;

    // 玩家魔法抗性
    public float MagicResistanceBase;
    public float MagicResistancePercent;
    public float MagicResistanceExtra;

    // 自定义float表单
    public AddableDict<float> CustomFloats = new();

    // 自定义int表单
    public AddableDict<int> CustomInts = new();

#endregion

#region 公开接口

    private static readonly IList<FieldInfo> AddableFields = typeof(CombatAddableState).GetFields(BindingFlags.Public | BindingFlags.Instance);

    public void Add(CombatAddableState state) {
        foreach (var field in AddableFields) {
            var obj = DynamicOperator.ForceAdd(field.GetValue(this), field.GetValue(state));
            field.SetValue(this, obj);
        }
    }

    public void Sub(CombatAddableState state) {
        foreach (var field in AddableFields) {
            var obj = DynamicOperator.ForceSub(field.GetValue(this), field.GetValue(state));
            field.SetValue(this, obj);
        }
    }

    public CombatAddableState Neg() {
        var ret = new CombatAddableState();
        ret.Sub(this);
        return ret;
    }

#endregion
}

// 所有的战斗属性
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CombatState : CombatAddableState {
    private float m_health;
    private float m_mana;

#region 计算字段

    // 玩家最大生命值
    public float HealthMax => HealthMaxBase * (1 + HealthMaxPercent / 100) + HealthMaxExtra;

    // 玩家最大法力
    public float ManaMax => ManaMaxBase * (1 + ManaMaxPercent / 100) + ManaMaxExtra;

    // 玩家物理护甲
    public float PhysicalArmor => PhysicalArmorBase * (1 + PhysicalArmorPercent / 100) + PhysicalArmorExtra;

    // 玩家魔法抗性
    public float MagicResistance => MagicResistanceBase * (1 + MagicResistancePercent / 100) + MagicResistanceExtra;

#endregion

#region 区间约束字段

    // 玩家当前生命值
    public float Health {
        get => m_health;
        set => m_health = Math.Clamp(value, 0, HealthMax);
    }

    // 玩家当前法力
    public float Mana {
        get => m_mana;
        set => m_mana = Math.Clamp(value, 0, ManaMax);
    }

#endregion

#region 复杂机制字段

    // 玩家等级
    public int Level;

    // 玩家经验值
    public int Exp;

#endregion

#region 公开接口

    // 在逻辑层处理生命修改请求
    public void ApplyHealthChange(HealthRequest request) {
        request.Value = request.GetValue();
        var causer = request.Causer;
        var target = request.Target;
        causer.BoardCast(effect => effect.BeforeTakeHpChange(request));
        if (target.BoardCastAny(effect => effect.BeforeSelfHpChange(request))) return;

        var value = request.Value;
        if (!request.IsHeal) {
            value -= request.DamageParams.DamageType switch {
                DamageType.Physical => PhysicalArmor,
                DamageType.Magical  => MagicResistance,

                _ => throw new ArgumentOutOfRangeException()
            };
        }

        value = Math.Max(0, value);

        // Todo 处理元素击碎相关效果

        var old = Health;
        if (request.IsHeal) {
            Health += value;
        }
        else {
            Health -= value;
        }

        var change = Health - old;

        request.Value = Math.Abs(change);
        target.BoardCast(effect => effect.AfterSelfHpChange(request));
        causer.BoardCast(effect => effect.AfterTakeHpChange(request));

        request.OnFinish.Invoke(request);
    }

#endregion
}
}
