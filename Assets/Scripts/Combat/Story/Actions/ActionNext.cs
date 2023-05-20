using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Combat.Story.Actions {
/*
@next // 结束回合
 */
public class ActionNext : StoryAction {
    public override StoryAction Build(IReadOnlyList<string> args) {
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        if (combatant.isOtherPlayer) {
            combatant.EndTurn();
        }
        else {
            yield return combatant.Judge.help.Show($"请结束本回合。");
            // 清除结束按钮上的事件
            var playController = controller as PlayerController;
            Debug.Assert(playController != null, nameof(playController) + " != null");

            playController.endTurnButton.onClick.RemoveAllListeners();
            playController.endTurnButton.interactable = true;
            bool clickedEndTurnButton = false;
            playController.endTurnButton.onClick.AddListener(() => {
                clickedEndTurnButton = true;
            });
            

            // 当玩家操作合法时，触发回合结束事件
            var validEndOperation = new Func<bool>(() => {
                if (clickedEndTurnButton || Input.GetKeyDown(KeyCode.Q)) {
                    return true;
                }
                clickedEndTurnButton = false;
                return false;
            });
            while (!validEndOperation()) yield return null;

            // 当需要弃牌时，处理弃牌请求
            if (combatant.Cards.Count > combatant.State.MaxCardCnt) {
                yield return combatant.Judge.help.Show($"玩家当前的最大卡牌数为{combatant.State.MaxCardCnt}, 请丢弃适当数量的卡牌后再结束回合。");
                playController.commitButton.onClick.RemoveAllListeners();
                playController.commitButton.interactable = true;
                bool clickedCommitButton = false;
                playController.commitButton.onClick.AddListener(() => {
                    clickedCommitButton = true;
                });

                var validDiscardOperation = new Func<bool>(() => {
                    if (clickedCommitButton || Input.GetKeyDown(KeyCode.Space)) {
                        return true;
                    }
                    clickedCommitButton = false;
                    return false;
                });

                combatant.Controller.isDiscardStage = true;
                while (combatant.Cards.Count > combatant.State.MaxCardCnt) {
                    clickedCommitButton = false;
                    while (!validDiscardOperation()) yield return null;
                    combatant.Discard();
                }
                
                clickedEndTurnButton = false;
                while (!validEndOperation()) yield return null;
            }
            combatant.EndTurn();
            
            // 恢复按钮功能
            playController.endTurnButton.interactable = false;
            playController.endTurnButton.onClick.RemoveAllListeners();
            playController.endTurnButton.onClick.AddListener(playController.EndTurnAction);
            playController.commitButton.interactable = false;
            playController.commitButton.onClick.RemoveAllListeners();
            playController.commitButton.onClick.AddListener(playController.CommitAction);
        }
    }

    protected override string ToDescription() {
        return "结束回合";
    }
}
}
