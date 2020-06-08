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
    /// stores all the properties owned by the player
    /// </summary>
    List<Property> _propertiesOwned;
    /// <summary>
    /// flags whether player is in the current game
    /// </summary>
    private bool _playing;
    /// <summary>
    /// flags whether the player is moving
    /// </summary>
    private bool _isMoving;
    /// <summary>
    /// stores where the player is moving to
    /// </summary>
    Vector3 pos;
    public Transform[] _target;
    int targetcount = 0;
    private float speed = 25f;
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
        //pos = Vector3.MoveTowards(transform.position, endPosition.transform.position, 1.5f * Time.deltaTime);
        _isMoving = true;
        int start = Array.IndexOf(GameManager.Tiles, _playerPosition);
        int end = Array.IndexOf(GameManager.Tiles, endPosition);
        //add all the tiles on the players path to an array of transform objects
        if (start < end)
        {
            int count = 0;
            _target = new Transform[end - start];
            for (int i = start + 1; i <= end; i++)
            {
                _target[count] = GameManager.Tiles[i].transform;
                count++;
            }
        }
        else if(end < start)
        {
            int count = 0;
            _target = new Transform[(end + GameManager.Tiles.Length) - start];
            for (int j = start + 1; j < GameManager.Tiles.Length; j++)
            {
                _target[count] = GameManager.Tiles[j].transform;
                count++;
            }
            for(int k = 0; k <= end; k++)
            {
                _target[count] = GameManager.Tiles[k].transform;
                count++;
            }
        }
        bool traveledByTileLink = false;
        _playerPosition = endPosition;
        //if there is a snake or ladder the player will move along it
        _playerPosition.MoveAlongTileLink();

        //graphical representation

        //Collect $200 if you pass go
        if (!traveledByTileLink || !Jail.InJail())
        {
            int position = start;
            while (position != 0)
            {
                position++;
                if (position > 39)
                {
                    position = 0;
                    this.AddFunds(200);
                }
                if (position == end)
                {
                    position = 0;
                }

            }
        }
    }
    public void Move(sbyte tiles)
    {
        Move(GameManager.Tiles[(Array.IndexOf(GameManager.Tiles, _playerPosition) + tiles) % GameManager.Tiles.Length]);
    }
    void Update()
    {
        if(_isMoving)
        {
            if (targetcount < _target.Length)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, _target[targetcount].position, 25f * Time.deltaTime);
                transform.position = pos;
                if (transform.position == _target[targetcount].position)
                {
                    targetcount++;
                }
            }
            else
            {
                transform.position = _playerPosition.transform.position;
                _target = null;
                targetcount = 0;
                _isMoving = false;
            }
        }
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

    public void RemoveProperty(Property p)
    {
        PropertiesOwned.Remove(p);
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
        set { _playing = value; }
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
            {
                funds += playerProp.MortgageValue;
                Street street = playerProp.GetComponent<Street>();
                funds += street != null ? street.Houses * street.SellHousePrice : 0;
            }
        return funds;
    }

    /// <summary>
    /// adds card to players hand, uses immediately depending on card
    /// </summary>
    /// <param name="c"></param>
    public void AddCard(Card c)
    {
        _cards.Add(c);
    }

    public void RemoveCard(Card c)
    {
        _cards.Remove(c);
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
}
