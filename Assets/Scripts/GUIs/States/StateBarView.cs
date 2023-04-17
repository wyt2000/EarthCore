using Combat;
using Combat.Enums;
using UnityEngine;
using Utils;

namespace GUIs.States {
// Todo 使用layout组件优化布局
public class StateBarView : MonoBehaviour {
#region prefab配置

    public CombatantComponent combatant;

    [SerializeField]
    private StateFieldView health;

    [SerializeField]
    private StateFieldView mana;

    [SerializeField]
    private StateFieldView physicalShield;

    [SerializeField]
    private StateFieldView physicalAmplify;

    [SerializeField]
    private StateFieldView physicalReduce;

    [SerializeField]
    private StateFieldView magicShield;

    [SerializeField]
    private StateFieldView magicAmplify;

    [SerializeField]
    private StateFieldView magicReduce;

    // 元素法印
    [SerializeField]
    private StateFieldView[] elementSeals;

#endregion

    private void Start() {
        BindAll();
        FreshUI();
    }

    // Todo 监听StateChange,统一所有字段的动画
    public void FreshUI() {
        // Todo 血条滚动特效
        health.FreshUI();
        mana.FreshUI();
        // Todo 数字滚动特效
        physicalShield.FreshUI();
        physicalAmplify.FreshUI();
        physicalReduce.FreshUI();
        magicShield.FreshUI();
        magicAmplify.FreshUI();
        magicReduce.FreshUI();
        // Todo 法印破碎/重置特效
        elementSeals.ForEach(element => element.FreshUI());
    }

    private void BindAll() {
        var state = combatant.State;
        health.OnShow         = () => $"{state.Health:F0}/{state.HealthMax}";
        health.tooltip.OnShow = () => "生命值";
        mana.OnShow = () =>
        {
            var batch = combatant.PreviewBatch;
            var cost = batch.PreviewManaCost();
            var sign = cost >= 0 ? '-' : '+';
            var costStr = batch.Cards.Length == 0 ? "" : $"({sign}{Mathf.Abs(cost):F0})";
            return $"{state.Mana:F0}{costStr}/{state.ManaMax}";
        };
        mana.tooltip.OnShow            = () => "魔法值";
        physicalShield.OnShow          = () => $"{state.PhysicalShield:F0}";
        physicalShield.tooltip.OnShow  = () => "物理护盾";
        physicalAmplify.OnShow         = () => $"{state.PhysicalDamageAmplify}%";
        physicalAmplify.tooltip.OnShow = () => "物理伤害增幅";
        physicalReduce.OnShow          = () => $"{state.PhysicalDamageReduce}%";
        physicalReduce.tooltip.OnShow  = () => "物理伤害减免";
        magicShield.OnShow             = () => $"{state.MagicShield:F0}";
        magicShield.tooltip.OnShow     = () => "魔法护盾";
        magicAmplify.OnShow            = () => $"{state.MagicDamageAmplify}%";
        magicAmplify.tooltip.OnShow    = () => "魔法伤害增幅";
        magicReduce.OnShow             = () => $"{state.MagicDamageReduce}%";
        magicReduce.tooltip.OnShow     = () => "魔法伤害减免";

        elementSeals.ForEach((element, i) =>
        {
            var type = (ElementType)i;
            // Todo 显示破碎效果
            element.OnShow         = () => $"{state.ElementAttach[type]}";
            element.tooltip.OnShow = () => $"{type.ToDescription()}元素法印";
        });
    }
}
}
