﻿using System;
using System.Collections;
using System.Collections.Generic;
using Combat.Requests.Details;
using UnityEngine;

namespace Combat.Requests {
public abstract class CombatRequest {
#region 配置项

    // 所处上下文
    public CombatJudge Judge;

    // 发起人
    public CombatantComponent Causer;

    // 用于拦截请求
    public bool Reject;

    // 拒绝原因字段
    public string RejectReason;

    // 结点
    public LinkedListNode<CombatRequest> Node;

#endregion

#region 虚函数

    // 检查请求是否能入队列
    public abstract bool CanEnqueue();

    // 真正执行请求(可以多帧执行)
    public virtual IEnumerator Execute() {
        ExecuteNoCross();
        yield break;
    }

    // 真正执行请求(无跨帧)
    protected virtual void ExecuteNoCross() {
        Debug.LogError("没有实现ExecuteNoCross");
    }

    // 任务描述
    public virtual string Description() {
        return null;
    }

    public override string ToString() {
        return Description() ?? base.ToString();
    }

#endregion

#region 辅助函数

    protected void Add(CombatRequest request) {
        request.Causer ??= Causer;
        Judge.Requests.Add(request);
    }

    protected void AddPost(Action action) {
        Judge.Requests.AddPost(new RequestPostLogic {
            Causer   = Causer,
            OnFinish = action
        });
    }

    protected void AddFirst(CombatRequest request) {
        request.Causer ??= Causer;
        Judge.Requests.AddFirst(request);
    }

    protected void AddLast(CombatRequest request) {
        request.Causer ??= Causer;
        Judge.Requests.AddLast(request);
    }

    protected void AddBefore(CombatRequest request) {
        request.Causer ??= Causer;
        Judge.Requests.AddBefore(this, request);
    }

    protected void AddAfter(CombatRequest request) {
        request.Causer ??= Causer;
        Judge.Requests.AddAfter(this, request);
    }

    protected bool Require(bool expression, string error) {
        if (expression) return true;
        Reject       = true;
        RejectReason = error;
        return false;
    }

#endregion
}
}
