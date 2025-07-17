using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "AdvertismentCard", menuName = "Cards/AdvertismentCard")]
public class AdvertismentCard : Card
{
    [SerializeField]
    private int _amount;


    public override int ReviseEarnedMoney(CardContext context, int amount)
    {
        try
        {
            if (amount > 0)
            {
                return amount + _amount;
            }
            else
            {
                return amount - _amount;
            }
        }
        finally
        {
            context.RemoveCard(this);
        }
    }

}
