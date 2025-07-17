using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "new BandStats", menuName = "BandStats")]
public class BandStats : ScriptableObject
{

    [SerializeField]
    private int _money = 0;
    private int _uiMoneyAmount = 0;


    [SerializeField]
    private int _fans = 0;
    private int _uiFanAmount = 0;

    [SerializeField]
    private List<Card> _cards = new List<Card>();

    private HashSet<Card> _removedCards = new HashSet<Card>();

    public int GetNumberOfAllowedMistakesForBonus(int amount)
    {
        foreach (Card card in _cards)
        {
            amount = card.ReviseNumberOfAllowedMistakesForBonus(new CardContext(this), amount);
        }
        return amount;
    }

    public int ReviseMistakeLimit(int amount)
    {
        foreach (Card card in _cards)
        {
            amount = card.ReviseMistakeLimit(new CardContext(this), amount);
        }

        return amount;
    }

    public void UpdateEarnedMoney(int amount)
    {
        foreach (Card card in _cards)
        {
            amount = card.ReviseEarnedMoney(new CardContext(this), amount);
        }
        this.Money += amount;
    }

    public void UpdateFans(int amount)
    {
        foreach (Card card in _cards)
        {
            amount = card.ReviseEarnedFans(new CardContext(this), amount);
        }
        this.Fans += amount;
    }

    public void ApplyRemoveCards()
    {
        foreach (Card card in _removedCards)
        {
            _cards.Remove(card);
        }

        _removedCards.Clear();
    }


    #region properties

    public int UImoneyAmount
    {
        get { return _uiMoneyAmount; }
        set 
        {
            if (value < 0)
            {
                _uiMoneyAmount = 0;
            }
            else
            {
                _uiMoneyAmount = value;
            } 
        }

    }
    public int Money
    {
        get { return _money; }
        set 
        { 
            if (value < 0)
            {
                _money = 0;
            }
            else
            {
                _money = value;
            }  
        }
    }


    public int UIfanAmount
    {
        get { return _uiFanAmount; }
        set
        {
            if (value < 0)
            {
                _uiFanAmount = 0;
            }
            else
            {
                _uiFanAmount = value;
            }
        }

    }
    public int Fans
    {
        get { return _fans; }
        set
        {
            if (value < 0)
            {
                _fans = 0;
            }
            else
            {
                _fans = value;
            }
        }
    }

    #endregion

    public bool hasCard(Predicate<Card> predicate)
    {
        return _cards.Where(predicate.Invoke).Any();
    }

    internal void RemoveCard(Card card)
    {
        _removedCards.Add(card);
    }

    internal void AddCard(Card card)
    {
        _cards.Add(card);
    }

    public IEnumerable<Card> Cards { get { return _cards; } }
}
