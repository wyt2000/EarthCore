using System.Collections;

namespace Combat.Requests {
public abstract class CombatRequest {
    // 所处上下文
    public CombatJudge Judge;

    // 发起人
    public CombatantComponent Causer;

    // 用于拦截请求
    public bool Reject;

    // 检查请求是否合法
    public abstract bool PreCheckValid();

    // 修改逻辑状态前的动画
    public virtual IEnumerable PlayPreAnimation() {
        yield break;
    }

    // 修改逻辑状态的代码
    public abstract void ExecuteLogic();

    // 修改逻辑状态后的动画
    public virtual IEnumerable PlayPostAnimation() {
        yield break;
    }
}
}
