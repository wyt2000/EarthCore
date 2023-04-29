using Combat.Requests.Details;

namespace Combat.Effects.Templates {
// 作用在本次出牌的效果
public class EffectBatch : Effect {
    public EffectBatch() {
        UiHidde = true;
    }

    protected override void OnAfterPlayBatchCard(RequestPlayBatchCard request) {
        base.OnAfterPlayBatchCard(request);

        Remove();
    }
}
}
