using System;

namespace Combat.Requests.Details {
// 用来做延迟调用
public sealed class RequestLogic : CombatRequest {
#region 配置项

    public Action Logic;

#endregion

    public override bool PreCheckValid() {
        return true;
    }

    public override void ExecuteLogic() {
        Logic?.Invoke();
    }

    public override string ToString() {
        return "自定义回调";
    }
}
}
