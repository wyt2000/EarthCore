using System;
using System.Collections.Generic;
using Combat.Requests.Details;

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

    // 效果创建者
    public CombatantComponent Causer;

    // 附着的目标
    public CombatantComponent Target;

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
    public string UiName = "_None_";

    // 效果描述(鼠标悬浮显示)
    public string UiDescription = "_None_";

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
    - 计算法力消耗后
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
    protected virtual void OnAfterDetach() { }

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
    protected virtual void OnBeforeTakeHpChange(RequestHpChange request) { }

    /// <summary>
    /// 自己的生命修改前,预处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">修改请求</param>
    /// <returns>返回是否拒绝请求</returns>
    protected virtual bool OnBeforeSelfHpChange(RequestHpChange request) {
        return false;
    }

    /// <summary>
    /// 自己的生命修改后,后处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">修改请求</param>
    protected virtual void OnAfterSelfHpChange(RequestHpChange request) { }

    /// <summary>
    /// 造成别人的生命修改后,后处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">伤害请求</param>
    protected virtual void OnAfterTakeHpChange(RequestHpChange request) { }

    /// <summary>
    /// 后处理法力消耗.已有的所有效果都触发
    /// </summary>
    /// <param name="request">出牌请求</param>
    protected virtual void OnBeforePreviewMana(RequestPlayBatchCard request) { }

    /// <summary>
    /// 后处理法力消耗.已有的所有效果都触发
    /// </summary>
    /// <param name="request">出牌请求</param>
    protected virtual void OnAfterPreviewMana(RequestPlayBatchCard request) { }

    /// <summary>
    /// 出牌前调用.已有的所有效果都触发,(多张牌也只调用一次)
    /// </summary>
    /// <param name="request">出牌请求</param>
    protected virtual void OnBeforePlayBatchCard(RequestPlayBatchCard request) { }

    /// <summary>
    /// 出牌后调用.已有的所有效果都触发,(多张牌也只调用一次)
    /// </summary>
    /// <param name="request">出牌请求</param>
    protected virtual void OnAfterPlayBatchCard(RequestPlayBatchCard request) { }

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
        Target.Judge.Requests.Add(new RequestEffect {
            Causer = Causer,
            Effect = this,
            Attach = true,
        });
    }

    public void Remove() {
        Target.Judge.Requests.Add(new RequestEffect {
            Causer = Causer,
            Effect = this,
            Attach = false,
        });
    }

    public void DoAttach() {
        var reject = Target.BoardCastAny(effect => effect.BeforeAttach(this));
        if (reject) return;
        m_effectStamp = ms_stamp++;
        Target.Effects.Add(this);
        AfterAttach();
    }

    public void DoRemove() {
        Target.Effects.Remove(this);
        AfterDetach();
    }

#endregion

#region 统一事件接口

    private bool BeforeAttach(Effect effect) {
        return OnBeforeAttach(effect);
    }

    private void AfterAttach() {
        OnAfterAttach();
    }

    private void AfterDetach() {
        OnAfterDetach();
    }

    public void BeforeTurnStart() {
        OnBeforeTurnStart();
    }

    public void AfterTurnEnd() {
        OnAfterTurnEnd();
        if (--LgRemainingRounds == 0) Remove();
    }

    public void BeforeTakeHpChange(RequestHpChange request) {
        OnBeforeTakeHpChange(request);
    }

    public bool BeforeSelfHpChange(RequestHpChange request) {
        return OnBeforeSelfHpChange(request);
    }

    public void AfterSelfHpChange(RequestHpChange request) {
        OnAfterSelfHpChange(request);
    }

    public void AfterTakeHpChange(RequestHpChange request) {
        OnAfterTakeHpChange(request);
    }

    public void BeforePreviewMana(RequestPlayBatchCard request) {
        OnBeforePreviewMana(request);
    }

    public void AfterPreviewMana(RequestPlayBatchCard request) {
        OnAfterPreviewMana(request);
    }

    public void BeforePlayBatchCard(RequestPlayBatchCard request) {
        OnBeforePlayBatchCard(request);
    }

    public void AfterPlayBatchCard(RequestPlayBatchCard request) {
        OnAfterPlayBatchCard(request);
    }

#endregion
}
}
