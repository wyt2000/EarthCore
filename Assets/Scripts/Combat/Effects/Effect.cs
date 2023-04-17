using System;
using System.Collections.Generic;
using Combat.Requests.Details;
using Combat.States;
using GUIs.Effects;
using UnityEngine;

namespace Combat.Effects {
// 增益,减益,控制等.用于过滤某些tag
public enum EffectTag {
    // 增益类buff
    Buff,

    // 减益类buff
    DeBuff,

    // 控制类buff
    Control,

    // 固有buff,不可移除
    Fixed,
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
    // 特殊配置项: OnImp , 用于匿名类的虚函数追加

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

    // effect标签
    public ISet<EffectTag> LgTags = new HashSet<EffectTag>();

    // 剩余持续回合,为0时表示不限制回合次数
    public int LgRemainingRounds;

    // 最大叠加层数
    public int LgMaxOverlay = int.MaxValue;

    // 叠加层数
    public int LgOverlay = 1;

    // 是否开启合并功能(开启合并的功能必须填写Ui字段)
    public bool LgOpenMerge = false;

    // 结算优先级(越小越优先)
    public int LgPriority = 0;

#endregion

#region 事件接口

    /*
    - 效果附着前(免疫负面效果)
    - 判断是否能合并(默认同类型才能合并)
    - 决定如何合并(一般是叠加层数,重置持续回合)
    - 效果附着后(施加增益效果)
    - 效果消失后(移除增益效果)
    - 回合开始前(中毒buff)
    - 回合结束后(减去持续回合)
    - 造成伤害前(伤害增幅)
    - 受到伤害前(免疫伤害/减伤)
    - 受到伤害后(反弹伤害)
    - 造成伤害后(吸血)
    - 计算法力消耗后(法力消耗减半)
    - 出牌前(如 不竭:本次出牌法力消耗减半,抽一张牌)
    - 出牌后(如 生命汲取:本次出牌造成伤害的50%转化为生命)
    - 状态修改后(如 增加物理护盾)
    */

    /// <summary>
    /// 有新效果附着前调用,判定是否拒绝附着
    /// </summary> 
    /// <param name="effect">即将附着的效果</param>
    /// <returns>返回true表示拒绝附着,任意一个effect拒绝即可</returns>
    /// <example>
    /// 免疫负面效果
    /// </example>
    protected virtual bool OnRejectAttach(Effect effect) {
        return false;
    }

    /// <summary>
    /// 有新效果可附着前调用,判定是否能合并
    /// </summary>
    /// <param name="effect">即将附着的效果</param>
    /// <returns>返回true表示能合并,任意一个</returns>
    /// <example>
    /// 燃烧效果
    /// </example>
    protected virtual bool OnCheckMergeAble(Effect effect) {
        // Todo 为匿名类生成guid
        return
            UiName == effect.UiName &&
            UiDescription == effect.UiDescription &&
            UiIconPath == effect.UiIconPath
            ;
    }

    /// <summary>
    /// 有新效果可合并时调用,决定如何合并
    /// </summary>
    /// <param name="effect">即将合并的效果</param>
    /// <example>
    /// 燃烧效果
    /// </example>
    protected virtual void OnDoMerge(Effect effect) {
        LgOverlay = Math.Min(LgOverlay + effect.LgOverlay, LgMaxOverlay);

        LgRemainingRounds = Math.Max(LgRemainingRounds, effect.LgRemainingRounds);
    }

    /// <summary>
    /// 自身附着后调用
    /// </summary>
    /// <example>
    /// 施加增益效果
    /// </example>
    protected virtual void OnAfterAttach() { }

    /// <summary>
    /// 自身脱离后调用
    /// </summary>
    /// <example>
    /// 移除增益效果
    /// </example>
    protected virtual void OnAfterDetach() { }

    /// <summary>
    /// 目标回合开始前调用
    /// </summary>
    /// <example>
    /// 护甲buff
    /// </example>
    protected virtual void OnAfterTurnStart() { }

    /// <summary>
    /// 自己回合结束后调用.只给自身触发
    /// </summary>
    /// <example>
    /// 中毒buff
    /// </example>
    protected virtual void OnAfterTurnEnd() { }

    /// <summary>
    /// 造成别人的生命修改前,预处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">伤害请求</param>
    /// <example>
    /// 伤害增幅
    /// </example>
    protected virtual void OnBeforeTakeHpChange(RequestHpChange request) { }

    /// <summary>
    /// 自己的生命修改前,预处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">修改请求</param>
    /// <returns>返回是否拒绝请求</returns>
    /// <example>
    /// 免疫伤害/减伤
    /// </example>
    protected virtual bool OnBeforeSelfHpChange(RequestHpChange request) {
        return false;
    }

    /// <summary>
    /// 自己的生命修改后,后处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">修改请求</param>
    /// <example>
    /// 反弹伤害
    /// </example>
    protected virtual void OnAfterSelfHpChange(RequestHpChange request) { }

    /// <summary>
    /// 造成别人的生命修改后,后处理修改请求.已有的所有效果都触发
    /// </summary>
    /// <param name="request">伤害请求</param>
    /// <example>
    /// 吸血
    /// </example>
    protected virtual void OnAfterTakeHpChange(RequestHpChange request) { }

    /// <summary>
    /// 前处理出牌请求
    /// </summary>
    /// <param name="request">出牌请求</param>
    /// <example>
    /// 元素浸染
    /// </example>
    protected virtual void OnBeforePreviewMana(RequestPlayBatchCard request) { }

    /// <summary>
    /// 后处理法力消耗
    /// </summary>
    /// <param name="request">出牌请求</param>
    /// <example>
    /// 减少法力消耗
    /// </example>
    protected virtual void OnAfterPreviewMana(RequestPlayBatchCard request) { }

    /// <summary>
    /// 出牌前调用.已有的所有效果都触发,(多张牌也只调用一次)
    /// </summary>
    /// <param name="request">出牌请求</param>
    /// <example>
    /// 抽一张牌
    /// </example>
    protected virtual void OnBeforePlayBatchCard(RequestPlayBatchCard request) { }

    /// <summary>
    /// 出牌后调用.已有的所有效果都触发,(多张牌也只调用一次)
    /// </summary>
    /// <param name="request">出牌请求</param>
    /// <example>
    /// 本次出牌造成伤害的50%转化为生命
    /// </example>
    protected virtual void OnAfterPlayBatchCard(RequestPlayBatchCard request) { }

    /// <summary>
    /// 渲染调用,用于实现一些自定义ui
    /// </summary>
    protected virtual void OnViewRender(EffectView view) { }

    /// <summary>
    /// 状态修改后调用,用于监听通用属性修改(必须走state的BoardCast才能监听到)
    /// </summary>
    /// <param name="delta"></param>
    protected virtual void OnAfterStateChange(CombatState delta) { }

#endregion

#region 快捷重写事件

    public Func<Effect, Effect, bool> OnImpRejectAttach;

    public Func<Effect, Effect, bool> OnImpCheckMergeAble;

    public Action<Effect, Effect> OnImpDoMerge;

    public Action<Effect> OnImpAfterAttach;

    public Action<Effect> OnImpAfterDetach;

    public Action<Effect> OnImpAfterTurnStart;

    public Action<Effect> OnImpAfterTurnEnd;

    public Action<Effect, RequestHpChange> OnImpBeforeTakeHpChange;

    public Func<Effect, RequestHpChange, bool> OnImpBeforeSelfHpChange;

    public Action<Effect, RequestHpChange> OnImpAfterSelfHpChange;

    public Action<Effect, RequestHpChange> OnImpAfterTakeHpChange;

    public Action<Effect, RequestPlayBatchCard> OnImpBeforePreviewMana;

    public Action<Effect, RequestPlayBatchCard> OnImpAfterPreviewMana;

    public Action<Effect, RequestPlayBatchCard> OnImpBeforePlayBatchCard;

    public Action<Effect, RequestPlayBatchCard> OnImpAfterPlayBatchCard;

    public Action<Effect, EffectView> OnImpViewRender = (self, view) =>
    {
        view.remainTurns.text = self.LgRemainingRounds > 0 ? $"{self.LgRemainingRounds}" : "";
        view.layerCount.text  = self.LgOverlay > 1 ? $"{self.LgOverlay}" : "";
    };

    public Action<Effect, CombatState> OnImpAfterStateChange;

#endregion

#endregion

#region 公开函数

    public int CompareTo(Effect other) {
        return (LgPriority, m_effectStamp).CompareTo((other.LgPriority, other.m_effectStamp));
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

    public bool DoAttach() {
        var reject = !LgTags.Contains(EffectTag.Fixed) && Target.BoardCastAny(effect => effect.RejectAttach(this));
        if (reject) return false;
        var mergeAble = Target.BoardCastAny(effect =>
        {
            if (!effect.CheckMergeAble(this)) return false;
            effect.DoMerge(this);
            return true;
        });
        if (mergeAble) return false;
        m_effectStamp = ms_stamp++;
        Target.Effects.Add(this);
        AfterAttach();
        return !UiHidde;
    }

    public void DoRemove() {
        if (LgTags.Contains(EffectTag.Fixed)) {
            Debug.LogError("固定效果不可移除!");
            return;
        }
        Target.Effects.Remove(this);
        AfterDetach();
    }

#endregion

#region 统一事件接口

    private bool RejectAttach(Effect effect) {
        return OnRejectAttach(effect) || (OnImpRejectAttach?.Invoke(this, effect) ?? false);
    }

    private bool CheckMergeAble(Effect effect) {
        return LgOpenMerge && effect.LgOpenMerge && (OnCheckMergeAble(effect) || (OnImpCheckMergeAble?.Invoke(this, effect) ?? false));
    }

    private void DoMerge(Effect effect) {
        AfterDetach();
        OnDoMerge(effect);
        OnImpDoMerge?.Invoke(this, effect);
        AfterAttach();
    }

    private void AfterAttach() {
        OnAfterAttach();
        OnImpAfterAttach?.Invoke(this);
    }

    private void AfterDetach() {
        OnAfterDetach();
        OnImpAfterDetach?.Invoke(this);
    }

    public void AfterTurnStart() {
        OnAfterTurnStart();
        OnImpAfterTurnStart?.Invoke(this);
    }

    public void AfterTurnEnd() {
        OnAfterTurnEnd();
        OnImpAfterTurnEnd?.Invoke(this);
        if (LgRemainingRounds > 0 && --LgRemainingRounds == 0) Remove();
    }

    public void BeforeTakeHpChange(RequestHpChange request) {
        OnBeforeTakeHpChange(request);
        OnImpBeforeTakeHpChange?.Invoke(this, request);
    }

    public bool BeforeSelfHpChange(RequestHpChange request) {
        return OnBeforeSelfHpChange(request) || (OnImpBeforeSelfHpChange?.Invoke(this, request) ?? false);
    }

    public void AfterSelfHpChange(RequestHpChange request) {
        OnAfterSelfHpChange(request);
        OnImpAfterSelfHpChange?.Invoke(this, request);
    }

    public void AfterTakeHpChange(RequestHpChange request) {
        OnAfterTakeHpChange(request);
        OnImpAfterTakeHpChange?.Invoke(this, request);
    }

    public void BeforePreviewMana(RequestPlayBatchCard request) {
        OnBeforePreviewMana(request);
        OnImpBeforePreviewMana?.Invoke(this, request);
    }

    public void AfterPreviewMana(RequestPlayBatchCard request) {
        OnAfterPreviewMana(request);
        OnImpAfterPreviewMana?.Invoke(this, request);
    }

    public void BeforePlayBatchCard(RequestPlayBatchCard request) {
        OnBeforePlayBatchCard(request);
        OnImpBeforePlayBatchCard?.Invoke(this, request);
    }

    public void AfterPlayBatchCard(RequestPlayBatchCard request) {
        OnAfterPlayBatchCard(request);
        OnImpAfterPlayBatchCard?.Invoke(this, request);
    }

    public void ViewRender(EffectView view) {
        OnViewRender(view);
        OnImpViewRender?.Invoke(this, view);
    }

    public void AfterStateChange(CombatState delta) {
        OnAfterStateChange(delta);
        OnImpAfterStateChange?.Invoke(this, delta);
    }

#endregion
}
}
