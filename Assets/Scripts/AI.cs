using System.Collections;
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
        if (timer == 0)
        {
            Player playerComp = gameObject.GetComponent<Player>();
            if (GameManager.CurrentPlayer == playerComp
                || CardCollect.currentPayee == playerComp
                || TradingSystem.Tradee == playerComp)
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
                else if (TradingSystem.TradingSetup.enabled == true)
                {
                    uint highestVal = 0;
                    Button playerButton = null;

                    //finds player who can trade the most total value                        //change this for different personalitites
                    foreach (Button button in TradingSystem.TradingPartnerOptions)
                    {
                        if (button.interactable == true)
                        {
                            uint val = 0;
                            foreach (Player player in GameManager.Players)
                                if (player.gameObject.name == button.GetComponentInChildren<Text>().text)
                                {
                                    val = TradingSystem.GetTradingValue(player.PropertiesOwned);
                                    break;
                                }
                            if (val >= highestVal)
                            {
                                highestVal = val;
                                playerButton = button;
                            }
                        }
                    }
                    Click(playerButton);
                }
                else if (TradingSystem.TradingOptions.enabled == true)
                {
                    //if AI is the tradee
                    if (TradingSystem.CounterOfferInProgress)                        //change this for different personalitites
                    {
                        //AI will accept the trade if it gains more than it loses
                        uint traderVal = TradingSystem.GetTradingValue(TradingSystem.CurrentPlayerOffer);
                        uint thisVal = TradingSystem.GetTradingValue(TradingSystem.TradeeOffer);
                        Debug.Log(traderVal + " " + thisVal);
                        if (traderVal >= thisVal)
                            Click(TradingSystem.Accept);
                        else
                        {
                            //get trader cards not currently on offer
                            List<Property> notOffered = new List<Property>();
                            foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
                                if (!TradingSystem.CurrentPlayerOffer.Contains(property))
                                    notOffered.Add(property);

                            //add trader properties to increase value gained by AI
                            foreach (Property property in notOffered)
                            {
                                TradingSystem.ToggleCardInOffer(property);
                                traderVal = TradingSystem.GetTradingValue(TradingSystem.CurrentPlayerOffer);
                                if (traderVal > thisVal)
                                {
                                    Click(TradingSystem.Offer);
                                    break;
                                }
                            }

                            //get tradee cards currently on offer - we will modify the list in trading system while looping through it so
                            //we need a clone
                            List<Property> thisOnOffer = new List<Property>();
                            foreach (Property property in TradingSystem.Tradee.PropertiesOwned)
                                if (TradingSystem.TradeeOffer.Contains(property))
                                    thisOnOffer.Add(property);

                            //if AI still does not gain enough it removes its properties from trade
                            if (thisVal >= traderVal)
                            {
                                foreach (Property property in thisOnOffer)
                                {
                                    TradingSystem.ToggleCardInOffer(property);
                                    thisVal = TradingSystem.GetTradingValue(TradingSystem.TradeeOffer);
                                    if (thisVal <= traderVal)
                                    {
                                        Click(TradingSystem.Offer);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        timer = timer == 0 ? TIME_BETWEEN_CLICKS + 1 : timer;
        timer--;
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
        return active;
    }
}
