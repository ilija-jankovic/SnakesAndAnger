//Base class for all properties in Monopoly.
public abstract class Property : Tile
{
    protected string _title;
    private ushort _price;
    private ushort _morgVal;
    private bool _mortgaged;
    //private Player owner;

    public Property(string title, ushort price, ushort morgtageValue)
    {
        _title = title;
        _price = price;
        _morgVal = morgtageValue;
    }

    public abstract string Description();

    public ushort Price
    {
        get { return _price; }
    }

    //price player pays when stepping on an owned property
    public abstract ushort PaymentPrice();

    /*
    public Player Owner
    {
        get { return _owner; }
    }

    public void ChangeOwner(Player player)
    {

    }
    */
    public ushort MortgageValue
    {
        get { return _morgVal; }
    }

    public bool Mortgaged
    {
        get { return _mortgaged; }
    }

    public virtual void Mortgage()
    {
        _mortgaged = true;
    }

    public void UnMortgage()
    {
        _mortgaged = false;
    }
}
