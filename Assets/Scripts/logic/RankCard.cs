using UnityEngine;

[CreateAssetMenu(fileName = "RankCard", menuName = "Cards/RankCard")]
public class RankCard : Card
{
    [SerializeField]
    private int _numberOfAllowedMistakes = 0;


    public override int ReviseNumberOfAllowedMistakesForBonus(CardContext context, int amount)
    {
        return amount += _numberOfAllowedMistakes;
    }

}
