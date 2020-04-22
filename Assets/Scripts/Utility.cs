using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : Property
{
    public Utility(string title, ushort price, ushort mortgageValue) : base(title, price, mortgageValue) { }

    public override string Description()
    {
        return "If one \"Utility\" is owned, rent is 4 times amount shown on dice.\n" +
               "If both \"Utilies\" are owned, rent is 10 times amount shown on dice.\n\n" +
               "Mortgage Value: $" + MortgageValue;
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
