using UnityEngine;
//Base class for all properties in Monopoly.
public abstract class Property : Tile
{
    [SerializeField]
    private string _title;
    [SerializeField]
    private ushort _price;
    protected ushort _morgVal;
    private bool _mortgaged;
    private Player _owner;
    protected TextMesh _titleMesh;
    protected TextMesh _priceMesh;

    public virtual void Awake()
    {
        _morgVal = (ushort)(_price/2);

        //title
        _titleMesh = new GameObject().AddComponent<TextMesh>();
        _titleMesh.anchor = TextAnchor.UpperCenter;
        _titleMesh.alignment = TextAlignment.Center;

        _titleMesh.text = _title.Replace('$', '\n');
        _title = _title.Replace('$', ' ');

        _titleMesh.color = Color.black;
        _titleMesh.fontSize = 12;
        _titleMesh.transform.parent = transform;
        _titleMesh.transform.localEulerAngles = new Vector3(90, 0, 0);
        _titleMesh.transform.localScale = new Vector3(0.1f, 0.0625f, 1);
        _titleMesh.transform.localPosition = new Vector3(0,0,0.4f);


        //price
        _priceMesh = new GameObject().AddComponent<TextMesh>();
        _priceMesh.anchor = TextAnchor.UpperCenter;
        _priceMesh.alignment = TextAlignment.Center;

        _priceMesh.text = "$"+Price;

        _priceMesh.color = Color.black;
        _priceMesh.fontSize = 12;
        _priceMesh.transform.parent = transform;
        _priceMesh.transform.localEulerAngles = new Vector3(90, 0, 0);
        _priceMesh.transform.localScale = new Vector3(0.1f, 0.0625f, 1);
        _priceMesh.transform.localPosition = new Vector3(0, 0, -0.30f);
    }

    public string Title
    {
        get { return _title; }
    }
    public abstract string Description();

    //price to purchase the property
    public ushort Price
    {
        get { return _price; }
    }

    //price player pays when stepping on an owned property
    public abstract ushort PaymentPrice();
    
    public Player Owner
    {
        get { return _owner; }
    }

    public void ChangeOwner(Player player)
    {
        _owner = player;
    }

    public bool Mortgaged
    {
        get { return _mortgaged; }
    }

    public virtual void Mortgage()
    {
        if (!Mortgaged)
        {
            _mortgaged = true;
            Owner.AddFunds(_morgVal);
        }
    }

    public void UnMortgage()
    {
        if (Mortgaged && Owner.GetBalance() >= Price)
        {
            Owner.RemoveFunds(Price);
            _mortgaged = false;
        }
    }
}
