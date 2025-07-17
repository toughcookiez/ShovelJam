using UnityEngine;

[CreateAssetMenu(fileName = "SponserCard", menuName = "Cards/SponserCard")]
public class SponserCard : Card
{
    [SerializeField]
    private int _moneyBonus;

    [SerializeField]
    private int _costInFans;

    public override int ReviseEarnedFans(CardContext context, int amount)
    {
        return amount - _costInFans;
    }

    public override int ReviseEarnedMoney(CardContext context, int amount)
    {
        return amount + _moneyBonus;
    }
}
