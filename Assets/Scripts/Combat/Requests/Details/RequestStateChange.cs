namespace Combat.Requests.Details {
// Todo 用于状态变更的UI显示,例如加个动画让数值平滑滚动到目标值
public class RequestStateChange : CombatRequest {
    public override bool CanEnqueue() {
        throw new System.NotImplementedException();
    }

    public override string ToString() {
        return "状态变更";
    }
}
}
