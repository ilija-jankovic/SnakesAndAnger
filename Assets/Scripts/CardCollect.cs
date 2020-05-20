using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollect : Card
{
    //Amount 
    ushort _amount;
    byte _type;

    /// <summary>
    /// Constructor for Cards that remove money from player
    /// </summary>
    /// <param name="does">Card description</param>
    /// <param name="amount">amount to be given</param>
    public CardCollect(string does, ushort amount, byte type) : base(does)
    {
        _amount = amount;
        _type = type;
    }


    public override void Use()
    {
        if (_type == 1)
        {
            _owner.AddFunds(_amount);
        }
        else //collect 50 from each player
        {
            foreach(Player p in GameManager.Players)
            {
                GameManager.PlayerMustPay((ushort)_amount, p);
                if ((System.Array.IndexOf(GameManager.Players, p)) != -1)
                {
                    _owner.AddFunds(_amount);
                }
            }
        } // camera doesn't move to show whos paying
    }


}
