using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "MoneyCard", menuName = "Cards/MoneyCard")]
public class MoneyCard : Card
{
    [SerializeField]
    private int _amount;

    public override void Apply(CardContext context)
    {
        base.Apply(context);
        context.AddMoney(this._amount);
    }
}