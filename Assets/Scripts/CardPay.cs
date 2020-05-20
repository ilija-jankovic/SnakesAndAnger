using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPay : Card
{
    //Amount 
    ushort _amount;
    //type of payment
    byte _type;
    // type 1 = direct remove
    // type 2 = pay each player
    // type 3 = for each hotel and house owned

    /// <summary>
    /// Constructor for Cards that remove money from player
    /// </summary>
    /// <param name="does">Card description</param>
    /// <param name="amount">amount to be given</param>
    /// <param name="type">type of payment</param>
    public CardPay(string does, ushort amount, byte type) : base(does)
    {
        _amount = amount;
        _type = type;
    }


    public override void Use()
    {
        if (_type == 1) //pay bank
        {
            GameManager.PlayerMustPay(_amount);
        }
        else if (_type == 2) //pay each player
        {
            PayPlayers();
        }
        else //for each hotel / house
        {
            ForHotelAndHouses();
        }
    }


    private void PayPlayers() //Need to make while checking if the payment has been made or the yhave forfeited
    {
        decimal toPay = _amount * GameManager.Players.Length;

        //checks player can pay amount
        GameManager.PlayerMustPay((ushort)toPay);
        //if statement to check player still exists
        if ((System.Array.IndexOf(GameManager.Players, Owner)) != -1)
        {
            //Pays amount to each player
            foreach (Player p in GameManager.Players)
            {
                p.AddFunds(_amount);
            }
        }

    }

    private void ForHotelAndHouses() //Need to make while checking if the payment has been made or the yhave forfeited. Also need to add Hotel stuff
    {
        byte houses = 0;
        byte hotels = 0;
        decimal pay = 0;
        ushort toPay = 0;
        //checks player can pay amount
        foreach (Property p in Owner.PropertiesOwned)
        {
            if (p is Street)
            {
                //adds houses on street to houses local variable
                Street strt = (Street)p;
                if (strt.Houses != 5)
                {
                    houses += strt.Houses;
                }
                else
                {
                    hotels += 1;
                }
            }
        }
        //calculates amount to pay
        pay = (25 * (int)houses) + (100 * (int)hotels);
        toPay = (ushort)pay;
        GameManager.PlayerMustPay(toPay);

        if ((System.Array.IndexOf(GameManager.Players, Owner)) != -1)
        {
            Owner.RemoveFunds(toPay);
        }

    }
}
