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
            railways += "If " + i + " Railways are owned: $" + i * 25 + "\n";
        return "Rent: $25" + railways + "Mortgage: $" + MortgageValue;
    }

    public override ushort PaymentPrice()
    {
        throw new System.NotImplementedException();
    }

    public override void DisplayOptions()
    {
        throw new System.NotImplementedException();
    }
}
