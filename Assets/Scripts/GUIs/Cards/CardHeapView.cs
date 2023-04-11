using Combat;
using TMPro;
using UnityEngine;

namespace GUIs.Cards {
// 牌堆视图
public class CardHeapView : MonoBehaviour {
#region prefab配置

    public CombatantComponent combatant;

    [SerializeField]
    private TextMeshProUGUI text;

#endregion

    public void Update() {
        var cnt = combatant.Heap.AllCards.Count;
        var discard = combatant.Heap.DiscardCount;
        text.text = $"剩余/弃牌:{cnt}/{discard}";
    }
}
}
