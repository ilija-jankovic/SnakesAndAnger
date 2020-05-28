using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

static class GameManager
{
    //stores players in the current game
    private static Player[] _players;
    //stores all Monopoly tiles in order from go to last
    private static Tile[] _tiles;
    private static Player _curPlayer;

    //when player needs to pay something, it is stored here until it is paid
    private static ushort _paymentNeeded;
    //if chance card is picked up, it is stored here until finished being used
    private static Card _activeCard;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        //order tiles
        _tiles = GameObject.FindObjectsOfType<Tile>();
        Array.Sort(_tiles, delegate (Tile t1, Tile t2) {
            return t1.name.CompareTo(t2.name);
        });

        

        //
        //remove this method call later
        //
        ResetBoard();

        //debugging
        CurrentPlayer.AddProperty(Tiles[1].GetComponent<Property>());
        CurrentPlayer.AddProperty(Tiles[3].GetComponent<Property>());
    }

    public static void ResetBoard()
    {
        InitialiseActivePlayers();
    }

    private static void InitialiseActivePlayers()
    {
        _players = GameObject.FindObjectsOfType<Player>();
        List<Player> playing = new List<Player>();

        //get active players
        foreach (Player player in _players)
            if (player.Playing)
                playing.Add(player);
            else
                player.GetComponent<Renderer>().enabled = false;                 //make this better

        //add active players to players array and reset their values
        _players = new Player[playing.Count];
        for (byte i = 0; i < playing.Count; i++)
        {
            Player player = playing[i];
            _players[i] = player;
            player.Reset();
        }

        //sets first player. Should probably randomise this later, initialises the camera also
        _curPlayer = _players[0];
        CameraFollow.target = _curPlayer.transform;
        MenuManager.UpdateInventoryData();
    }

    public static void RemoveActivePlayer(Player player)
    {
        if (!player.GetComponent<Renderer>().enabled)
            throw new MissingReferenceException("A player must be playing in order to be removed.");

        Player[] newPlayers = new Player[_players.Length - 1];

        //check for winner
        if(newPlayers.Length == 1)
        {
            MenuManager.SwitchToMenu(MenuManager.WinMenu);
            //clean up
            _players = new Player[0];
            _curPlayer = null;
            return;
        }

        //update player array
        int newIndex = 0;
        for(int i = 0; i < _players.Length; i++)
        {
            if (_players[i] == player)
            {
                player.GetComponent<Renderer>().enabled = false;
                continue;
            }
            newPlayers[newIndex] = _players[i];
            newIndex++;
        }

        _players = newPlayers;
    }

    public static void EndOfRollOptions()
    {
        CameraFollow.target = CurrentPlayer.transform;

        Tile tile = CurrentPlayer.Position;
        Property property = tile.GetComponent<Property>();
        ChanceTile chance = tile.GetComponent<ChanceTile>();
        //FreeParking parking = tile.GetComponent<FreeParking>();
        SandLTile sAndL = tile.GetComponent<SandLTile>();
        PaymentTile paymentTile = tile.GetComponent<PaymentTile>();
        if (property != null) 
        {
            //check if the player stepped on an unowned/mortgaged/their own property
            if (property.Owner == null || property.Owner == CurrentPlayer || (property.Mortgaged && property.Owner != CurrentPlayer))
                MenuManager.SwitchToMenuWithInventory(MenuManager.EndOfTurnOptions);
            else
                PlayerMustPay(property.PaymentPrice());

            //display current property
            MenuManager.UpdateCardInfo(CurrentPlayer.Position.GetComponent<Property>());
        }
        else if (chance != null || sAndL != null)
        {
            if(chance != null)
                _activeCard = ChanceDeck.DrawCard();
            else if(sAndL != null)
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                    _activeCard = new Snake();
                else
                    _activeCard = new Ladder();
            }

            //adds card to player's inventory
            CurrentPlayer.AddCard(ActiveCard);
            MenuManager.SwitchToMenuWithInventory(MenuManager.CardOptions);
            //pop up to show which card is drawn
            MenuManager.UpdateCardInfo(ActiveCard);
        }
        else if (paymentTile != null)
        {
            MenuManager.SwitchToMenuWithInventory(MenuManager.PaymentTileOptions);
        }
        //else if (parking != null)
        //{

        //}
        else
            MenuManager.SwitchToMenuWithInventory(MenuManager.EndOfTurnOptions);
    }

    public static void PlayerMustPay(ushort amount)
    {
        PlayerMustPay(amount, CurrentPlayer);
    }

    public static void PlayerMustPay(ushort amount, Player player)
    {
        //check if player can pay
        int funds = player.GetTotalPotentialBalance();
        _paymentNeeded = amount;
        if (funds >= amount)
        {
            MenuManager.SwitchToMenuWithInventory(MenuManager.PaymentOptions);
            MenuManager.UpdateInventoryData(player);
            //disable pay button so player must mortgage
            if (player.GetBalance() < amount)
            {
                GameObject.FindGameObjectWithTag("PayButton").GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            //player loses the game
            MenuManager.SwitchToMenu(MenuManager.LoseOptions);
        }
    }

    public static void UpdatePayButtonInteractibility()
    {
        if (MenuManager.PaymentOptions.enabled == true)
        {
            Button payButton = GameObject.FindGameObjectWithTag("PayButton").GetComponent<Button>();
            payButton.interactable = CurrentPlayer.GetBalance() >= PaymentNeeded;
        }
    }

    public static void UpdateBuyButtonInteractibility()
    {
        if (MenuManager.EndOfTurnOptions.enabled == true)
        {
            Button buyButton = GameObject.FindGameObjectWithTag("BuyButton").GetComponent<Button>();
            buyButton.interactable = false;
            //update this quick fix when the trading system is implemented
            if (GameObject.FindGameObjectWithTag("AuctionButton").GetComponent<Button>().interactable == false)
                return;

            Property property = CurrentPlayer.Position.GetComponent<Property>();
            if(property != null && property.Owner == null && CurrentPlayer.GetBalance() >= property.Price)
                buyButton.interactable = true;
        }
    }

    public static void UpdateAuctionButtonInteractibility()
    {
        if (MenuManager.EndOfTurnOptions.enabled == true)
        {
            Button auctionButton = GameObject.FindGameObjectWithTag("AuctionButton").GetComponent<Button>();
            auctionButton.interactable = false;
            Property property = CurrentPlayer.Position.GetComponent<Property>();
            if (property != null && property.Owner == null)
                auctionButton.interactable = true;
        }
    }

    public static void UpdateNextPlayerButtonInteractibility()
    {
        if (MenuManager.EndOfTurnOptions.enabled == true)
            GameObject.FindGameObjectWithTag("NextTurnButton").GetComponent<Button>().interactable = !GameObject.FindGameObjectWithTag("AuctionButton").GetComponent<Button>().interactable;
    }

    public static void NextPlayer()
    {
        _activeCard = null;
        MenuManager.SwitchToMenuWithInventory(MenuManager.TurnOptions);
        _curPlayer = _players[(Array.IndexOf(_players, _curPlayer) + 1) % _players.Length];
        //change the target for the camera to the current player
        CameraFollow.target = _curPlayer.transform;
        MenuManager.UpdateInventoryData();
    }

    public static Tile[] Tiles
    {
        get { return _tiles; }
    }

    public static Player CurrentPlayer
    {
        get { return _curPlayer; }
    }

    public static Player[] Players
    {
        get { return _players; }
    }

    public static ushort PaymentNeeded
    {
        get { return _paymentNeeded; }
    }

    public static Card ActiveCard
    {
        get { return _activeCard; }
    }

    //utitily method - could not find an appropriate class for so it is in GameManager
    public static Texture2D GetTextureFromSprite(string path)
    {
        var sprite = Resources.Load<Sprite>(path);
        var croppedTexture = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                (int)sprite.textureRect.y,
                                                (int)sprite.textureRect.width,
                                                (int)sprite.textureRect.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        return croppedTexture;
    }
}
