using System.Collections.Generic;
using System.Reflection;
using Combat.Enums;
using Utils;

namespace Combat.States {
public enum CombatBlockTag {
    // 属性封锁:不能使用某种固定属性卡牌
    BlockElement,

    // 摸牌封锁:不能抽牌
    BlockGetCard,

    // 联动封锁:无法触发元素联动
    BlockElementLink,

    // 叠加封锁:无法触发元素叠加
    BlockElementOverlap,

    // 护盾封锁:无法获得(魔法/物理)护盾
    BlockShield,

    // 护甲封锁:无法获得物理护甲
    BlockArmor,

    // 伤害封锁:无法受到伤害
    BlockDamage,

    // 物理伤害封锁:无法受到物理伤害
    BlockPhysicalDamage,

    // 魔法伤害封锁:无法受到魔法伤害
    BlockMagicDamage,
}

// 满足可加性的战斗属性,可以自动附着和脱离
public class CombatAddableState {
#region 可加性属性

    // 最大生命值
    public float HealthMaxBase;
    public float HealthMaxPercent;
    public float HealthMaxExtra;

    // 最大法力值
    public float ManaMaxBase;
    public float ManaMaxPercent;
    public float ManaMaxExtra;

    // 物理护盾
    public float PhysicalShield;

    // 魔法护盾(反伤物理伤害)
    public float MagicShield;

    // 物理伤害增幅(计算前)/减免(计算后)
    public float PhysicalDamageAmplify;
    public float PhysicalDamageReduce;

    // 魔法伤害增幅(计算前)/减免(计算后)
    public float MagicDamageAmplify;
    public float MagicDamageReduce;

    // 玩家结束时最大手牌数
    public int MaxCardCnt;

    // 元素法印(最大)层数
    public AddableDict<ElementType, int> ElementAttach    = new();
    public AddableDict<ElementType, int> ElementMaxAttach = new();

    // tag容器
    public AddableDict<CombatBlockTag, int> BlockTags = new();

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
}
