﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Properties that can have houses built on them.
public class Street : Property
{
    private byte _houses;
    private ushort _housePrice;
    private readonly ushort _rent;
    private readonly ushort[] _houseVals;
    private Color _colour;

    public Street(Color colour, string title, ushort price, ushort mortgageValue, ushort housePrice, ushort rent, ushort[] houseValues) : base(title, price, mortgageValue)
    {
        if (houseValues.Length != 5)
            throw new System.IndexOutOfRangeException("Streets must have 5 separate house values.");
        _colour = colour;
        _housePrice = housePrice;
        _rent = rent;
        _houseVals = houseValues;
    }

    public Color Colour
    {
        get { return _colour; }
    }

    //displays amount received by different number of houses
    public override string Description()
    {
        string houseValues = "\n";
        for (byte i = 1; i <= 4; i++)
            houseValues += "With " + i + " House" + (i != 1 ? "s" : "") + ": " + "$" + _houseVals[i - 1] + "\n";
        houseValues += "With HOTEL: $" + _houseVals[4] + "\n";
        return "Price: $" + Price + "\nRent: $" + _rent + houseValues + "One House Costs: $" + _housePrice + "\nMortgage Value: $" + MortgageValue;
    }

    public override ushort PaymentPrice()
    {
        return _houses == 0 ? _rent : _houseVals[_houses - 1];
    }

    public void BuildHouse()
    {
        if (CanBuildHouses() && _houses < 5) _houses++;
    }

    //player must own all properties of the same colour to build houses
    public bool CanBuildHouses()
    {
        throw new NotImplementedException();
    }

    public override void Mortgage()
    {
        base.Mortgage();
        _houses = 0;
    }

    public override void DisplayOptions()
    {
        throw new NotImplementedException();
    }
}
