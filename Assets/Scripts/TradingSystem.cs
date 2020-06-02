using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public static class TradingSystem
{
    private static Canvas _tradingSetup = GameObject.Find("TradingSetupOptions").GetComponent<Canvas>();
    private static Canvas _tradingOptions = GameObject.Find("TradingOptions").GetComponent<Canvas>();
    private static Canvas _backFromTradingOptions = GameObject.Find("BackFromTradingOptions").GetComponent<Canvas>();

    private static Button[] _buttons;
    private static Button _backFromTrading = GameObject.FindGameObjectWithTag("BackFromTradingButton").GetComponent<Button>();
    private static Button _offer = GameObject.FindGameObjectWithTag("OfferButton").GetComponent<Button>();
    private static Button _accept = GameObject.FindGameObjectWithTag("AcceptButton").GetComponent<Button>();
    private static Button[] _players;

    private static Player _tradee = null;
    private static bool _counterOffer = false;

    private static List<Property> _playerOffer;
    private static List<Property> _tradeeOffer;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        //initialise _buttons to have only trading system buttons
        Button[] allButtons = GameObject.FindObjectsOfType<Button>();
        List<Button> tradingButtons = new List<Button>();
        foreach (Button button in allButtons)
            if (button.transform.parent.tag == "TradingSystem")
                tradingButtons.Add(button);

        _buttons = new Button[tradingButtons.Count];
        for (byte i = 0; i < _buttons.Length; i++)
            _buttons[i] = tradingButtons[i];

        //fill _player array
        GameObject[] tradingPartnerButtonObjects = GameObject.FindGameObjectsWithTag("TradingPartnerButton");
        _players = new Button[tradingPartnerButtonObjects.Length];
        for (byte i = 0; i < tradingPartnerButtonObjects.Length; i++)
            _players[i] = tradingPartnerButtonObjects[i].GetComponent<Button>();
    }

    public static void ShowTradingOptions()
    {
        MenuManager.SwitchToMenu(TradingSetup);
        MenuManager.ShowMenu(BackFromTradingOptions);

        //activate buttons based on whether players are currently playing
        foreach (Button button in TradingPartnerOptions)
        {
            button.interactable = false;
            foreach (Player player in GameManager.Players)
                if(player != GameManager.CurrentPlayer && player.gameObject.name == button.GetComponentInChildren<Text>().text)
                {
                    button.interactable = true;
                    break;
                }
        }

        Back.interactable = true;
    }

    public static void CallTradingButtonListener(Button button)
    {
        if(button == Back)
        {
            //destroy previous cards
            if (TradingOptions.enabled == true)
            {
                foreach (GameObject cardObj in GameObject.FindGameObjectsWithTag("InventoryCard"))
                    GameObject.Destroy(cardObj);
                MenuManager.SwitchToMenu(TradingSetup);
                MenuManager.ShowMenu(BackFromTradingOptions);
            }
            else
                MenuManager.SwitchToMenuWithInventory(MenuManager.TurnOptions);

            _tradee = null;
            //prevent memory leaks
            Resources.UnloadUnusedAssets();
        }
        else if(button == Offer && !(MenuManager.ButtonClicked && ((Tradee.gameObject.GetComponent<AI>() != null && CounterOfferInProgress) 
                                                                    || (GameManager.CurrentPlayer.gameObject.GetComponent<AI>() != null && !CounterOfferInProgress))))
        {
            Text text = Offer.GetComponentInChildren<Text>();
            text.text = text.text == "Offer" ? "CounterOffer" : "Offer";

            _counterOffer = !_counterOffer;
            Accept.interactable = true;
            Back.interactable = !Back.interactable;

            UpdateCardsInTrade();
            Offer.interactable = false;
        }
        else if(button == Accept && !(MenuManager.ButtonClicked && ((Tradee.gameObject.GetComponent<AI>() != null && CounterOfferInProgress)
                                                                    || (GameManager.CurrentPlayer.gameObject.GetComponent<AI>() != null && !CounterOfferInProgress))))
        {
            foreach (Property property in _playerOffer)
            {
                GameManager.CurrentPlayer.RemoveProperty(property);
                Tradee.AddProperty(property);
            }
            foreach(Property property in _tradeeOffer)
            {
                Tradee.RemoveProperty(property);
                GameManager.CurrentPlayer.AddProperty(property);
            }

            _tradee = null;
            MenuManager.SwitchToMenuWithInventory(MenuManager.TurnOptions);
        }
        else if(Array.IndexOf(TradingPartnerOptions, button) != -1)
        {
            //set tradee to selected player
            foreach (Player player in GameManager.Players)
                if (player.gameObject.name == button.GetComponentInChildren<Text>().text)
                {
                    _tradee = player;
                    break;
                }

            //reset offer lists
            _tradeeOffer = new List<Property>();
            _playerOffer = new List<Property>();

            MenuManager.SwitchToMenu(TradingOptions);
            MenuManager.ShowMenu(BackFromTradingOptions);

            _counterOffer = false;
            Accept.interactable = false;
            Offer.GetComponentInChildren<Text>().text = "Offer";

            UpdateCardsInTrade();
            Offer.interactable = false;
        }
    }

    private static void UpdateCardsInTrade()
    {
        Offer.interactable = true;

        //destroy previous cards
        if (TradingOptions.enabled == true)
            foreach (GameObject cardObj in GameObject.FindGameObjectsWithTag("InventoryCard"))
                GameObject.Destroy(cardObj);

        //set up cards in trading system
        Image playerPanel = GameObject.Find("PlayerPanel").GetComponent<Image>();
        Image tradeePanel = GameObject.Find("TradeePanel").GetComponent<Image>();
        Image playerOfferPanel = GameObject.Find("PlayerOfferPanel").GetComponent<Image>();
        Image tradeeOfferPanel = GameObject.Find("TradeeOfferPanel").GetComponent<Image>();

        //set trader names/colour
        foreach (GameObject panelObj in GameObject.FindGameObjectsWithTag("TraderInfo"))
        {
            if (panelObj.transform.parent == playerOfferPanel.transform)
            {
                panelObj.GetComponentInChildren<Text>().text = GameManager.CurrentPlayer.gameObject.name;
                //lighten/darken panel if current player is trading
                panelObj.GetComponent<Image>().color = !CounterOfferInProgress ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.35f);
                continue;
            }
            //only other panel is tradee panel
            panelObj.GetComponentInChildren<Text>().text = Tradee.gameObject.name;
            //lighten/darken panel if current player is trading
            panelObj.GetComponent<Image>().color = !CounterOfferInProgress ? new Color(1f, 1f, 1f, 0.35f) : new Color(1f, 1f, 1f, 1f);
        }

        List<Property> notOffered = new List<Property>();
        foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
            if (!CurrentPlayerOffer.Contains(property))
                notOffered.Add(property);
        MenuManager.CreateRow(playerPanel.rectTransform, notOffered);

        notOffered.Clear();
        foreach (Property property in Tradee.PropertiesOwned)
            if (!TradeeOffer.Contains(property))
                notOffered.Add(property);
        MenuManager.CreateRow(tradeePanel.rectTransform, notOffered);

        MenuManager.CreateRow(playerOfferPanel.rectTransform, CurrentPlayerOffer);
        MenuManager.CreateRow(tradeeOfferPanel.rectTransform, TradeeOffer);
    }

    public static void ToggleCardInOffer(Property property)
    {
        if (Tradee != null && Tradeable(property))
            if (property.Owner == GameManager.CurrentPlayer)
                ToggleCardInOffer(property, CurrentPlayerOffer);
            else if (property.Owner == Tradee)
                ToggleCardInOffer(property, TradeeOffer);
    }

    private static void ToggleCardInOffer(Property property, List<Property> offer)
    {
        if (offer.Contains(property))
            offer.Remove(property);
        else
            offer.Add(property);
        Accept.interactable = false;
        UpdateCardsInTrade();
    }

    private static bool Tradeable(Property property)
    {
        Street street = property.GetComponent<Street>();
        return !(street != null && street.Houses > 0);
    }

    public static Canvas TradingSetup
    {
        get { return _tradingSetup; }
    }

    public static Canvas TradingOptions
    {
        get { return _tradingOptions; }
    }

    private static Canvas BackFromTradingOptions
    {
        get { return _backFromTradingOptions; }
    }

    public static Button Back
    {
        get { return _backFromTrading; }
    }

    public static Button Offer
    {
        get { return _offer; }
    }

    public static Button Accept
    {
        get { return _accept; }
    }

    public static Button[] Buttons
    {
        get { return _buttons; }
    }

    public static Button[] TradingPartnerOptions
    {
        get { return _players; }
    }

    public static List<Property> CurrentPlayerOffer
    {
        get { return _playerOffer; }
    }

    public static List<Property> CurrentPlayerNotOffered
    {
        get { return NotOnOffer(GameManager.CurrentPlayer, CurrentPlayerOffer); }
    }

    public static List<Property> TradeeOffer
    {
        get { return _tradeeOffer; }
    }

    public static List<Property> TradeeNotOffered
    {
        get { return NotOnOffer(Tradee, TradeeOffer); }
    }

    private static List<Property> NotOnOffer(Player player, List<Property> onOffer)
    {
        List<Property> notOffered = new List<Property>();
        foreach (Property property in player.PropertiesOwned)
            if (!onOffer.Contains(property))
                notOffered.Add(property);
        return notOffered;
    }

    public static Player Tradee
    {
        get { return _tradee; }
    }

    public static bool CounterOfferInProgress
    {
        get { return _counterOffer; }
    }

    //Evaluates the total amount a player can trade. Useful for AI behaviour.
    public static uint GetTradingValue(List<Property> properties)
    {
        uint value = 0;
        foreach (Property property in properties)
        {
            Street street = property.GetComponent<Street>();
            if (Tradeable(property))
                if (!property.Mortgaged)
                    value += property.Price;
                else
                    value += property.MortgageValue;
        }
        return value;
    }

    public static bool HumanCounterOffering
    {
        get 
        {
            if (Tradee == null)
                return false;
            return Tradee.GetComponent<AI>() == null && CounterOfferInProgress;
        }
    }
}
