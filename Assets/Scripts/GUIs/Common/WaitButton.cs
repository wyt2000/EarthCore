using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUIs.Common {
public class WaitButton : MonoBehaviour {
#region prefab配置

    [SerializeField]
    private Button button;

    [SerializeField]
    private GameObject wait;

    [SerializeField]
    private bool isWait;

#endregion

    private void FreshUI() {
        button.interactable = !isWait;
        wait.SetActive(isWait);
    }

    private void Start() {
        FreshUI();
    }

    private void OnDrawGizmos() {
        FreshUI();
    }
}
}
