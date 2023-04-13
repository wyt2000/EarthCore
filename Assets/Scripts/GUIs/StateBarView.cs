using System.Linq;
using Combat;
using TMPro;
using UnityEngine;
using Utils;

namespace GUIs {
// Todo 显示元素附着状态和其他状态
public class StateBarView : MonoBehaviour {
#region prefab配置

    public CombatantComponent combatant;

    [SerializeField]
    private TextMeshProUGUI text;

#endregion

    private void Update() {
        var state = combatant.State;
        // {combatant.name} status : 
        var element = string.Join(",", state.ElementAttach.Select(item => $"{item.Key.ToDescription()}x{item.Value}"));
        var str = $"元素:{element}\n";
        str += $"生命值 : {state.Health:F0}/{state.HealthMax} \n";
        str += $"魔法值 : {state.Mana:F0}/{state.ManaMax} \n";
        str += $"护甲 : {state.PhysicalArmor}\n";
        str += $"物理护盾 : {state.PhysicalShield}\n";
        str += $"物理伤害加成 : {state.PhysicalDamageAmplify}\n";
        str += $"物理伤害减免 : {state.PhysicalDamageReduce}\n";
        str += $"魔法护盾 : {state.MagicShield}\n";
        str += $"魔法伤害加成 : {state.MagicDamageAmplify}\n";
        str += $"魔法伤害减免 : {state.MagicDamageReduce}\n";

        text.text = str;
    }
}
}
