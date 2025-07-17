using UnityEngine;

[CreateAssetMenu(fileName = "TelevisedEventCard", menuName = "Cards/TelevisedEventCard")]
public class TelevisedEventCard : Card
{
    [SerializeField]
    private int _cost;

    [SerializeField]
    private int _fans;


    public override int ReviseEarnedFans(CardContext context, int amount)
    {
        return amount + _fans;
    }

    public override int ReviseEarnedMoney(CardContext context, int amount)
    {
        return amount - _cost;
    }

}
