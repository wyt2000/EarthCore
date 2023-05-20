using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Cards;
using Controllers;
using GUIs.Animations;
using UnityEngine;
using Utils;
using Debug = System.Diagnostics.Debug;

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
            // 清除出牌按钮上绑定的出牌事件
            var playController = controller as PlayerController;
            Debug.Assert(playController != null, nameof(playController) + " != null");
            playController.commitButton.onClick.RemoveAllListeners();
            playController.commitButton.interactable = true;

            bool clickedCommitButton = false;
            playController.commitButton.onClick.AddListener(() => {
                clickedCommitButton = true;
            });
            
            // 当玩家操作合法时，触发出牌事件
            var validOperation = new Func<bool>(() => {
                if (combatant.Cards.All(c => c.IsSelected == needs.Contains(c)))
                {
                    if (clickedCommitButton || Input.GetKeyDown(KeyCode.Space)) {
                        return true;
                    }
                }
                clickedCommitButton = false;
                return false;
            });
            while (!validOperation()) yield return null;
            combatant.PlayCard(needs);
            
            // 恢复出牌按钮功能
            playController.commitButton.interactable = false;
            playController.commitButton.onClick.RemoveAllListeners();
            playController.commitButton.onClick.AddListener(playController.CommitAction);
        }
    }

    protected override string ToDescription() {
        return $"出牌:{string.Join(",", m_cards)}";
    }
}
}
