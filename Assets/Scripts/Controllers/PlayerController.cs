using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Controllers {
public class PlayerController : CombatController {
    public Button commitButton;
    public Button endTurnButton;

    public UnityAction CommitAction;
    public UnityAction EndTurnAction;

    public void EnableButtons() {
        commitButton.interactable = true;
        endTurnButton.interactable = true;
    }

    public void DisableButtons() {
        commitButton.interactable = false;
        endTurnButton.interactable = false;
    }

    // 用户输入
    public override IEnumerator OnUserInput() {
    #if UNITY_EDITOR
        EnableButtons();
        // GM摸牌
        if (Input.GetKeyDown(KeyCode.G)) {
            combatant.GetCard(1);
        }

    #endif
        // 出牌
        if (Input.GetKeyDown(KeyCode.Space)) {
            CommitAction();
        }
        // 回合结束
        if (Input.GetKeyDown(KeyCode.Q)) {
            EndTurnAction();
        }
        return null;
    }
    
    public override IEnumerator OnDoAction() {
        var current = combatant.Judge.Script?.Current;
        var iter = current?.Execute(this);
        if (current != null) {
            DisableButtons();
            yield return iter;
            combatant.Judge.Script?.Finish();
        } else {
            yield return OnUserInput();
        }
    }

    
    private void Start() {
        CommitAction = () => {
            if (isDiscardStage) {
                combatant.Discard();
            }
            else {
                combatant.PlaySelectedCard();
            }
        };
        EndTurnAction = () => {
            combatant.EndTurn();
        };
        commitButton.onClick.AddListener(CommitAction);
        endTurnButton.onClick.AddListener(EndTurnAction);
    }

}
}
