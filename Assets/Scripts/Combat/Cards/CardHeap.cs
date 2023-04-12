﻿using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Requests.Details;
using Utils;

namespace Combat.Cards {
public class CardHeap {
    public readonly List<Card> AllCards = new();

    private readonly List<Card> m_discards = new();

    public int DiscardCount => m_discards.Count;

    public CardHeap() {
        for (var i = 0; i < 1; ++i) {
            AllCards.AddRange(CardDetails.CloneAll());
        }

        GRandom.Shuffle(AllCards);
    }

    private void ReuseCard() {
        if (m_discards.Count == 0) return;
        // 洗牌
        GRandom.Shuffle(m_discards);
        AllCards.AddRange(m_discards.Select(card => card.Clone()));
        m_discards.Clear();
    }

    // 回收卡牌
    public void RecycleCard(IEnumerable<Card> cards) {
        m_discards.AddRange(cards);
    }

    // Todo 优化摸牌算法到O(n)
    public List<Card> GetCards(RequestGetCard request) {
        // 牌不够时尝试重用卡牌
        if (AllCards.Count == 0) ReuseCard();

        var owner = request.Causer;
        var count = request.Count;
        var filter = request.Filter;
        var selector = request.SelectIndex;
        var valid = AllCards.Where(filter).ToList();
        var cards = new List<Card>();
        while (count-- > 0) {
            var index = selector(valid.Count);
            index = Math.Clamp(0, index, valid.Count);
            if (index == valid.Count) continue;
            var card = valid[index];
            card.Owner = owner;
            cards.Add(card);
            valid.RemoveAt(index);
            AllCards.Remove(card);
        }
        return cards;
    }
}
}
