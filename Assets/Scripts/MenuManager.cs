using static EnumsForCards.cardPay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class MenuManager
{
    private static Canvas[] allMenus;
    private static Canvas _startOfTurn = GameObject.Find("TurnOptions").GetComponent<Canvas>();
    private static Canvas _endOfTurn = GameObject.Find("EndOfTurnOptions").GetComponent<Canvas>();
    private static Canvas _propertyCardInfo = GameObject.Find("PropertyCardInfo").GetComponent<Canvas>();
    private static Canvas _usableCardInfo = GameObject.Find("UsableCardInfo").GetComponent<Canvas>();
    private static Canvas _inventory = GameObject.Find("Inventory").GetComponent<Canvas>();
    private static Canvas _payment = GameObject.Find("PaymentOptions").GetComponent<Canvas>();
    private static Canvas _lose = GameObject.Find("LoseOptions").GetComponent<Canvas>();
    private static Canvas _paymentTileOptions = GameObject.Find("PaymentTileOptions").GetComponent<Canvas>();
    private static Canvas _cardOptions = GameObject.Find("CardOptions").GetComponent<Canvas>();
    private static Canvas _tileLinkOptions = GameObject.Find("TileLinkOptions").GetComponent<Canvas>();
    private static Canvas _mainMenu = GameObject.Find("MainMenu").GetComponent<Canvas>();
    private static Canvas _setupOptions = GameObject.Find("SetupOptions").GetComponent<Canvas>();
    //make this at some point
    private static Canvas _winMenu;

    //ingame buttons - helpful to expicitly list them for AI
    private static Button[] allButtons;
    private static Button _roll = GameObject.FindGameObjectWithTag("RollButton").GetComponent<Button>();
    private static Button _buy = GameObject.FindGameObjectWithTag("BuyButton").GetComponent<Button>();
    private static Button _pay = GameObject.FindGameObjectWithTag("PayButton").GetComponent<Button>();
    private static Button _payFixed = GameObject.FindGameObjectWithTag("PayFixed").GetComponent<Button>();
    private static Button _payPercentage = GameObject.FindGameObjectWithTag("PayPercentageTotalEarning").GetComponent<Button>();
    private static Button _acknowledgeCard = GameObject.FindGameObjectWithTag("AcknowledgeCardButton").GetComponent<Button>();
    private static Button _auction = GameObject.FindGameObjectWithTag("AuctionButton").GetComponent<Button>();
    private static Button _trade = GameObject.FindGameObjectWithTag("TradeButton").GetComponent<Button>();
    private static Button _nextTurn = GameObject.FindGameObjectWithTag("NextTurnButton").GetComponent<Button>();
    private static Button _bankrupt = GameObject.FindGameObjectWithTag("LoseButton").GetComponent<Button>();
    private static Button _buildHouse = GameObject.FindGameObjectWithTag("BuildHouseButton").GetComponent<Button>();
    private static Button _backToNormalCamera = GameObject.FindGameObjectWithTag("BackFromOverviewButton").GetComponent<Button>();

    private static bool _buttonClicked = false;

    private static Camera _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    private static Camera _overviewCamera = GameObject.FindGameObjectWithTag("OverviewCamera").GetComponent<Camera>();

    private static bool _houseMode;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        allMenus = GameObject.FindObjectsOfType<Canvas>();
        allButtons = GameObject.FindObjectsOfType<Button>();

        //add listeners to buttons
        foreach (Button button in allButtons)
            button.onClick.AddListener(delegate { _buttonClicked = true; CallIngameButtonListener(button); });

        //outside of game buttons
        GameObject.Find("StartDefaultButton").GetComponent<Button>().onClick.AddListener(SetupManager.StandardGame);
        GameObject.Find("CustomGameButton").GetComponent<Button>().onClick.AddListener(SetupManager.LoadSetupMenu);

        SwitchToMenu(MainMenu);
    }

    //Funcitionality of all buttons relevant to an ongoing game. Their listeners are all placed
    //in this method as they can be called without pressing a button from the AI script.
    public static void CallIngameButtonListener(Button button)
    {
        Player player = GameManager.CurrentPlayer;
        if (player == null)
            return;

        bool ai = player.GetComponent<AI>();
        //Checks if ai called the listener or if the current player called it. This is needed
        //as if the player presses a button during the ai's turn, it needs to not do anything.
        if (button.interactable && ((!ai && ButtonClicked) || (ai && !ButtonClicked) 
            || /*only these buttons can be pressed by human when AI is active*/ button == Pay || button == TradingSystem.Offer || button == TradingSystem.Accept))
        {
            if (button == Roll)
                Die.Roll();
            else if (button == Buy)
            {
                GameManager.CurrentPlayer.Purchase();
                GameManager.UpdateBuyButtonInteractibility();
                GameManager.UpdateAuctionButtonInteractibility();
                GameManager.UpdateNextPlayerButtonInteractibility();
                UpdateInventoryData();
            }
            else if (button == PayFixed)
            {
                GameManager.PlayerMustPay(200);
            }
            else if (button == PayPercentage)
            {
                GameManager.PlayerMustPay((ushort)(0.1 * GameManager.CurrentPlayer.GetTotalPotentialBalance()));
            }
            else if (button == Pay)
            {
                Property property = GameManager.CurrentPlayer.Position.GetComponent<Property>();
                ChanceTile chance = GameManager.CurrentPlayer.Position.GetComponent<ChanceTile>();
                PaymentTile paymentTile = GameManager.CurrentPlayer.Position.GetComponent<PaymentTile>();
                if (property != null)
                {
                    ushort payment = property.PaymentPrice();
                    GameManager.CurrentPlayer.RemoveFunds(payment);
                    property.Owner.AddFunds(payment);
                    SwitchToMenuWithInventory(EndOfTurnOptions);
                }
                else if (chance != null)
                {
                    if (GameManager.ActiveCard is CardCollect)
                    {
                        ushort payment = 50;
                        CardCollect.currentPayee.RemoveFunds(payment);
                        GameManager.CurrentPlayer.AddFunds(payment);
                        CardCollect.currentPayee = GameManager.Players[(Array.IndexOf(GameManager.Players, CardCollect.currentPayee) + 1) % GameManager.Players.Length];

                        //switches payment to next player
                        if (CardCollect.currentPayee != GameManager.CurrentPlayer)
                            GameManager.PlayerMustPay(payment, CardCollect.currentPayee);
                        //if looped back around to current player, stop payment loop
                        else
                            SwitchToMenuWithInventory(EndOfTurnOptions);

                        //moves camera to whomever needs to pay
                        CameraFollow.target = CardCollect.currentPayee.transform;
                    }
                    else if (GameManager.ActiveCard is CardPay)
                    {
                        CardPay activeCard = (CardPay)GameManager.ActiveCard;
                        ushort payment = GameManager.PaymentNeeded;
                        GameManager.CurrentPlayer.RemoveFunds(payment);
                        CardPay cardPay = (CardPay)GameManager.ActiveCard;

                        //if statement to check player still exists
                        if (cardPay.Type == EnumsForCards.cardPay.payEachPlayer)
                        {
                            //Pays amount to each player
                            foreach (Player p in GameManager.Players)
                            {
                                if (p != GameManager.CurrentPlayer)
                                    p.AddFunds(activeCard.Amount);
                            }
                        }
                        SwitchToMenuWithInventory(EndOfTurnOptions);
                    }
                }
                else if (paymentTile != null)
                {
                    ushort payment = GameManager.PaymentNeeded;
                    GameManager.CurrentPlayer.RemoveFunds(payment);
                    SwitchToMenuWithInventory(EndOfTurnOptions);
                }
            }
            else if (button == AcknowledgeCard)
            {
                SwitchToMenuWithInventory(EndOfTurnOptions);
                if (!(GameManager.ActiveCard is CardGetOutOfJail || GameManager.ActiveCard is TileLink))
                    GameManager.ActiveCard.Use();
                UpdateInventoryData();
            }
            else if (button == Auction)
            {
                Auction.interactable = false;
                GameManager.UpdateBuyButtonInteractibility();
                GameManager.UpdateNextPlayerButtonInteractibility();                                                   //update this when Trading System is implemented
                AuctionSystem.StartAuction();
            }
            else if (button == NextTurn)
            {
                GameManager.NextPlayer();
            }
            else if (button == Bankrupt)
            {
                //remove all properties and cards from player
                Player lostPlayer = GameManager.CurrentPlayer;
                lostPlayer.Reset();
                GameManager.NextPlayer();
                GameManager.RemoveActivePlayer(lostPlayer);
            }
            else if (button == BuildHouse)
            {
                _houseMode = !_houseMode;
                if (_houseMode)
                {
                    Roll.interactable = false;
                    Trade.interactable = false;
                    BuildHouse.GetComponentInChildren<Text>().text = "Back";
                }
                else
                {
                    Roll.interactable = true;
                    Trade.interactable = true;
                    BuildHouse.GetComponentInChildren<Text>().text = "Build House";
                }
                UpdateInventoryData();
            }
            else if (button == BackToNormalCamera)
            {
                SwitchToMenuWithInventory(TurnOptions);
                SwitchToCamera(MainCamera);
            }
            else if(button == Trade)
            {
                TradingSystem.ShowTradingOptions();
            }
            //go to TradingSystem class if button is part of the trading system
            else if(Array.IndexOf(TradingSystem.Buttons, button) != -1)
            {
                TradingSystem.CallTradingButtonListener(button);
            }
        }

        _buttonClicked = false;
    }

    private static void DisableMenu(Canvas canvas)
    {
        canvas.enabled = false;
    }

    private static void DisableAllMenus()
    {
        foreach (Canvas canvas in allMenus)
            DisableMenu(canvas);
    }

    public static void SwitchToMenu(Canvas menu)
    {
        DisableAllMenus();
        foreach (Canvas canvas in allMenus)
            if (canvas == menu)
            {
                menu.enabled = true;
                /*if(menu == TurnOptions)
                {
                    //enable build house button if at least 1 house can be built
                    bool canBuildHouse = false;
                    foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned) {
                        Street street = property.GetComponent<Street>();
                        if (street != null && street.CanBuildHouse())
                        {
                            canBuildHouse = true;
                            break;
                        }
                    }

                    Button buildHouseButton = GameObject.FindGameObjectWithTag("BuildHouseButton").GetComponent<Button>();
                    if (!canBuildHouse)
                        buildHouseButton.interactable = true;
                    else
                        buildHouseButton.interactable = false;
                }
                else */if(menu == EndOfTurnOptions)
                {
                    GameManager.UpdateAuctionButtonInteractibility();
                    GameManager.UpdateBuyButtonInteractibility();
                    GameManager.UpdateNextPlayerButtonInteractibility();
                }
                else if(menu == PaymentOptions)
                    //displays price to pay on button
                    GameObject.FindGameObjectWithTag("PayButton").GetComponent<Button>().GetComponentInChildren<Text>().text = "Pay $" + GameManager.PaymentNeeded;
                break;
            }
    }

    public static void SwitchToMenuWithInventory(Canvas menu)
    {
        SwitchToMenu(menu);
        UpdateInventoryData();
    }

    public static void ShowMenu(Canvas menu)
    {
        menu.enabled = true;
    }

    public static void UpdateInventoryData()
    {
        UpdateInventoryData(GameManager.CurrentPlayer);
    }

    public static void UpdateInventoryData(Player player)
    {
        if (player != null)
        {
            ShowMenu(Inventory);
            GameObject.FindGameObjectWithTag("Balance").GetComponent<Text>().text = "$" + GameManager.CurrentPlayer.GetBalance();

            //get rid of previous cards
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("InventoryCard"))
                GameObject.Destroy(obj);

            //prevent memory leaks
            Resources.UnloadUnusedAssets();

            CreateRow(Inventory.transform, player.PropertiesOwned, new Vector2(-430, -260), new Vector2(50, 100));

            //show usable cards in inventory
            for (int i = 0; i < GameManager.CurrentPlayer.UsableCards.Count; i++)
            {
                Card usableCard = GameManager.CurrentPlayer.UsableCards[i];
                GameObject cardObj = new GameObject("InventoryCard");
                Image card = cardObj.AddComponent<Image>();
                card.tag = "InventoryCard";
                card.transform.SetParent(Inventory.transform);
                card.rectTransform.sizeDelta = new Vector2(50, 100);
                card.rectTransform.localPosition = new Vector2(-430, 260 - 120 * i);

                cardObj.AddComponent<InventoryCardMouseInputUI>().card = usableCard;

                //may be a source of lag
                RawImage img = new GameObject().AddComponent<RawImage>();
                img.transform.SetParent(card.transform);
                img.rectTransform.localPosition = Vector2.zero;
                img.rectTransform.sizeDelta = new Vector2(40, 40);

                Texture2D icon = usableCard.Icon;
                if (icon != null)
                    img.texture = icon;

                //gray out card if player already rolled
                if (TurnOptions.enabled == false)
                    DisableCard(cardObj);
            }
        }
    }

    public static void CreateRow(RectTransform parent, List<Property> properties)
    {
        CreateRow(parent, properties, new Vector2(-parent.rect.width / 2, 0), new Vector2(parent.rect.height / 2, parent.rect.height));
    }

    public static void CreateRow(Transform parent, List<Property> properties, Vector2 position, Vector2 size)
    {
        for (byte i = 0; i < properties.Count; i++)
            CreatePropertyCard(parent, properties[i], new Vector2(position.x + size.x / 2 + (10 + size.x) * i, position.y), size);
    }

    public static void CreatePropertyCard(Transform parent, Property property, Vector2 localPosition, Vector2 sizeDelta)
    {
        Street street = property.GetComponent<Street>();

        GameObject cardObj = new GameObject("InventoryCard");
        Image card = cardObj.AddComponent<Image>();
        card.tag = "InventoryCard";
        card.transform.SetParent(parent);
        card.rectTransform.sizeDelta = sizeDelta;
        card.rectTransform.localPosition = localPosition;

        cardObj.AddComponent<InventoryPropertyMouseInputUI>().property = property;

        Text title = new GameObject().AddComponent<Text>();
        title.text = property.Title;
        title.fontSize = 6;
        title.color = Color.black;
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.alignment = TextAnchor.UpperCenter;
        title.transform.SetParent(card.transform);
        title.rectTransform.sizeDelta = new Vector2(card.rectTransform.sizeDelta.x * 4 / 5, card.rectTransform.sizeDelta.y / 6);
        title.rectTransform.localPosition = new Vector2(0f, 35f);

        if (!property.Mortgaged)
        {
            Text price = new GameObject().AddComponent<Text>();
            price.text = "$" + property.Price;
            price.fontSize = 6;
            price.color = Color.black;
            price.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            price.alignment = TextAnchor.UpperCenter;
            price.transform.SetParent(card.transform);
            price.rectTransform.sizeDelta = new Vector2(card.rectTransform.sizeDelta.x * 4 / 5, card.rectTransform.sizeDelta.y / 6);
            price.rectTransform.localPosition = new Vector2(0f, -40f);

            if (street != null)
            {
                Image streetColour = new GameObject("StreetColour").AddComponent<Image>();
                streetColour.transform.SetParent(card.transform);
                streetColour.rectTransform.sizeDelta = new Vector2(card.rectTransform.sizeDelta.x, card.rectTransform.sizeDelta.y / 6);
                streetColour.rectTransform.localPosition = new Vector2(0f, card.rectTransform.sizeDelta.y/2-streetColour.rectTransform.sizeDelta.y/2);
                streetColour.color = new Color(street.Colour.r, street.Colour.g, street.Colour.b, 1);

                title.rectTransform.localPosition = new Vector2(0f, 20f);

                //grays out street if not able to build house in house mode
                if (BuildHouseMode && !street.CanBuildHouse())
                    DisableCard(cardObj);
            }
            else if (property.GetComponent<Utility>() != null || property.GetComponent<RailwayStation>() != null)
            {
                Image icon = new GameObject("Icon").AddComponent<Image>();
                icon.sprite = property.GetComponent<Tile>().Icon;
                icon.rectTransform.SetParent(card.transform);
                icon.rectTransform.localPosition = Vector2.zero;
                icon.rectTransform.sizeDelta = new Vector2(card.rectTransform.sizeDelta.x * 0.7f, card.rectTransform.sizeDelta.x * 0.7f);
            }
        }
        else
        {
            title.text = "MORTGAGED";
            title.rectTransform.localPosition = Vector3.zero;
        }

        //grays out non-street properties if in house mode or if cant sell house, or if in payment menu and property mortgaged, or can't pay unmortgage cost
        if ((!TradingMode 
            && ((BuildHouseMode && (street == null || street.Mortgaged))
            || (!BuildHouseMode && street != null && !street.CanMortagage() && !street.Mortgaged && !street.CanSellHouse())
            || (PaymentOptions.enabled == true && property.Mortgaged)
            || (property.Mortgaged && property.Owner.GetBalance() < property.UnMortgageCost)))
            || (TradingMode && street != null && street.Houses > 0))
            DisableCard(cardObj);
    }

    private static void DisableCard(GameObject cardObj)
    {
        Image grayOut = new GameObject("GrayOut").AddComponent<Image>();
        Image img = cardObj.GetComponent<Image>();
        grayOut.transform.SetParent(img.transform);
        grayOut.transform.localPosition = Vector2.zero;
        grayOut.rectTransform.sizeDelta = img.rectTransform.sizeDelta;
        grayOut.color = new Color(0f, 0f, 0f, 0.5f);

        cardObj.GetComponent<MouseInputUI>().clickEnabled = false;
    }

    public static Canvas TurnOptions
    {
        get { return _startOfTurn; }
    }

    public static Canvas EndOfTurnOptions
    {
        get { return _endOfTurn; }
    }

    public static Canvas PropertyCardInfo
    {
        get { return _propertyCardInfo; }
    }

    public static Canvas PaymentTileOptions
    {
        get { return _paymentTileOptions; }
    }

    public static Canvas UsableCardInfo
    {
        get { return _usableCardInfo; }
    }

    public static Canvas LoseOptions
    {
        get { return _lose; }
    }

    public static Canvas CardOptions
    {
        get { return _cardOptions; }
    }

    public static bool BuildHouseMode
    {
        get { return _houseMode; }
    }

    public static bool TradingMode
    {
        get
        {
            foreach (Canvas canvas in allMenus)
                if (canvas.enabled == true && canvas.tag == "TradingSystem")
                    return true;
            return false;
        }
    }

    public static void UpdateCardInfo(Property property)
    {
        ShowMenu(PropertyCardInfo);
        DisableMenu(UsableCardInfo);
        if (property != null)
        {
            GameObject.FindGameObjectWithTag("PropertyInfo").GetComponent<Text>().text = property.Description();
            GameObject.FindGameObjectWithTag("PropertyTitle").GetComponent<Text>().text = property.GetComponent<Property>().Title;

            Image streetColour = GameObject.FindGameObjectWithTag("StreetColour").GetComponent<Image>();
            if (!property.Mortgaged)
            {
                //set colour of card
                Street street = property.GetComponent<Street>();
                if (street != null)
                    streetColour.color = new Color(street.Colour.r, street.Colour.g, street.Colour.b, 1);
                else
                    streetColour.color = Vector4.zero;
            }
            else
            {
                GameObject.FindGameObjectWithTag("PropertyInfo").GetComponent<Text>().text = "\n\nMORTGAGED for $" + property.MortgageValue + "\nPay $" + property.UnMortgageCost + " to unmortgage";
                streetColour.color = Vector4.zero;
            }
        }
        else
            DisableMenu(PropertyCardInfo);
    }

    public static void UpdateCardInfo(Card card)
    {
        ShowMenu(UsableCardInfo);
        DisableMenu(PropertyCardInfo);
        if(card != null)
        {
            GameObject.FindGameObjectWithTag("UsableCardInfo").GetComponent<Text>().text = card.GetDescription();

            GameObject imageObj = GameObject.FindGameObjectWithTag("CardSprite");
            RawImage img = imageObj.GetComponent<RawImage>();
            Texture2D icon = card.Icon;
            if (icon != null)
                img.texture = icon;
            else
                img.texture = null;
        }
        else
            DisableMenu(UsableCardInfo);
    }

    public static Canvas WinMenu
    {
        get { return _winMenu; }
    }

    public static Canvas Inventory
    {
        get { return _inventory; }
    }

    public static Canvas PaymentOptions
    {
        get { return _payment; }
    }

    public static Canvas TileLinkOptions
    {
        get { return _tileLinkOptions; }
    }

    public static Canvas MainMenu
    {
        get { return _mainMenu; }
    }

    public static Canvas SetupOptions
    {
        get { return _setupOptions; }
    }

    public static Button Roll
    {
        get { return _roll; }
    }

    public static Button Buy
    {
        get { return _buy; }
    }

    public static Button Pay
    {
        get { return _pay; }
    }

    public static Button PayFixed
    {
        get { return _payFixed; }
    }

    public static Button PayPercentage
    {
        get { return _payPercentage; }
    }

    public static Button AcknowledgeCard
    {
        get { return _acknowledgeCard; }
    }

    public static Button Auction
    {
        get { return _auction; }
    }

    public static Button Trade
    {
        get { return _trade; }
    }

    public static Button NextTurn
    {
        get { return _nextTurn; }
    }

    public static Button Bankrupt
    {
        get { return _bankrupt; }
    }

    public static Button BuildHouse
    {
        get { return _buildHouse; }
    }

    public static Button BackToNormalCamera
    {
        get { return _backToNormalCamera; }
    }

    public static Camera MainCamera
    {
        get { return _mainCamera; }
    }

    public static Camera OverviewCamera
    {
        get { return _overviewCamera; }
    }

    public static void SwitchToCamera(Camera camera)
    {
        MainCamera.gameObject.SetActive(false);
        OverviewCamera.gameObject.SetActive(false);
        camera.gameObject.SetActive(true);
    }

    public static bool ButtonClicked
    {
        get { return _buttonClicked; }
    }
}
