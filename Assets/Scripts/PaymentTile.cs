using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaymentTile : PriceTypeTile
{
    public override void Awake()
    {
        base.Awake();
        PriceMesh.text = "Pay " + PriceMesh.text;
    }
}
