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
    //make this at some point
    private static Canvas _winMenu;

    private static bool _houseMode;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        allMenus = GameObject.FindObjectsOfType<Canvas>();

        //call Roll method in Die class when 'RollButton' is clicked
        GameObject.FindGameObjectWithTag("RollButton").GetComponent<Button>().onClick.AddListener(Die.Roll);
        //call Player's Buy method when 'BuyButton' is clicked
        GameObject.FindGameObjectWithTag("BuyButton").GetComponent<Button>().onClick.AddListener(
            delegate { 
                GameManager.CurrentPlayer.Purchase();
                GameManager.UpdateBuyButtonInteractibility();
                GameManager.UpdateAuctionButtonInteractibility();
                GameManager.UpdateNextPlayerButtonInteractibility();
                UpdateInventoryData();
            });

        //methods to call when Player needs to pay
        GameObject.FindGameObjectWithTag("PayButton").GetComponent<Button>().onClick.AddListener(
            delegate {
                Property property = GameManager.CurrentPlayer.Position.GetComponent<Property>();
                if (property != null)
                {
                    ushort payment = property.PaymentPrice();
                    GameManager.CurrentPlayer.RemoveFunds(payment);
                    property.Owner.AddFunds(payment);
                    SwitchToMenuWithInventory(EndOfTurnOptions);
                }
            });

        //methods to call when Player auctions an unowned property
        GameObject.FindGameObjectWithTag("AuctionButton").GetComponent<Button>().onClick.AddListener(
            delegate {
                GameObject.FindGameObjectWithTag("AuctionButton").GetComponent<Button>().interactable = false;
                GameManager.UpdateBuyButtonInteractibility();
                GameManager.UpdateNextPlayerButtonInteractibility();                                                   //update this when Trading System is implemented
                UpdateInventoryData();
            });

        GameObject.FindGameObjectWithTag("NextTurnButton").GetComponent<Button>().onClick.AddListener(
            delegate {
                GameManager.NextPlayer();
            });

        //methods to call when player bankrupts
        GameObject.FindGameObjectWithTag("LoseButton").GetComponent<Button>().onClick.AddListener(
            delegate {
                //remove all properties and cards from player
                Player lostPlayer = GameManager.CurrentPlayer;
                lostPlayer.Reset();
                GameManager.NextPlayer();
                GameManager.RemoveActivePlayer(lostPlayer);
            });

        //toggle house mode
        Button buildHouseButton = GameObject.FindGameObjectWithTag("BuildHouseButton").GetComponent<Button>();
        buildHouseButton.onClick.AddListener(
            delegate {
                _houseMode = !_houseMode;
                Button rollButton = GameObject.FindGameObjectWithTag("RollButton").GetComponent<Button>();
                Button tradeButton = GameObject.FindGameObjectWithTag("TradeButton").GetComponent<Button>();
                if (_houseMode)
                {
                    rollButton.interactable = false;
                    tradeButton.interactable = false;
                    buildHouseButton.GetComponentInChildren<Text>().text = "Back";
                }
                else
                {
                    rollButton.interactable = true;
                    tradeButton.interactable = true;
                    buildHouseButton.GetComponentInChildren<Text>().text = "Build House";
                }
                UpdateInventoryData();
            });

        SwitchToMenuWithInventory(TurnOptions);
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
        if (GameManager.CurrentPlayer != null)
        {
            ShowMenu(Inventory);
            GameObject.FindGameObjectWithTag("Balance").GetComponent<Text>().text = "$" + GameManager.CurrentPlayer.GetBalance();

            //get rid of previous cards
            GameObject[] prevPlayerCards = GameObject.FindGameObjectsWithTag("InventoryCard");
            foreach (GameObject obj in prevPlayerCards)
                GameObject.Destroy(obj);

            //create icons for each property in the player's inventory
            for (int i = 0; i < GameManager.CurrentPlayer.PropertiesOwned.Count; i++)
            {
                Property property = GameManager.CurrentPlayer.PropertiesOwned[i];
                GameObject cardObj = new GameObject("InventoryCard");
                Image card = cardObj.AddComponent<Image>();
                card.tag = "InventoryCard";
                card.transform.SetParent(Inventory.transform);
                card.rectTransform.sizeDelta = new Vector2(50, 100);
                card.rectTransform.localPosition = new Vector2(-430 + 80 * i, -260);

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

                Street street = property.GetComponent<Street>();

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
                        streetColour.rectTransform.localPosition = new Vector2(0f, 42f);
                        streetColour.color = new Color(street.Colour.r, street.Colour.g, street.Colour.b, 1);

                        title.rectTransform.localPosition = new Vector2(0f, 20f);

                        //grays out street if not able to build house in house mode
                        if (BuildHouseMode && !street.CanBuildHouse())
                            GrayOutImage(card);
                    }
                }
                else
                {
                    title.text = "MORTGAGED";
                    title.rectTransform.localPosition = Vector3.zero;
                }

                //grays out non-street properties if in house mode
                if (street == null && BuildHouseMode)
                    GrayOutImage(card);
            }

            //show usable cards in inventory
            for (int i = 0; i < GameManager.CurrentPlayer.UsableCards.Count; i++)
            {
                Card usableCard = GameManager.CurrentPlayer.UsableCards[i];
                GameObject cardObj = new GameObject("InventoryCard");
                Image card = cardObj.AddComponent<Image>();
                card.tag = "InventoryCard";
                card.transform.SetParent(Inventory.transform);
                card.rectTransform.sizeDelta = new Vector2(50, 100);
                card.rectTransform.localPosition = new Vector2(-430, 260 - 150 * i);

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
                    GrayOutImage(card);
            }
        }
    }

    private static void GrayOutImage(Image img)
    {
        Image grayOut = new GameObject("GrayOut").AddComponent<Image>();
        grayOut.transform.SetParent(img.transform);
        grayOut.transform.localPosition = Vector2.zero;
        grayOut.rectTransform.sizeDelta = img.rectTransform.sizeDelta;
        grayOut.color = new Color(0f, 0f, 0f, 0.5f);
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

    public static Canvas UsableCardInfo
    {
        get { return _usableCardInfo; }
    }

    public static Canvas LoseOptions
    {
        get { return _lose; }
    }

    public static bool BuildHouseMode
    {
        get { return _houseMode; }
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
}
