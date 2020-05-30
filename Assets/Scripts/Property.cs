using UnityEngine;
//Base class for all properties in Monopoly.
public abstract class Property : PriceTypeTile
{
    protected ushort _morgVal;
    private bool _mortgaged;

    public override void Awake()
    {
        base.Awake();
        _morgVal = (ushort)(Price/2);
    }

    //price player pays when stepping on an owned property
    public abstract ushort PaymentPrice();

    public abstract string Description();

    public Player Owner
    {
        get
        {
            foreach (Player player in GameManager.Players)
                if (player.PropertiesOwned.Contains(this))
                    return player;
            return null;
        }
    }

    public bool Mortgaged
    {
        get { return _mortgaged; }
    }

    public ushort MortgageValue
    {
        get { return _morgVal; }
    }

    public ushort UnMortgageCost
    {
        get { return (ushort)(MortgageValue + MortgageValue / 10); }
    }

    public virtual bool CanMortagage()
    {
        return !Mortgaged;
    }

    public virtual void Mortgage()
    {
        if (CanMortagage())
        {
            _mortgaged = true;
            Owner.AddFunds(_morgVal);

            GameManager.UpdatePayButtonInteractibility();
            GameManager.UpdateBuyButtonInteractibility();
            MenuManager.UpdateInventoryData(Owner);
        }
    }

    public void UnMortgage()
    {
        if (Mortgaged && Owner.GetBalance() >= UnMortgageCost)
        {
            Owner.RemoveFunds(Price);
            _mortgaged = false;

            GameManager.UpdateBuyButtonInteractibility();
            MenuManager.UpdateInventoryData(Owner);
        }
    }

    public virtual void ReturnToBank()
    {
        _mortgaged = false;
    }
}
