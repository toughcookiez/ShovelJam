using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class ShopHandler : MonoBehaviour
{
    [SerializeField] private BandStats _bandStats;

    [SerializeField] private Deck _shopDeck;

    [SerializeField] private Deck _primaryDeck;

    [SerializeField] private List<CardHandler> _cards;


    private void Start()
    {
        if (_primaryDeck == null)
        {
            Debug.LogError("Shop handler has no primary deck.");
            return;
        }

        if (_shopDeck == null)
        {
            Debug.LogError("Shop handler has no shop deck.");
            return;
        }

        if (_cards == null || _cards.Count == 0)
        {
            Debug.LogError("Cannot apply show deck to empty card handler list.");
            return;
        }

        if (_shopDeck.Count != _cards.Count)
        {
            _shopDeck.replaceCards(_primaryDeck.selectRandomCards(_cards.Count));
        }

        if (_shopDeck.Count != _cards.Count)
        {
            Debug.LogError("Invalid shop deck state.");
            return;
        }

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].Card = _shopDeck[i];
        }
    }


    public void AddCard(CardHandler card)
    {
        if (card.Price > _bandStats.Money)
        {
            return;
        }
        if (_bandStats.Cards.Contains(card.Card))
        {
            return;
        }
        if (_bandStats.Cards.Count() == 3)
        {
            return;
        }
        _bandStats.Money -= card.Price;
        _bandStats.AddCard(card.Card);
        card.GetComponent<Animator>().SetTrigger("Flip");
    }

    internal void AddAllCards(HashSet<Card> excludedCards)
    {
        if (_cards == null)
        {
            return;
        }

        foreach (var card in _cards)
        {
            if (card == null || card.Card == null)
            {
                continue;
            }

            excludedCards.Add(card.Card);
        }
    }

    internal void UpdateShowDeck()
    {
        if (_cards == null || _cards.Count == 0)
        {
            Debug.LogError("Updating shop deck failed: no cards.");
            return;
        }

        if (_shopDeck == null)
        {
            Debug.LogError("Updating shop deck failed: no shop deck.");
            return;
        }

        _shopDeck.replaceCards(_cards.Select(c => c.Card).ToList());

    }
}
