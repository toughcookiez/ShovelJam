using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;


[CreateAssetMenu(fileName = "Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{


    [SerializeField]
    private List<Card> _cards = new List<Card>();

    public int Count
    {
        get
        {
            return _cards.Count;
        }
    }

    /// <summary>
    /// Gets a random list of cards.
    /// </summary>
    /// <param name="excludedCards"></param>
    /// <param name="numberOfCards"></param>
    /// <returns></returns>
    public List<Card> selectRandomCards(int numberOfCards)
    {
        return selectRandomDecks(new HashSet<Card>(), 1, numberOfCards)[0];
    }

    /// <summary>
    /// Gets a random list of cards.
    /// </summary>
    /// <param name="excludedCards"></param>
    /// <param name="numberOfCards"></param>
    /// <returns></returns>
    public List<Card> selectRandomCards(HashSet<Card> excludedCards, int numberOfCards)
    {
        return selectRandomDecks(excludedCards, 1, numberOfCards)[0];
    }

    /// <summary>
    /// Gets a random list of decks with random cards.
    /// </summary>
    /// <param name="excludedCards"></param>
    /// <param name="numberOfDecks"></param>
    /// <param name="numberOfCardsPerDeck"></param>
    /// <returns></returns>
    public List<List<Card>> selectRandomDecks(HashSet<Card> excludedCards, int numberOfDecks, int numberOfCardsPerDeck)
    {
        List<List<Card>> decks = new List<List<Card>>();

        for (int i = 0; i < numberOfDecks; i++)
        {
            decks.Add(new List<Card>());
        }

        foreach (Card excludedCard in excludedCards)
        {
            if (excludedCard == null)
            {
                continue;
            }
            Debug.Log("Excludnig this card:" + excludedCard.Title);
        }


        List<int> shuffledIndicies = Shuffle(_cards.Count);


        int numberOfCards = numberOfDecks * numberOfCardsPerDeck;

        int currentDeck = 0;
        int currentCount = 0;
        foreach (int index in shuffledIndicies)
        {
            if (excludedCards.Contains(_cards[index])) {
                Debug.Log("Skipping this card:" + _cards[index].Title);
                continue;
            }

            Debug.Log("Choosing this card:" + _cards[index].Title);
            decks[currentDeck].Add(_cards[index]);
            currentCount++;
            currentDeck = (currentDeck + 1) % numberOfDecks;

            if (currentCount == numberOfCards)
            {
                break;
            }
        }

        return decks;
    }



    private static List<int> Shuffle(int numberOfitems)
    {
        if (numberOfitems == 0) return new List<int> { };

        List<int> list = Enumerable.Range(0, numberOfitems).ToList();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    internal void replaceCards(List<Card> cards)
    {
        _cards.Clear();
        _cards.AddRange(cards);
    }

    public Card this[int i]
    {
        get => _cards[i];

    }
}
