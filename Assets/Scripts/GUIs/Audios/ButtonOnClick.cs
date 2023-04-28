using System;
using UnityEngine;
using UnityEngine.UI;

namespace GUIs.Audios {
public class ButtonOnClick : MonoBehaviour {
    private void Start() {
        var button = GetComponent<Button>();
        button.onClick?.AddListener(GAudio.PlayButtonClick);
    }
}
}
