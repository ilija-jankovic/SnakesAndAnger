﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    private enum modes { aggressive, passive, trading };
    private const uint TIME_BETWEEN_CLICKS = 120;
    private uint timer = TIME_BETWEEN_CLICKS;
    void Start()
    {
        
    }

    void Update()
    {
        if (GameManager.CurrentPlayer == gameObject.GetComponent<Player>() || GameManager.CurrentPlayer == CardCollect.currentPayee)
        {
            if (timer == 0)
            {
                if (MenuManager.TurnOptions.enabled == true)
                {
                    if (!MenuManager.BuildHouseMode)
                    {
                        //Build house if possible - update this for different AI modes
                        bool canBuildHouse = false;
                        foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
                            if (property.GetComponent<Street>() != null && property.GetComponent<Street>().CanBuildHouse())
                            {
                                canBuildHouse = true;
                                break;
                            }

                        if (canBuildHouse)
                            Click(MenuManager.BuildHouse);
                        else
                            Click(MenuManager.Roll);
                    }
                    else
                    {
                        //Build house on heighest value streets - update this for different AI modes
                        bool houseBuilt = false;
                        for (byte i = (byte)(GameManager.CurrentPlayer.PropertiesOwned.Count - 1); i >= 0; i--)
                        {
                            foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
                            {
                                Street street = property.GetComponent<Street>();
                                if (street != null && street.CanBuildHouse())
                                {
                                    street.BuildHouse();
                                    houseBuilt = true;
                                    break;
                                }
                            }
                            if (houseBuilt)
                            {
                                Click(MenuManager.BuildHouse);
                                break;
                            }
                        }
                    }
                }
                else if (MenuManager.EndOfTurnOptions.enabled == true)
                {
                    if (!Click(MenuManager.Buy))
                        if (!Click(MenuManager.Auction))
                            Click(MenuManager.NextTurn);
                }
                else if (MenuManager.PaymentOptions.enabled == true)
                {
                    if (!(GameManager.ActiveCard is CardCollect) || CardCollect.currentPayee == GameManager.CurrentPlayer)
                        if (!Click(MenuManager.Pay))
                            Mortgage();
                }
                else if (MenuManager.PaymentTileOptions.enabled == true)
                {
                    int potentialFunds = GameManager.CurrentPlayer.GetTotalPotentialBalance();
                    if (potentialFunds > 200)                                               //may need to change the 200 for super tax tile
                        Click(MenuManager.PayFixed);
                    else
                        Click(MenuManager.PayPercentage);
                }
                else if (MenuManager.CardOptions.enabled == true)
                {
                    Click(MenuManager.AcknowledgeCard);
                }
                else if (MenuManager.LoseOptions.enabled == true)
                {
                    Click(MenuManager.Bankrupt);
                }
                else
                    timer = TIME_BETWEEN_CLICKS;
            }
            
            timer--;
        }
    }

    //mortgage/sell properties when AI has enough potential funds but doesn't have enough in hand
    private void Mortgage()
    {
        foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
            if (property.CanMortagage())
            {
                property.Mortgage();
                return;
            }

        //if can't mortgage AI can still sell houses
        foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
        {
            Street street = property.GetComponent<Street>();
            if (street != null && street.CanSellHouse())
            {
                street.SellHouse();
                return;
            }
        }
    }

    private bool Click(Button button)
    {
        bool active = button.interactable;
        MenuManager.CallIngameButtonListener(button);
        timer = TIME_BETWEEN_CLICKS;
        return active;
    }
}
