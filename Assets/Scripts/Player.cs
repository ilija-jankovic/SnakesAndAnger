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
    /// <summary>
    /// stores the players account balance
    /// </summary>
    private int _playerBalance;
    /// <summary>
    /// stores all the properties owned by the player
    /// </summary>
    List<Property> _propertysOwned = new List<Property>();
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
        _propertysOwned.Add(p);
        p.ChangeOwner(this);
    }
    /// <summary>
    /// returns the players balance as a decimal
    /// </summary>
    /// <returns></returns>
    public decimal GetBalance()
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
        get { return _propertysOwned; }
    }
}
