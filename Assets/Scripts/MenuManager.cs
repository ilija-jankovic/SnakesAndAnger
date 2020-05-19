using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class MenuManager
{
    private static Canvas[] allMenus;
    private static Canvas _startOfTurn = GameObject.Find("TurnOptions").GetComponent<Canvas>();
    private static Canvas _endOfTurn = GameObject.Find("EndOfTurnOptions").GetComponent<Canvas>();
    private static Canvas _cardInfo = GameObject.Find("CardInfo").GetComponent<Canvas>();
    private static Canvas _inventory = GameObject.Find("Inventory").GetComponent<Canvas>();
    private static Canvas _payment = GameObject.Find("PaymentOptions").GetComponent<Canvas>();
    //make this at some point
    private static Canvas _winMenu;

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
                if(menu == EndOfTurnOptions)
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

                    Street street = property.GetComponent<Street>();
                    if (street != null)
                    {
                        Image streetColour = new GameObject("StreetColour").AddComponent<Image>();
                        streetColour.transform.SetParent(card.transform);
                        streetColour.rectTransform.sizeDelta = new Vector2(card.rectTransform.sizeDelta.x, card.rectTransform.sizeDelta.y / 6);
                        streetColour.rectTransform.localPosition = new Vector2(0f, 42f);
                        streetColour.color = new Color(street.Colour.r, street.Colour.g, street.Colour.b, 1);

                        title.rectTransform.localPosition = new Vector2(0f, 20f);
                    }
                }
                else
                {
                    title.text = "MORTGAGED";
                    title.rectTransform.localPosition = Vector3.zero;
                }
            }
           
        }
    }

    public static Canvas TurnOptions
    {
        get { return _startOfTurn; }
    }

    public static Canvas EndOfTurnOptions
    {
        get { return _endOfTurn; }
    }

    public static Canvas CardInfo
    {
        get { return _cardInfo; }
    }

    public static void UpdateCardInfo(Property property)
    {
        ShowMenu(CardInfo);
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
            DisableMenu(CardInfo);
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
