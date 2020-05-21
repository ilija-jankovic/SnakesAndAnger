using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// stores the current position of the player
    /// </summary>
    private Tile _playerPosition;
    [SerializeField]
    /// <summary>
    /// stores the players account balance
    /// </summary>
    private int _playerBalance;
    /// <summary>
    /// stores the players chance cards
    /// </summary>
    List<Card> _cards = new List<Card>();
    /// <summary>
    /// card that is being used - this for for use in MenuManager
    /// </summary>
    private Card cardInHand = null;
    /// <summary>
    /// stores all the properties owned by the player
    /// </summary>
    List<Property> _propertiesOwned;
    [SerializeField]
    /// <summary>
    /// flags whether player is in the current game
    /// </summary>
    private bool _playing;
    /// <summary>
    /// creates a player object
    /// </summary>
    /// <param name="playerPosition"></param>
    public Player (Tile playerPosition)
    {
        _playerPosition = playerPosition;
    }
    /// <summary>
    /// moves the player to the specified tile
    /// </summary>
    /// <param name="endPosition"></param>
    public void Move(Tile endPosition)
    {
        _playerPosition = endPosition;
        if (_playerPosition.TileLink != null && _playerPosition.TileLink.Head == _playerPosition)
            _playerPosition = endPosition.TileLink.Tail;

        //graphical representation
        transform.position = _playerPosition.transform.position;
    }
    public void Move(sbyte tiles)
    {
        Move(GameManager.Tiles[(Array.IndexOf(GameManager.Tiles, _playerPosition) + tiles) % GameManager.Tiles.Length]);
    }
    /// <summary>
    /// adds funds to the player
    /// </summary>
    /// <param name="Amount"></param>
    public void AddFunds(ushort Amount)
    {
        _playerBalance += Amount;
    }
    /// <summary>
    /// removes funds from the player
    /// </summary>
    /// <param name="Amount"></param>
    public void RemoveFunds(ushort Amount)
    {
        _playerBalance -= Amount;
    }
    /// <summary>
    /// adds a property to the players list of properties
    /// </summary>
    /// <param name="p"></param>
    public void AddProperty(Property p)
    {
        //remove property from other players
        foreach (Player player in GameManager.Players)
            player.PropertiesOwned.Remove(p);

        PropertiesOwned.Add(p);

        //sort from lowest value to highest value properties
        PropertiesOwned.Sort(delegate (Property p1, Property p2) {
            return p1.name.CompareTo(p2.name);
        });
    }

    public void Purchase()
    {
        if (_playerPosition != null)
        {
            Property property = _playerPosition.GetComponent<Property>();
            if (property != null && property.Owner == null)
                if (_playerBalance >= property.Price)
                {
                    RemoveFunds(property.Price);
                    AddProperty(property);
                }
        }
    }
    /// <summary>
    /// returns the players balance as a decimal
    /// </summary>
    /// <returns></returns>
    public int GetBalance()
    {
        return _playerBalance;
    }
    /// <summary>
    /// returns whether the player is in the current game as a boolean
    /// </summary>
    public bool Playing
    {
        get { return _playing; }
    }
    /// <summary>
    /// returns a list of properties that the player owns
    /// </summary>
    public List<Property> PropertiesOwned
    {
        get { return _propertiesOwned; }
    }

    public Tile Position
    {
        get { return _playerPosition; }
    }
    public void Reset()
    {
        _playerPosition = GameManager.Tiles[0];
        transform.position = GameManager.Tiles[0].transform.position;
        _playerBalance = 1500;

        //return properties to the bank
        foreach (Property property in PropertiesOwned)
            property.ReturnToBank();
        _propertiesOwned = new List<Property>();

        //debugging
        _cards = new List<Card>() { new Ladder(), new Snake() };
    }

    public int GetTotalPotentialBalance()
    {
        int funds = GetBalance();
        foreach (Property playerProp in PropertiesOwned)
            if (!playerProp.Mortgaged)
                funds += playerProp.MortgageValue;
        return funds;
    }

    /// <summary>
    /// adds card to players hand, uses immediately depending on card
    /// </summary>
    /// <param name="c"></param>
    public void AddCard(Card c)
    {
        _cards.Add(c);

    if(c is CardCollect || c is CardMove || c is CardPay)
        {
            UseCard(c);

        }
    }

    /// <summary>
    /// uses the card passed in
    /// </summary>
    /// <param name="c2"></param>
    public void UseCard(Card c2)
    {
        cardInHand = c2;
        c2.Use();
        _cards.Remove(c2);
        ChanceDeck.PlaceUnderDeck(c2);
    }

    /// <summary>
    /// gives desired card to another player.
    /// </summary>
    /// <param name="c3">card to give</param>
    /// <param name="p1">player to give to</param>
    public void GiveCard(Card c3, Player p1)
    {
        _cards.Remove(c3);
        p1.AddCard(c3);
    }

    public List<Card> UsableCards
    {
        get { return _cards; }
    }

    //initialise lists
    public void Awake()
    {
        _propertiesOwned = new List<Property>();
        _cards = new List<Card>();
    }

    public Card CardInHand
    {
        get { return cardInHand; }
    }

}
