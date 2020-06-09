using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class AuctionSystem{
    public const short TIME_GIVEN_FOR_BIDS = 600;
    public static short timer;

    private static Player highestBidder;
    private static ushort _reservePrice;

    private static Canvas auctionOptions = GameObject.Find("AuctionOptions").GetComponent<Canvas>();
    private static GameObject propertyInfo;
    private static GameObject[] prefabs;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        prefabs = new GameObject[8];
        prefabs[0] = GameObject.Find("AuctionPlayerBalance");
        GameObject[] moneyButtons = GameObject.FindGameObjectsWithTag("AddToAuctionButton");
        for (byte i = 1; i < 7; i++)
            prefabs[i] = moneyButtons[i - 1];
        prefabs[7] = GameObject.Find("AuctionPlayerProperties");

        //disable prefabs
        foreach (GameObject obj in prefabs)
            obj.SetActive(false);

        propertyInfo = GameObject.Find("AuctionPropertyPrice");
    }

    public static void StartAuction()
    {
        MenuManager.SwitchToMenu(auctionOptions);
        MenuManager.UpdateCardInfo(Property);

        short gap = 150;

        //make buttons and add listeners
        for (byte i = 0; i < GameManager.Players.Length; i++){
            Player player = GameManager.Players[i];
            for (byte j = 0; j < prefabs.Length; j++)
            {
                GameObject prefab = prefabs[j];
                GameObject newObj = GameObject.Instantiate(prefab, prefab.transform.parent);
                newObj.SetActive(true);
                newObj.tag = "Temp";
                newObj.transform.position = new Vector3(prefab.transform.position.x, prefab.transform.position.y - gap * i);

                if (j == 0)
                {
                    newObj.GetComponent<Image>().color = Color.white;
                    newObj.transform.GetComponentInChildren<Text>().text = player.gameObject.name + "\n$" + player.GetBalance();
                }
                else if (j <= 6)
                {
                    string bidText = newObj.GetComponentInChildren<Text>().text;
                    newObj.GetComponent<Button>().onClick.AddListener(delegate { Bid(player, byte.Parse(bidText.Substring(1, bidText.Length - 1))); });
                }
                else
                    newObj.name = player.gameObject.name + "AuctionPanel";
            }
        }

        Bid(GameManager.CurrentPlayer, 0);
    }

    public static void Bid(Player player, byte amount)
    {
        highestBidder = player;
        _reservePrice += amount;
        timer = TIME_GIVEN_FOR_BIDS;

        //destroy previous inventory cards
        foreach (GameObject cardObj in GameObject.FindGameObjectsWithTag("InventoryCard"))
            GameObject.Destroy(cardObj);

        foreach (Player playerInAuction in GameManager.Players)
        {
            //find where to put row of properties
            GameObject panel = null;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Temp"))
                if (obj.name == playerInAuction.gameObject.name + "AuctionPanel")
                {
                    panel = obj;
                    break;
                }
            //display properties
            MenuManager.CreateRow(panel.GetComponent<RectTransform>(), playerInAuction.PropertiesOwned);
        }
    }

    public static void Update()
    {
        if (timer >= 0)
            timer--;
        if(timer == 0)
        {
            highestBidder.AddProperty(Property);

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Temp"))
                GameObject.Destroy(obj);
            //prevent memory leaks
            Resources.UnloadUnusedAssets();

            GameManager.PlayerMustPay(ReservePrice, highestBidder);
            _reservePrice = 0;
        }

        propertyInfo.GetComponentInChildren<Text>().text = timer + "\n$" + ReservePrice;
    }

    public static Property Property
    {
        get { return GameManager.CurrentPlayer.Position.GetComponent<Property>(); }
    }

    public static ushort ReservePrice
    {
        get { return _reservePrice; }
    }
}
