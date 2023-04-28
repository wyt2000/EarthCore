using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Combat.Cards;
using Combat.Effects;
using Combat.Enums;

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
        set => m_health = m_isRecording ? value : Math.Clamp(value, 0, HealthMax);
    }

    // 玩家当前法力
    [ListenChange]
    public float Mana {
        get => m_mana;
        set => m_mana = m_isRecording ? value : Math.Clamp(value, 0, ManaMax);
    }

#endregion

#region 复杂状态字段

    // 玩家的效果
    public readonly SortedSet<Effect> Effects;

    // 玩家的手牌
    public readonly List<Card> Cards;

    // 玩家的牌堆
    public readonly CardHeap Heap;
    
    // 玩家立绘资源路径
    public string SpritePath;
    
    // 玩家立绘背景色调
    public ElementType? SpriteColor;

#endregion

    public CombatState() : this(false) { }

    private CombatState(bool isRecording) {
        m_isRecording = isRecording;
        if (isRecording) return;
        Effects = new SortedSet<Effect>();
        Cards   = new List<Card>();
        Heap    = new CardHeap();
    }

#region 广播修改事件

    // old , cur , delta
    public event Action<CombatState, CombatState, CombatState> OnStateChange;

    private static readonly FieldInfo[] Fields = typeof(CombatState)
        .GetFields(BindingFlags.Public | BindingFlags.Instance)
        .Where(field => field.GetCustomAttribute<ListenChangeAttribute>() != null)
        .ToArray();

    private static readonly PropertyInfo[] Properties = typeof(CombatState)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(field => field.GetCustomAttribute<ListenChangeAttribute>() != null)
        .ToArray();

    private CombatState m_record;

    private bool m_anyChange;

    private readonly bool m_isRecording;

    public void BeginRecord() {
        m_record    = new CombatState(true);
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
        var delta = new CombatState(true);
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
        OnStateChange?.Invoke(m_record, this, delta);
    }

#endregion
}
}
