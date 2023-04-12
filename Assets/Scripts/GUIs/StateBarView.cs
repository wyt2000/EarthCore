using System;
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
        var effects = combatant.Effects;
        var cards = combatant.Cards;
        var str = $"{combatant.name} status : \n";
        str += $"生命值 : {state.Health:F0}/{state.HealthMax} \n";
        str += $"魔法值 : {state.Mana:F0}/{state.ManaMax} \n";
        str += $"物理护盾 : {state.PhysicalShield}\n";
        str += $"物理伤害加成 : {state.PhysicalDamageAmplify}\n";
        str += $"物理伤害减免 : {state.PhysicalDamageReduce}\n";
        str += $"魔法护盾 : {state.MagicShield}\n";
        str += $"魔法伤害加成 : {state.MagicDamageAmplify}\n";
        str += $"魔法伤害减免 : {state.MagicDamageReduce}\n";

        // str += $"当前效果 x{effects.Count}: \n";
        // str += string.Join("",
        //     effects
        //        .Where(e => !e.UiHidde)
        //        .Select(e => $" - {e.UiName} , 剩余回合 : {e.LgRemainingRounds} \n"));
        //
        // str += "当前手牌 : \n";
        // for (int i = 0, n = cards.Count; i < n; ++i) {
        //     var c = cards[i];
        //     var select = c.IsSelected ? 'x' : ' ';
        //     str += $" - {select} [{c.LgElement?.ToDescription() ?? "无"}][{i + 1}/{n}]{c.UiName}({c.ManaCost}) : {c.UiDescription} \n";
        // }

        text.text = str;
    }
}
}
