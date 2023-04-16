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

    // Todo 改成响应式更新
    private void Update() {
        FreshUI();
    }

    private void FreshUI() {
        health.FreshUI();
        mana.FreshUI();
        physicalShield.FreshUI();
        physicalAmplify.FreshUI();
        physicalReduce.FreshUI();
        magicShield.FreshUI();
        magicAmplify.FreshUI();
        magicReduce.FreshUI();
    }

    private void Start() {
        var state = combatant.State;
        health.OnShow         = () => $"{state.Health:F0}/{state.HealthMax}";
        health.tooltip.OnShow = () => "生命值";
        mana.OnShow = () =>
        {
            var cost = combatant.PreviewBatch.PreviewManaCost();
            var costStr = cost <= 0 ? "" : $"(-{cost:F0})";
            return $"{state.Mana:F0}{costStr}/{state.ManaMax}";
        };
        mana.tooltip.OnShow            = () => "魔法值";
        physicalShield.OnShow          = () => $"{state.PhysicalShield}";
        physicalShield.tooltip.OnShow  = () => "物理护盾";
        physicalAmplify.OnShow         = () => $"{state.PhysicalDamageAmplify}%";
        physicalAmplify.tooltip.OnShow = () => "物理伤害增幅";
        physicalReduce.OnShow          = () => $"{state.PhysicalDamageReduce}%";
        physicalReduce.tooltip.OnShow  = () => "物理伤害减免";
        magicShield.OnShow             = () => $"{state.MagicShield}";
        magicShield.tooltip.OnShow     = () => "魔法护盾";
        magicAmplify.OnShow            = () => $"{state.MagicDamageAmplify}%";
        magicAmplify.tooltip.OnShow    = () => "魔法伤害增幅";
        magicReduce.OnShow             = () => $"{state.MagicDamageReduce}%";
        magicReduce.tooltip.OnShow     = () => "魔法伤害减免";

        // Todo 显示Armor

        elementSeals.ForEach((element, i) =>
        {
            var type = (ElementType)i;
            element.OnShow         = () => $"{state.ElementAttach[type]}";
            element.tooltip.OnShow = () => $"{type.ToDescription()}元素法印";
        });
    }
}
}
