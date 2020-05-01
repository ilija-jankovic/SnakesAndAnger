﻿using UnityEngine;
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

    public virtual void Awake()
    {
        _morgVal = (ushort)(_price/2);
    }

    public string Title
    {
        get { return _title; }
    }
    public abstract string Description();

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

    public bool CanPurchase(Player player)
    {
        return Owner == null && player.GetBalance() >= Price;
    }

    public void Purchase(Player player)
    {
        if(CanPurchase(player))
        {
            player.RemoveFunds(Price);
            player.AddProperty(this);
            _owner = player;
        }
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
