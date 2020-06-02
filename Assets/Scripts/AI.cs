using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    private enum modes { aggressive, passive, trading };

    private const uint TIME_BETWEEN_CLICKS = 120;
    private uint timer = TIME_BETWEEN_CLICKS;

    private const float CHANCE_OF_TRADING_BEFORE_ROLL = 0.5f;
    private const byte MAX_OFFERS_PER_TURN = 3;
    private byte offers;
    void Start()
    {
        
    }

    //Add AI pays when a collect from everyone card is picked up

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
                        //check if a player has anything to trade
                        bool tradingPartnerExists = false;
                        foreach(Player player in GameManager.Players)
                            if(player != playerComp && player.PropertiesOwned.Count > 0)
                            {
                                tradingPartnerExists = true;
                                break;
                            }

                        //has % chance to intitate trading if amount of offers this turn have not been exceded
                        if (tradingPartnerExists && offers < MAX_OFFERS_PER_TURN && UnityEngine.Random.Range(0f, 1f) < CHANCE_OF_TRADING_BEFORE_ROLL)
                            Click(MenuManager.Trade);
                        else
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
                        {
                            //reset number of trade offers AI has attempted
                            offers = 0;
                            Click(MenuManager.NextTurn);
                        }
                }
                else if (MenuManager.PaymentOptions.enabled == true)
                {
                    if (!(GameManager.ActiveCard is CardCollect) || CardCollect.currentPayee == GameManager.CurrentPlayer)
                        if (!Click(MenuManager.Pay))
                            Mortgage();
                }
                else if (MenuManager.PaymentTileOptions.enabled == true)
                {
                    int potentialFunds = GameManager.CurrentPlayer.GetTotalPotentialBalance() + GameManager.CurrentPlayer.GetBalance()/10;
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
                    //goes back if AI has made enough offers this turn
                    if (offers >= MAX_OFFERS_PER_TURN)
                        Click(TradingSystem.Back);
                    else
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
                }
                else if (TradingSystem.TradingOptions.enabled == true)
                {
                    //if AI is the tradee
                    if (TradingSystem.Tradee == playerComp && TradingSystem.CounterOfferInProgress)                          //set it up so AI does not repeat offers
                        TradingDecision(false);
                    //if AI is the trader
                    else if (GameManager.CurrentPlayer == playerComp && !TradingSystem.CounterOfferInProgress)
                        TradingDecision(true);
                }
            }
        }
        timer = timer == 0 ? TIME_BETWEEN_CLICKS + 1 : timer;
        timer--;
    }

    private void TradingDecision(bool aiIsTrader)
    {
        //goes back if AI has made enough offers this turn
        if (aiIsTrader && offers >= MAX_OFFERS_PER_TURN)
            Click(TradingSystem.Back);

        //AI will accept the trade if it gains more than it loses
        uint aiVal = TradingVal(aiIsTrader);
        uint otherVal = TradingVal(!aiIsTrader);

        if (otherVal > aiVal)
        {
            Click(TradingSystem.Accept);
            return;
        }
        //add other's properties to increase value gained by AI
        foreach (Property property in NotOffered(!aiIsTrader))
        {
            TradingSystem.ToggleCardInOffer(property);
            otherVal = TradingSystem.GetTradingValue(Offered(!aiIsTrader));
            if (otherVal >= aiVal)
            {
                Click(TradingSystem.Offer);
                offers++;
                return;
            }
        }

        //get ai cards currently on offer - we will modify the list in trading system while looping through it so
        //we need a clone
        List<Property> aiOfferClone = new List<Property>();
        foreach (Property property in Offered(aiIsTrader))
            aiOfferClone.Add(property);

        //if AI still does not gain enough it removes its properties from trade until it gains enough
        if (aiVal >= otherVal)
        {
            foreach (Property property in aiOfferClone)
            {
                TradingSystem.ToggleCardInOffer(property);
                aiVal = TradingSystem.GetTradingValue(Offered(aiIsTrader));
                if (aiVal <= otherVal)
                {
                    Click(TradingSystem.Offer);
                    offers++;
                    return;
                }
            }
        }
    }

    private uint TradingVal(bool trader)
    {
        return trader ? TradingSystem.GetTradingValue(TradingSystem.CurrentPlayerOffer) : TradingSystem.GetTradingValue(TradingSystem.TradeeOffer);
    }

    private List<Property> Offered(bool trader)
    {
        return trader ? TradingSystem.CurrentPlayerOffer : TradingSystem.TradeeOffer;
    }

    private List<Property> NotOffered(bool trader)
    {
        return trader ? TradingSystem.CurrentPlayerNotOffered : TradingSystem.TradeeNotOffered;
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
