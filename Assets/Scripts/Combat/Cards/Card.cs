﻿using System;
using Combat.Enums;
using Combat.Requests.Details;

namespace Combat.Cards {
// 卡牌 基类
public class Card {
#region 元信息

    // 卡牌持有者
    public CombatantComponent Owner;

    // 卡牌是否被选中
    public bool IsSelected = false;

    // 卡牌是否可以被选中
    public bool IsSelectable = true;

    // 克隆原始卡牌
    public Func<Card> Clone;

#endregion

#region UI配置

    // 卡牌名称
    public string UiName = "_None_";

    // 卡牌描述
    public string UiDescription = "_None_";

    // 卡牌图片路径 Todo 改成在编辑器里选材质
    public string UiImagePath = "";

#endregion

#region 逻辑配置

    // 法力消耗
    public float LgManaCost;

    // 动态法力消耗
    public Func<Card, float> LgManaCostFunc = null;

    // 卡牌伤害
    public float LgDamage;

    // 动态卡牌伤害
    public Func<Card, float> LgDamageFunc = null;

    // 卡牌本身的元素类型(仅用于触发元素联动,不一定造成伤害)
    public ElementType? LgElement = null;

    // 卡牌是否有浸染效果(染色)
    public bool LgInfect = false;

    // 卡牌是否有唯一(不能搭配其他牌)效果 Todo 处理唯一效果
    public bool LgUnique = false;

#endregion

#region 事件配置

    // 前处理法力消耗,如元素浸染
    public Action<RequestPlayBatchCard> OnBeforePreviewMana = delegate { };

    // 后处理法力消耗,如本次法力消耗减半效果
    public Action<RequestPlayBatchCard> OnAfterPreviewMana = delegate { };

    // 计算元素联动前,如浸染效果
    public Action<RequestPlayBatchCard> OnBeforePlayBatchCard = delegate { };

    // 卡牌效果
    public Action<RequestPlayCard> OnExecute = delegate { };

    // 出牌后
    public Action<RequestPlayBatchCard> OnAfterPlayBatchCard = delegate { };

#endregion

#region 公开函数

    public float ManaCost => LgManaCostFunc?.Invoke(this) ?? LgManaCost;

    private float Damage => LgDamageFunc?.Invoke(this) ?? LgDamage;

    public void ConfirmValue() {
        LgManaCost = ManaCost;
        LgDamage   = Damage;
    }

    // 快捷造成伤害
    public void TakeDamage(RequestPlayCard request) {
        request.Causer.Attack(request.Batch.Target, new RequestHpChange {
            Value   = Damage,
            Type    = DamageType.Physical,
            Element = LgElement,
        });
    }

#endregion
}
}
