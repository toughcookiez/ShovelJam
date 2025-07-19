
using UnityEngine;



public abstract class Card : ScriptableObject
{
    public string Title;
    public string Description;
    public Sprite BackGround;
    public int Price;

    public virtual void Apply(CardContext context)
    {
        context.RemoveCard(this);
    }

    public virtual int ReviseNumberOfAllowedMistakesForBonus(CardContext context, int amount)
    {
        return amount;
    }

    public virtual int ReviseEarnedMoney(CardContext context, int amount)
    {
        return amount;
    }
    public virtual int ReviseEarnedFans(CardContext context, int amount)
    {
        return amount;
    }

    public virtual int ReviseMistakeLimit(CardContext context, int amount)
    {
        return amount;
    }

}
