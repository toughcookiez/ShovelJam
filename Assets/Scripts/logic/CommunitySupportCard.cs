using UnityEngine;

[CreateAssetMenu(fileName = "CommunitySupportCard", menuName = "Cards/CommunitySupportCard")]
public class CommunitySupportCard : Card
{
    [SerializeField]
    private int _amount;

    public override int ReviseMistakeLimit(CardContext context, int amount)
    {
            if (_amount > 0)
            {
                _amount--;
                return amount;
            }
            else
            {
            return amount++;     
            }


    }
}
