﻿using System;
using System.Collections.Generic;
using Combat.Requests;

namespace Combat.Effects {
// 增益,减益,控制等.用于过滤某些tag
public enum EffectTag {
    Buff,
    DeBuff,
    Control,
}

// 效果 基类
public class Effect : IComparable<Effect> {
#region 私有字段

    private static int ms_stamp;

    private int m_effectStamp;

#endregion

#region 元信息

    // 附着的目标
    public CombatantComponent Target;

    // 辅助实现复合效果
    public Effect Parent;

#endregion

#region 配置项

    // 配置项命名规范:
    // UI: Ui
    // 逻辑: Lg
    // 事件: On
    // 若子类新增,则按U1,U2,L1,L2,On1,On2等格式命名,数字取决于继承层级
    // 所有配置项均为public,方便初始化

#region UI配置

    // 效果名
    public string UiName = "效果";

    // 效果描述(鼠标悬浮显示)
    public string UiDescription = "效果描述";

    // 效果图标路径
    public string UiIconPath = "";

    // 是否隐藏UI显示(例如立即伤害那种就不需要)
    public bool UiHidde = false;

    // Todo 完善UI/动画/粒子效果等配置

#endregion

#region 逻辑配置

    // Todo 完善逻辑相关配置

    // effect标签
    public ISet<EffectTag> LgTags = new HashSet<EffectTag>();

    // 剩余持续回合,为0时表示不限制回合次数
    public int LgRemainingRounds;

    // 最大叠加层数
    public int LgMaxOverlay = 1;

    // 叠加层数
    public int LgOverlay = 1;

    // 结算优先级(越小越优先)
    public int LgPriority = 0;

#endregion

#region 事件接口

    // Todo 调用&重写事件接口

    /*
    - 效果附着前
    - Todo 判断是否能合并
    - Todo 决定如何合并
    - 效果附着后
    - 效果消失后
    - 回合开始前
    - 回合结束后
    - 造成伤害前
    - 受到伤害前
    - 受到伤害后
    - 造成伤害后
    - 出牌前
    - 出牌后
     */

    /// <summary>
    /// 效果附着前调用.已有的所有效果都触发
    /// </summary> 
    /// <param name="effect">即将附着的效果</param>
    /// <returns>返回true表示拒绝附着,任意一个effect拒绝即可</returns>
    /// <example>
    /// 净化buff,免疫负面效果;
    /// 可合并buff
    /// </example>
    protected virtual bool OnBeforeAttach(Effect effect) {
        return false;
    }

    /// <summary>
    /// 效果附着后调用.只给自身触发
    /// </summary>
    protected virtual void OnAfterAttach() { }

    /// <summary>
    /// 效果消失后调用.只给自身触发
    /// </summary>
    protected virtual void OnLeaveAttach() { }

    /// <summary>
    /// 自己回合开始前调用.只给自身触发
    /// </summary>
    protected virtual void OnBeforeTurnStart() { }

    /// <summary>
    /// 自己回合结束后调用.只给自身触发
    /// </summary>
    protected virtual void OnAfterTurnEnd() { }

    /// <summary>
    /// 造成别人的生命修改前,预处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">伤害请求</param>
    /// <example>
    /// 护甲buff
    /// </example>
    protected virtual void OnBeforeTakeHpChange(HealthRequest request) { }

    /// <summary>
    /// 自己的生命修改前,预处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">修改请求</param>
    /// <returns>返回是否拒绝请求</returns>
    protected virtual bool OnBeforeSelfHpChange(HealthRequest request) {
        return false;
    }

    /// <summary>
    /// 自己的生命修改后,后处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">修改请求</param>
    protected virtual void OnAfterSelfHpChange(HealthRequest request) { }

    /// <summary>
    /// 造成别人的生命修改后,后处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">伤害请求</param>
    protected virtual void OnAfterTakeHpChange(HealthRequest request) { }

    /// <summary>
    /// 出牌前调用.已有的所有效果都触发
    /// </summary>
    /// <param name="request">出牌请求</param>
    protected virtual void OnBeforePlayCard(PlayCardRequest request) { }

    /// <summary>
    /// 出牌后调用.已有的所有效果都触发
    /// </summary>
    /// <param name="request">出牌请求</param>
    protected virtual void OnAfterPlayCard(PlayCardRequest request) { }

#endregion

#endregion

#region 公开函数

    public int CompareTo(Effect other) {
        if (LgPriority == other.LgPriority) {
            return m_effectStamp - other.m_effectStamp;
        }

        return LgPriority - other.LgPriority;
    }

    public void Attach(CombatantComponent target) {
        Target = target;
        Target.Judge.AddEffectTask(new EffectRequest {
            Effect = this,
            Attach = true,
        });
    }

    public void Remove() {
        Target.Judge.AddEffectTask(new EffectRequest {
            Effect = this,
            Attach = false,
        });
    }

    public void DoAttach() {
        var reject = Target.BoardCastAny(effect => effect.OnBeforeAttach(this));
        if (reject) return;
        m_effectStamp = ms_stamp++;
        Target.Effects.Add(this);
        OnAfterAttach();
    }

    public void DoRemove() {
        OnLeaveAttach();
        var cur = this;
        while (cur.Parent != null)
            cur = cur.Parent;
        Target.Effects.Remove(cur);
    }

#endregion

#region 公开事件

    public bool BeforeAttach(Effect effect) {
        return OnBeforeAttach(effect);
    }

    public void AfterAttach() {
        OnAfterAttach();
    }

    public void LeaveAttach() {
        OnLeaveAttach();
    }

    public void BeforeTurnStart() {
        OnBeforeTurnStart();
    }

    public void AfterTurnEnd() {
        OnAfterTurnEnd();
        if (--LgRemainingRounds == 0) Remove();
    }

    public void BeforeTakeHpChange(HealthRequest request) {
        OnBeforeTakeHpChange(request);
    }

    public bool BeforeSelfHpChange(HealthRequest request) {
        return OnBeforeSelfHpChange(request);
    }

    public void AfterSelfHpChange(HealthRequest request) {
        OnAfterSelfHpChange(request);
    }

    public void AfterTakeHpChange(HealthRequest request) {
        OnAfterTakeHpChange(request);
    }

    public void BeforePlayCard(PlayCardRequest request) {
        OnBeforePlayCard(request);
    }

    public void AfterPlayCard(PlayCardRequest request) {
        OnAfterPlayCard(request);
    }

#endregion
}
}
