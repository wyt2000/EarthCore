using System.Linq;
using Combat.Cards;
using Combat.Effects;
using Utils;

namespace Combat.Requests.Details {
// 批量出牌请求
public class RequestPlayBatchCard : CombatRequest {
#region 配置项

    // 出牌目标(自己或对手)
    public CombatantComponent Target;

    // 本次请求所有打出的牌
    public Card[] Cards;

    // 本次出牌消耗的总法力
    public float TotalManaCost;

    // 本次出牌造成的总伤害
    public float TotalDamage;

#endregion

#region 重写

    public override bool CanEnqueue() {
        return RequireAll(
            Require(
                Judge.Requests.Count == 0,
                "不能在有任务在队列中时出牌"
            ),
            Require(
                Causer != null && Cards is { Length: > 0 },
                "无效的出牌请求"
            ),
            Require(
                PreviewManaCost() <= Causer.State.Mana,
                "法力值不足"
            )
        );
    }

    protected override void ExecuteNoCross() {
        // 扣除法力值
        Causer.State.Mana -= TotalManaCost;

        // 计算元素联动
        var (link, other) = PreviewElementLink();
        if (link != null) {
            (other ? Target : Causer).AddBuffFrom(link, Causer);
            Add(new RequestLogic {
                Causer = Causer,
                Logic  = RealExecuteLogic
            });
        }
        else {
            RealExecuteLogic();
        }
    }

    public override string Description() {
        return $"批量出牌x{Cards.Length}";
    }

#endregion

#region 辅助函数

    private class ListenDamageEffect : Effect {
        public float TotalDamage;

        protected override void OnAfterTakeHpChange(RequestHpChange request) {
            base.OnAfterTakeHpChange(request);
            if (request.IsHeal) return;
            TotalDamage += request.Value;
        }
    }

    private void RealExecuteLogic() {
        Cards.ForEach(c => Causer.Cards.Remove(c));

        // 出牌前回调
        Cards.ForEach(c => c.OnBeforePlayBatchCard(this));
        Causer.BoardCast(e => e.BeforePlayBatchCard(this));

        foreach (var card in Cards) {
            Add(new RequestPlayCard {
                Batch   = this,
                Current = card
            });
        }

        Add(new RequestAnimation {
            Anim = () => Causer.cardSlot.Discards(Cards)
        });

        // 监听伤害
        var listener = new ListenDamageEffect {
            Causer     = Causer,
            Target     = Causer,
            UiHidde    = true,
            LgPriority = 1
        };
        AddFirst(new RequestEffect {
            Effect = listener,
            Attach = true
        });

        AddPost(() => {
            listener.Remove();
            TotalDamage = listener.TotalDamage;
            Causer.BoardCast(e => e.AfterPlayBatchCard(this));
            Cards.ForEach(c => c.OnAfterPlayBatchCard(this));
        });
    }

    public float PreviewManaCost() {
        // 浸染
        Cards.ForEach(c =>
        {
            if (c.LgInfect) {
                Cards.ForEach(c2 => c2.LgElement ??= c.LgElement);
            }
        });

        Cards.ForEach(c => c.OnBeforePreviewMana(this));
        Causer.BoardCast(e => e.BeforePreviewMana(this));

        Cards.ForEach(c => c.ConfirmValue());
        TotalManaCost = Cards.Sum(c => c.LgManaCost);

        Cards.ForEach(c => c.OnAfterPreviewMana(this));
        Causer.BoardCast(e => e.AfterPreviewMana(this));

        var link = PreviewElementLink().Item1;
        link?.AfterPreviewMana(this);
        return TotalManaCost;
    }

    public (Effect, bool) PreviewElementLink() {
        return EffectLinks.GetElementLink(Cards);
    }

#endregion
}
}
