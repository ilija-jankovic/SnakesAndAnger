using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollect : Card
{
    //Amount 
    ushort _amount;


    /// <summary>
    /// Constructor for Cards that remove money from player
    /// </summary>
    /// <param name="id">id of Card</param>
    /// <param name="does">Card description</param>
    /// <param name="amount">amount to be given</param>
    public CardCollect(byte id, string does, ushort amount) : base(id, does)
    {
        _amount = amount;
    }


    public override void Use()
    {
        _owner.AddFunds(_amount);
    }
}
