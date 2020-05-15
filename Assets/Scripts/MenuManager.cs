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
    //make this at some point
    private static Canvas _winMenu;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        allMenus = GameObject.FindObjectsOfType<Canvas>();

        //call Roll method in Die class when 'RollButton' is clicked
        GameObject.FindGameObjectWithTag("RollButton").GetComponent<Button>().onClick.AddListener(Die.Roll);
        //call Player's Buy method when 'BuyButton' is clicked
        GameObject.FindGameObjectWithTag("BuyButton").GetComponent<Button>().onClick.AddListener(delegate { GameManager.CurrentPlayer.Purchase(); });

        SwitchToMenuWithInventory(TurnOptions);
    }

    private static void DisableAllMenus()
    {
        foreach (Canvas canvas in allMenus)
            canvas.enabled = false;
    }

    public static void SwitchToMenu(Canvas menu)
    {
        DisableAllMenus();
        foreach (Canvas canvas in allMenus)
            if (canvas == menu)
                menu.enabled = true;
    }

    public static void SwitchToMenuWithInventory(Canvas menu)
    {
        SwitchToMenu(menu);
        Inventory.enabled = true;
    }

    public static void ShowMenu(Canvas menu)
    {
        menu.enabled = true;
    }

    public static void UpdateInventoryData()
    {
        if (GameManager.CurrentPlayer != null && Inventory.enabled == true)
        {
            GameObject.FindGameObjectWithTag("Balance").GetComponent<Text>().text = "$" + GameManager.CurrentPlayer.GetBalance();

            //get rid of previous players inventory
            GameObject[] prevPlayerCards = GameObject.FindGameObjectsWithTag("InventoryCard");
            foreach (GameObject obj in prevPlayerCards)
                GameObject.Destroy(obj);

            for(int i = 0; i < GameManager.CurrentPlayer.PropertiesOwned.Count; i++)
            {
                Property property = GameManager.CurrentPlayer.PropertiesOwned[i];
                GameObject cardObj = new GameObject("InventoryCard");
                Image card = cardObj.AddComponent<Image>();
                card.tag = "InventoryCard";
                card.transform.SetParent(Inventory.transform);
                card.rectTransform.sizeDelta = new Vector2(50, 100);
                card.rectTransform.localPosition = new Vector2(-430 + 80*i, -260);

                cardObj.AddComponent<InventoryCardMouseInputUI>().property = property;

                Text title = new GameObject().AddComponent<Text>();
                title.text = property.Title;
                title.fontSize = 6;
                title.color = Color.black;
                title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                title.alignment = TextAnchor.UpperCenter;
                title.transform.SetParent(card.transform);
                title.rectTransform.sizeDelta = new Vector2(card.rectTransform.sizeDelta.x * 4 / 5, card.rectTransform.sizeDelta.y / 6);
                title.rectTransform.localPosition = new Vector2(0f, 35f);

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

    public static Canvas WinMenu
    {
        get { return _winMenu; }
    }

    public static Canvas Inventory
    {
        get { return _inventory; }
    }
}
