using System;

namespace Combat.Requests.Details {
// 用来做延迟调用
public sealed class RequestLogic : CombatRequest {
#region 配置项

    public Action Logic;

#endregion

    public override bool CanEnqueue() {
        return Require(
            Logic != null,
            "无效的自定义逻辑请求"
        );
    }

    protected override void ExecuteNoCross() {
        Logic.Invoke();
    }

    public override string ToString() {
        return "自定义逻辑";
    }
}
}
