using Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers {
public class PlayerButton : MonoBehaviour {
    public Button play;
    public Button discard;

    public CombatantComponent combatant;

    private void Start() {
        play.onClick.AddListener(() => combatant.PlaySelectedCard());
        discard.onClick.AddListener(() => combatant.Discard());
    }
}
}
