using UnityEngine;
using UnityEngine.EventSystems;

namespace GUIs.Dialogs {
public class DialogTrigger : MonoBehaviour, IPointerClickHandler {
    public DialogController dialogController;

    public void OnPointerClick(PointerEventData eventData) {
        dialogController.Next();
    }
}
}
