using UnityEngine;

namespace GUIs.Audios {
public class ButtonOnClick : MonoBehaviour {
    public void OnClick() {
        GAudio.PlayButtonClick();
    }
}
}
