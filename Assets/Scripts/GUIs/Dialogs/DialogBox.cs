using DG.Tweening;
namespace GUIs.Dialogs
{
    public class DialogBox : TextBox
    {
        public float showTime = 0.5f;
        public override void SetText(string content)
        {
            text.text = "";
            text.DOText(content, showTime);
        }
    }
}