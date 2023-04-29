using System;
using System.Collections;

namespace Combat.Requests.Details {
public class RequestAnimation : CombatRequest {
    public Func<IEnumerator> Anim = () => null;

    public override bool CanEnqueue() {
        return Require(
            Anim != null,
            "无效的延迟动画请求"
        );
    }

    public override IEnumerator Execute() {
        return Anim();
    }

    public override string ToString() {
        return "延迟动画";
    }
}
}
