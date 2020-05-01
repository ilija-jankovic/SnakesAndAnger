using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Properties that can have houses built on them.
public class Street : Property
{
    [SerializeField]
    private Color _colour;
    [SerializeField]
    private ushort _rent;
    [SerializeField]
    private ushort _housePrice;
    [SerializeField]
    private ushort _firstHouseValue;
    [SerializeField]
    private ushort _secondHouseValue;
    [SerializeField]
    private ushort _thirdHouseValue;
    [SerializeField]
    private ushort _fourthHouseValue;
    [SerializeField]
    private ushort _hotelValue;

    private byte _houses;
    private ushort[] _houseValues;

    public override void Awake()
    {
        base.Awake();
        _houseValues = new ushort[] { _firstHouseValue, _secondHouseValue, _thirdHouseValue, _fourthHouseValue, _hotelValue };

        GameObject banner = GameObject.CreatePrimitive(PrimitiveType.Plane);
        banner.transform.parent = transform;
        banner.transform.localScale = new Vector3(transform.localScale.x / 100, transform.localScale.y / 100, transform.localScale.z / 600);
        banner.transform.localRotation = transform.localRotation;
        banner.transform.localPosition = new Vector3(0,0.55f,0.5f-5*banner.transform.localScale.z);

        banner.GetComponent<Renderer>().material.color = Colour;
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
            houseValues += "With " + i + " House" + (i != 1 ? "s" : "") + ": " + "$" + _houseValues[i - 1] + "\n";
        houseValues += "With HOTEL: $" + _houseValues[4] + "\n";
        return "Price: $" + Price + "\nRent: $" + _rent + houseValues + "One House Costs: $" + _housePrice + "\nMortgage Value: $" + _morgVal;
    }

    public override ushort PaymentPrice()
    {
        return _houses == 0 ? _rent : _houseValues[_houses - 1];
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
