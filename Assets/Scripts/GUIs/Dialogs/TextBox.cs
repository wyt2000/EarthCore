using TMPro;
using UnityEngine;

namespace GUIs.Dialogs {
public abstract class TextBox : MonoBehaviour {
    public TextMeshProUGUI text;

    public abstract void SetText(string content);
}
}
