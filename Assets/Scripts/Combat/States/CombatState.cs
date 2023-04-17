using System;
using System.Linq;
using System.Reflection;

namespace Combat.States {
// 所有的战斗属性
public class CombatState : CombatAddableState {
    private float m_health;
    private float m_mana;

#region 计算字段

    // 玩家最大生命值
    public float HealthMax => HealthMaxBase * (1 + HealthMaxPercent / 100) + HealthMaxExtra;

    // 玩家最大法力
    public float ManaMax => ManaMaxBase * (1 + ManaMaxPercent / 100) + ManaMaxExtra;

    // 玩家是否死亡(生命值归零且无元素附着)
    public bool IsDead => Health <= 0 && ElementAttach.Count == 0;

#endregion

#region 区间约束字段

    // 玩家当前生命值
    [ListenChange]
    public float Health {
        get => m_health;
        set => m_health = Math.Clamp(value, 0, HealthMax);
    }

    // 玩家当前法力
    [ListenChange]
    public float Mana {
        get => m_mana;
        set => m_mana = Math.Clamp(value, 0, ManaMax);
    }

#endregion

#region 广播修改事件

    public event Action<CombatState> OnStateChange;

    private static readonly FieldInfo[] Fields = typeof(CombatState)
        .GetFields(BindingFlags.Public | BindingFlags.Instance)
        .Where(field => field.GetCustomAttribute<ListenChangeAttribute>() != null)
        .ToArray();

    private static readonly PropertyInfo[] Properties = typeof(CombatState)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(field => field.GetCustomAttribute<ListenChangeAttribute>() != null)
        .ToArray();

    private CombatState m_record;
    private bool        m_anyChange;

    public void BeginRecord() {
        m_record    = new CombatState();
        m_anyChange = false;
        foreach (var field in Fields) {
            var value = field.GetValue(this);
            field.SetValue(m_record, value);
        }
        foreach (var property in Properties) {
            var value = property.GetValue(this);
            property.SetValue(m_record, value);
        }
    }

    public void EndRecord() {
        var delta = new CombatState();
        foreach (var field in Fields) {
            var oldValue = field.GetValue(m_record);
            var newValue = field.GetValue(this);
            if (Equals(oldValue, newValue)) continue;
            var differ = DynamicOperator.ForceSub(newValue, oldValue);
            field.SetValue(delta, differ);
            m_anyChange = true;
        }
        foreach (var property in Properties) {
            var oldValue = property.GetValue(m_record);
            var newValue = property.GetValue(this);
            if (Equals(oldValue, newValue)) continue;
            var differ = DynamicOperator.ForceSub(newValue, oldValue);
            property.SetValue(delta, differ);
            m_anyChange = true;
        }
        if (!m_anyChange) return;
        OnStateChange?.Invoke(delta);
    }

#endregion
}
}
