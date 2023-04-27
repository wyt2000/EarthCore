using TMPro;
using UnityEngine;

namespace GUIs.Menus {
// Todo! 完善教程框
public class HelpView : MonoBehaviour {
#region prefab配置

    [SerializeField]
    private TextMeshProUGUI text;

#endregion

    public void Show(string msg) {
        text.text = msg;
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
}
