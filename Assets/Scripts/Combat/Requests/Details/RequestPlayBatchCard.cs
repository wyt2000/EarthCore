using System.Linq;
using Combat.Cards;
using Combat.Effects;
using Combat.States;
using GUIs.Audios;
using Utils;

namespace Combat.Requests.Details {
public enum BatchCardState {
    // 能出
    CanPlay,

    // 能选但不能出
    CannotPlay,

    // 不能选也不能出
    CannotSelect
}

// 批量出牌请求
public class RequestPlayBatchCard : CombatRequest {
#region 配置项

    // 出牌目标(自己或对手)
    public CombatantComponent Target;

    // 本次请求所有打出的牌
    public Card[] Cards;

#endregion

    // 本次出牌的效果增幅
    public float Scale = 1;

    // 本次出牌消耗的总法力
    public float TotalManaCost;

    // 本次出牌造成的总伤害
    public float TotalDamage;

#region 重写

    public override bool CanEnqueue() {
        return EvaluateState() == BatchCardState.CanPlay;
    }

    // 评估状态
    public BatchCardState EvaluateState() {
        var can =
            Require(
                Judge.Requests.Count == 0,
                "不能在有任务在队列中时出牌"
            );
        if (!can) return BatchCardState.CannotPlay;
        can =
            Require(
                Cards.Length is >= 1 and <= 5,
                "只能出1~5张牌"
            ) &&
            Require(
                Causer != null && Target != null,
                "出牌人或目标为空"
            ) &&
            Require(
                !(Cards.Length > 1 && Cards.Any(c => c.LgUnique)),
                "存在有唯一效果的卡牌"
            ) &&
            Require(
                Cards.Count(c => !c.LgElement.HasValue) <= 1,
                "最多只能有一张无属性牌"
            );
        if (!can) return BatchCardState.CannotSelect;
        can =
            Require(
                !Causer.State.BlockTags.ContainsKey(CombatBlockTag.BlockPlayCard),
                "当前状态不能出牌"
            );
        if (!can) return BatchCardState.CannotPlay;
        var manaEnough = Require(
            PreviewManaCost() <= Causer.State.Mana,
            "法力值不足"
        );
        // 无属性不参与计算
        var elements = Cards.Where(c => c.LgElement.HasValue).ToArray();
        if (elements.Length == 0) return BatchCardState.CanPlay;
        var typeCnt = elements.Select(e => e.LgElement!).Distinct().Count();
        // 判断元素叠加(只有一种元素) 
        if (typeCnt == 1) {
            // 2/3/4/5一次增幅5%/10%/15%/20%
            Scale = 1 + (elements.Length - 1) * 0.05f;
        }
        // 有多种元素,则必须触发元素联动
        else {
            can = Require(
                elements.Length == typeCnt,
                "不能有重复卡"
            );
            if (!can) return BatchCardState.CannotSelect;
            can = Require(
                PreviewElementLink().Item1 != null,
                "元素联动未触发"
            );
            if (!can) return typeCnt == 4 ? BatchCardState.CannotPlay : BatchCardState.CannotSelect;
        }
        return manaEnough ? BatchCardState.CanPlay : BatchCardState.CannotPlay;
    }

    protected override void ExecuteNoCross() {
        // 扣除法力值
        Causer.State.Mana -= TotalManaCost;

        GAudio.PlayPlayCard();

        // 出牌动画
        Add(new RequestAnimation {
            Anim = () => Causer.cardSlot.Discards(Cards)
        });

        // 计算元素联动
        var (link, other) = PreviewElementLink();
        if (link != null) {
            // Todo 元素联动动画
            Judge.logger.AddLog($"触发元素联动{link.UiName}");
            // 元素联动摸牌
            Causer.GetCard(1);
            // 施加联动buff
            (other ? Target : Causer).AddBuffFrom(link, Causer);
            // 全都处理完了再真正出牌
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

        // 依次出牌
        foreach (var card in Cards) {
            Add(new RequestPlayCard {
                Batch   = this,
                Current = card
            });
        }

        // 监听伤害
        var listener = new ListenDamageEffect {
            Causer     = Causer,
            Target     = Causer,
            UiHidde    = true,
            LgPriority = 1
        };

        // 挂数据统计buff
        AddFirst(new RequestEffect {
            Effect = listener,
            Attach = true
        });

        AddPost(() =>
        {
            listener.Remove();
            TotalDamage = listener.TotalDamage;
            Causer.BoardCast(e => e.AfterPlayBatchCard(this));
            Cards.ForEach(c => c.OnAfterPlayBatchCard(this));
        });
    }

    public float PreviewManaCost() {
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

    private (Effect, bool) PreviewElementLink() {
        return EffectLinks.GetElementLink(Cards);
    }

#endregion
}
}
