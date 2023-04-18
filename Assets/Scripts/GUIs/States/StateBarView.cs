using System.Collections;
using System.Linq;
using Combat;
using Combat.Enums;
using Combat.States;
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

    private float m_duration = 1.0f;

    public void FreshSync() {
        var old = m_duration;
        m_duration = 0;
        FreshUI(combatant.State, combatant.State, new CombatState());
        m_duration = old;
    }

    public void Init() {
        BindAll();
        FreshSync();
        combatant.State.OnStateChange += FreshUI;
    }

    private void FreshUI(CombatState old, CombatState cur, CombatState delta) {
        StartCoroutine(ImpFreshUI(old, cur, delta));
    }

    private IEnumerator ImpFreshUI(CombatState old, CombatState cur, CombatState delta) {
        // Todo 加锁
        return GCoroutine.Parallel(
            health.FreshHealthBar(m_duration, old.Health, cur.Health, old.HealthMax, cur.HealthMax),
            mana.FreshManaBar(m_duration, old.Mana, cur.Mana, old.ManaMax, cur.ManaMax, combatant),
            physicalShield.FreshField(m_duration, old.PhysicalShield, cur.PhysicalShield),
            physicalAmplify.FreshField(m_duration, old.PhysicalDamageAmplify, cur.PhysicalDamageAmplify),
            physicalReduce.FreshField(m_duration, old.PhysicalDamageReduce, cur.PhysicalDamageReduce),
            magicShield.FreshField(m_duration, old.MagicShield, cur.MagicShield),
            magicAmplify.FreshField(m_duration, old.MagicDamageAmplify, cur.MagicDamageAmplify),
            magicReduce.FreshField(m_duration, old.MagicDamageReduce, cur.MagicDamageReduce),
            GCoroutine.Parallel(elementSeals.Select((element, i) =>
            {
                var type = (ElementType)i;
                return element.FreshSeal(m_duration, old.ElementAttach[type], cur.ElementAttach[type]);
            }))
        );
    }

    private void BindAll() {
        health.tooltip.OnShow          = () => "生命值";
        mana.tooltip.OnShow            = () => "魔法值";
        physicalShield.tooltip.OnShow  = () => "物理护盾";
        physicalAmplify.tooltip.OnShow = () => "物理伤害增幅";
        physicalReduce.tooltip.OnShow  = () => "物理伤害减免";
        magicShield.tooltip.OnShow     = () => "魔法护盾";
        magicAmplify.tooltip.OnShow    = () => "魔法伤害增幅";
        magicReduce.tooltip.OnShow     = () => "魔法伤害减免";

        elementSeals.ForEach((element, i) =>
        {
            var type = (ElementType)i;
            // Todo 显示破碎效果
            // element.OnShow         = state => $"{state.ElementAttach[type]}";
            element.tooltip.OnShow = () => $"{type.ToDescription()}元素法印";
        });
    }
}
}
