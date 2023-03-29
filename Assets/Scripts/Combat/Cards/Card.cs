using System;
using Combat.Effects;
using Combat.Requests;
using Utils;

namespace Combat.Cards {
// 卡牌 基类 Todo
public class Card {
#region UI配置

    // 卡牌名称
    public string Name = "Card";

    // 卡牌描述
    public string Description = "Card";

    // 卡牌图片路径 Todo 改成在编辑器里选材质
    public string ImagePath = "";

#endregion

#region 逻辑配置

    // 法力消耗
    public float ManaCost = 0;

    // 动态法力消耗
    public Func<PlayCardRequest, float> ManaCostFunc = null;

    // 卡牌本身的元素类型(仅用于触发元素联动,不一定造成伤害)
    public ElementType? Element = null;

    // 卡牌效果
    public Action<PlayCardRequest> OnPlay = delegate { };

#endregion

#region 公开函数

    public float GetManaCost(PlayCardRequest request) {
        return ManaCostFunc?.Invoke(request) ?? ManaCost;
    }


    // 出牌的逻辑操作
    public void PlayCard(PlayCardRequest request) {
        request.Causer.BoardCast(e => e.BeforePlayCard(this));
        OnPlay(request);
        request.Causer.BoardCast(e => e.AfterPlayCard(this));
    }

#endregion
}
}
