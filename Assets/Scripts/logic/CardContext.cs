using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardContext
{
    private readonly BandStats _bandStats;

    public CardContext(BandStats bandStats)
    {
        _bandStats = bandStats;
    }


    public void AddMoney(int amount)
    {
        _bandStats.Money += amount;
    }

    internal void RemoveCard(Card card)
    {
        _bandStats.RemoveCard(card);
    }

    
}