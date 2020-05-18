using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    //desription of card
    private string _description;
    //Player that picked up the card
    protected Player _owner;

    /// <summary>
    /// constructor for Card
    /// </summary>
    /// <param name="does">Card description</param>
    public Card(string does)
    {
        _description = does;
    }

    public Player Owner
    {
        get { return _owner; }
        //For making owner null when card is put back in deck
        set { _owner = value; }
    }


    //Methods

    /// <summary>
    /// Give card to another player
    /// </summary>
    /// <param name="player">player to give to</param>
    public void GiveCard(Player player)
    {
        _owner = player;
    }

    /// <summary>
    /// Uses the card, each card has a different use function
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// gets the description of the card
    /// </summary>
    /// <returns></returns>
    public string GetDescription()
    {
        return _description;
    }

}
