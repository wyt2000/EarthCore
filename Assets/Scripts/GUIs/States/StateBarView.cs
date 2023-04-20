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

    private const float Duration = 0.5f;

    private readonly CoroutineLocker m_locker = new(ResolvePolicy.Delay);

    public void FreshSync() {
        var iter = ImpFreshUI(0, combatant.State, combatant.State);
        while (iter.MoveNext()) { }
    }

    public void Init() {
        BindAll();
        FreshSync();
        combatant.State.OnStateChange += (old, cur, _) => FreshUI(Duration, old, cur);
    }

    private void FreshUI(float duration, CombatState old, CombatState cur) {
        StartCoroutine(ImpFreshUI(duration, old, cur).Lock(m_locker));
    }

    private IEnumerator ImpFreshUI(float duration, CombatState old, CombatState cur) {
        string Format(float  f) => f >= 0 ? $"+{f:F0}" : $"{f:F0}";
        string Percent(float f) => f >= 0 ? $"+{f:F0}%" : $"{f:F0}%";
        return GCoroutine.Parallel(
            health.FreshHealthBar(duration, old.Health, cur.Health, old.HealthMax, cur.HealthMax),
            mana.FreshManaBar(duration, old.Mana, cur.Mana, old.ManaMax, cur.ManaMax, combatant),
            physicalShield.FreshField(duration, old.PhysicalShield, cur.PhysicalShield, Format),
            physicalAmplify.FreshField(duration, old.PhysicalDamageAmplify, cur.PhysicalDamageAmplify, Percent),
            physicalReduce.FreshField(duration, old.PhysicalDamageReduce, cur.PhysicalDamageReduce, Percent),
            magicShield.FreshField(duration, old.MagicShield, cur.MagicShield, Format),
            magicAmplify.FreshField(duration, old.MagicDamageAmplify, cur.MagicDamageAmplify, Percent),
            magicReduce.FreshField(duration, old.MagicDamageReduce, cur.MagicDamageReduce, Percent),
            GCoroutine.Parallel(elementSeals.Select((element, i) =>
            {
                var type = (ElementType)i;
                return element.FreshSeal(duration, old.ElementAttach[type], cur.ElementAttach[type]);
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
