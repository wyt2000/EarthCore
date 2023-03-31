using System;
using System.Linq;
using Combat.Cards;
using UnityEngine;

namespace Combat.Requests.Details {
// 摸牌请求
public class RequestGetCard : CombatRequest {
#region 配置项

    // 需要几张牌
    public int Count = 1;

    // 满足要求的卡牌
    public Func<Card, bool> Filter = delegate { return true; };

    // 给定候选长度如何选index
    public Func<int, int> SelectIndex = delegate { return 0; };

#endregion

    public override bool PreCheckValid() {
        if (Causer != null && Count > 0) return true;
        Debug.LogWarning("Invalid card request");
        return false;
    }

    public override void ExecuteLogic() {
        var owner = Causer;
        var heap = owner.AllCards;
        var cards = owner.Cards;
        var count = Count;
        var filter = Filter;
        var selector = SelectIndex;
        var valid = heap.Where(filter).ToList();
        // Todo 优化摸牌算法到O(n)
        while (count-- > 0) {
            var index = selector(valid.Count);
            index = Math.Clamp(0, index, valid.Count);
            if (index == valid.Count) continue;
            var card = valid[index];
            card.Owner = owner;
            cards.Add(card);
            valid.RemoveAt(index);
            heap.Remove(card);
        }
    }

    public override string ToString() {
        return $"{Causer.name}摸{Count}张牌";
    }
}
}
