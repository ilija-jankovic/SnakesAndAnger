using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : Property
{
    public override void Awake()
    {
        base.Awake();
    }
    public override string Description()
    {
        return "If one \"Utility\" is owned, rent is 4 times amount shown on dice.\n" +
               "If both \"Utilies\" are owned, rent is 10 times amount shown on dice.\n\n" +
               "Mortgage Value: $" + _morgVal;
    }

    public override ushort PaymentPrice()
    {
        byte utilities = ((Func<byte>)(() =>
        {
            foreach (Property p in Owner.PropertiesOwned)
                if (p != this && p is Utility)
                    return 10;
            return 4;
        }))();
        return (ushort)(Die.Result * utilities);
    }

    public override void DisplayOptions()
    {
        throw new System.NotImplementedException();
    }
}
