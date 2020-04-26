using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailwayStation : Property
{
    public RailwayStation(string title, ushort price, ushort mortgageValue) : base(title, price, mortgageValue) { }

    public override string Description()
    {
        string railways = "\n";
        for (byte i = 2; i <= 4; i++)
            railways += "If " + i + " Railways are owned: $" + (byte)(Mathf.Pow(2,i-1) * 25) + "\n";
        return "Rent: $25" + railways + "Mortgage: $" + _morgVal;
    }

    public override ushort PaymentPrice()
    {
        byte stations = 0;
        foreach (Property p in Owner.PropertiesOwned)
            if (p is RailwayStation)
                stations++;
        return (ushort)(25 * Mathf.Pow(2,stations-1));
    }

    public override void DisplayOptions()
    {
        throw new System.NotImplementedException();
    }
}
