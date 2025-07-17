using UnityEngine;

[CreateAssetMenu(fileName = "TelevisedEventCard", menuName = "Cards/TelevisedEventCard")]
public class TelevisedEventCard : Card
{
    [SerializeField]
    private int _costInMoney;

    [SerializeField]
    private int _fansBonus;


    public override int ReviseEarnedFans(CardContext context, int amount)
    {
        return amount + _fansBonus;
    }

    public override int ReviseEarnedMoney(CardContext context, int amount)
    {
        return amount - _costInMoney;
    }

}
