using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Cards;
using Controllers;
using GUIs.Animations;
using UnityEngine;
using Utils;

namespace Combat.Story.Actions {
/*
@play {card_name,}* // 出指定的牌
 */
public class ActionPlay : StoryAction {
    private string[] m_cards;

    private StoryAction Build(string[] cards) {
        m_cards = cards;
        return this;
    }

    public override StoryAction Build(IReadOnlyList<string> args) {
        return Build(args[0].Split(','));
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        var needs = new HashSet<Card>();

        if (!combatant.isOtherPlayer) {
            yield return combatant.Judge.help.Show($"请出牌:{string.Join(",", m_cards)}");
        }
        m_cards.ForEach(c =>
        {
            var need = combatant.Cards.Find(cd => cd.UiName == c && !needs.Contains(cd));
            if (need != null) needs.Add(need);
        });
        if (combatant.isOtherPlayer) {
            yield return GAnimation.Wait(1.0f);
            combatant.PlayCard(needs);
        }
        else {
            while (!(Input.GetKeyDown(KeyCode.Space) && combatant.Cards.All(c => c.IsSelected == needs.Contains(c)))) yield return null;
            combatant.PlayCard(needs);
        }
    }

    protected override string ToDescription() {
        return $"出牌:{string.Join(",", m_cards)}";
    }
}
}
