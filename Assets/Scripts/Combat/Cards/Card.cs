using System;
using Combat.Requests;

namespace Combat.Cards {
// 卡牌 基类
public class Card {
#region 元信息

    // 卡牌持有者
    public CombatantComponent Owner;

#endregion

#region UI配置

    // 卡牌名称
    public string UiName = "Card";

    // 卡牌描述
    public string UiDescription = "Card";

    // 卡牌图片路径 Todo 改成在编辑器里选材质
    public string UiImagePath = "";

#endregion

#region 逻辑配置

    // 法力消耗
    public float LgManaCost = 0;

    // 动态法力消耗
    public Func<Card, float> LgManaCostFunc = null;

    // 卡牌本身的元素类型(仅用于触发元素联动,不一定造成伤害)
    public ElementType? LgElement = null;

#endregion

#region 事件配置

    // 卡牌效果
    public Action<PlayCardRequest> OnPlay = delegate { };

#endregion

#region 公开函数

    public float GetManaCost() {
        return LgManaCostFunc?.Invoke(this) ?? LgManaCost;
    }

    // 出牌的逻辑操作
    public void PlayCard(PlayCardRequest request) {
        OnPlay(request);
    }

#endregion
}
}
