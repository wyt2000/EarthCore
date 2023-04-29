using System;

namespace Combat.Requests.Details {
// 后处理逻辑(所有请求执行完毕后执行)
public sealed class RequestPostLogic : CombatRequest {
#region 配置项

    public Action OnFinish;

#endregion

    public override bool CanEnqueue() {
        return Require(
            OnFinish != null,
            "无效的后处理逻辑请求"
        );
    }

    protected override void ExecuteNoCross() {
        OnFinish.Invoke();
    }

    public override string ToString() {
        return "后处理逻辑";
    }
}
}
