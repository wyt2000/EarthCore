using System.Collections;
using System.Collections.Generic;
using Controllers;

namespace Combat.Story.Actions {
/*
// 显示位置指定立绘.自动加载Textures/{image_name}下的图片
@show {image_name} {"left"|"right"}
 */
public class ActionShow : StoryAction {
    private string m_imageName;
    private string m_side;

    public override StoryAction Build(IReadOnlyList<string> args) {
        m_imageName = args[0];
        m_side      = args[1];
        return this;
    }

    public override IEnumerator Execute(CombatController controller) {
        var combatant = controller.combatant;
        var dialog = combatant.Judge.dialog;
        return dialog.Show(m_imageName, m_side);
    }

    protected override string ToDescription() {
        return $"显示立绘:{m_imageName}";
    }
}
}
