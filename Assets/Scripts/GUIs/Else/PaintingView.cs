using Combat.Enums;
using Combat.States;
using UnityEngine;
using UnityEngine.UI;

namespace GUIs.Else {
// 立绘视图
public class PaintingView : MonoBehaviour {
#region prefab配置

    [SerializeField]
    private Image back;

    [SerializeField]
    private Image image;

#endregion

    public void FreshUI(CombatState state) {
        back.color = state.SpriteColor.MainColor();
        image.sprite = Resources.Load<Sprite>(state.SpritePath);
    }
}
}
