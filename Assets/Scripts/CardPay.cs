using static EnumsForCards.cardPay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPay : Card
{
    //Amount 
    private ushort _amount;
    //type of cardPay, found inside class EnumsForCards
    EnumsForCards.cardPay _type;

    /// <summary>
    /// Constructor for Cards that remove money from player
    /// </summary>
    /// <param name="does">Card description</param>
    /// <param name="amount">amount to be given</param>
    /// <param name="type">type of payment</param>
    public CardPay(string does, ushort amount, EnumsForCards.cardPay type) : base(does)
    {
        _amount = amount;
        _type = type;
    }


    public override void Use()
    {
        if (_type == EnumsForCards.cardPay.directRemove) //pay bank
        {
            GameManager.PlayerMustPay(_amount);
        }
        else if (_type == EnumsForCards.cardPay.payEachPlayer) //pay each player
        {
            PayPlayers();
        }
        else //for each hotel / house
        {
            ForHotelAndHouses();
        }
        base.Use();
    }


    private void PayPlayers() 
    {
        int toPay = _amount * (GameManager.Players.Length-1);

        //checks player can pay amount and pay it
        GameManager.PlayerMustPay((ushort)toPay);

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


    }
    public ushort Amount
    {
        get { return _amount; }
    }
    public EnumsForCards.cardPay Type
    {
        get { return _type; }
    }

    public override Sprite Icon
    {
        get { return Resources.Load<Sprite>("makePayCard"); }
    }
}
