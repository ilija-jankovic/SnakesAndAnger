using static EnumsForCards.cardCollect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollect : Card
{
    //Amount 
    ushort _amount;
    //type of cardCollect, found inside class EnumsForCards
    EnumsForCards.cardCollect _type;
    public static Player currentPayee = null;

    /// <summary>
    /// Constructor for Cards that remove money from player
    /// </summary>
    /// <param name="does">Card description</param>
    /// <param name="amount">amount to be given</param>
    public CardCollect(string does, ushort amount, EnumsForCards.cardCollect type) : base(does)
    {
        _amount = amount;
        _type = type;
    }


    public override void Use()
    {
        if (_type == EnumsForCards.cardCollect.fromBank)
        {
            Owner.AddFunds(_amount);
        }
        else //collect 50 from each player
        {
            currentPayee = GameManager.Players[(Array.IndexOf(GameManager.Players, GameManager.CurrentPlayer) + 1) % GameManager.Players.Length];
            GameManager.PlayerMustPay(50, currentPayee);

            //camera moves to show who is paying
            CameraFollow.target = currentPayee.transform;
        }
    }



}
