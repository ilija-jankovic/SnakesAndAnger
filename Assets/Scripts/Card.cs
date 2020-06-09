using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    //desription of card
    private string _description;
    //Player that picked up the card
    //protected Player _owner;

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
        get
        {
            foreach (Player player in GameManager.Players)
                if (player.UsableCards.Contains(this))
                    return player;
            return null;
        }
    }


    //Methods

    /// <summary>
    /// Uses the card, each card has a different use function
    /// </summary>
    public virtual void Use()
    {
        Owner.UsableCards.Remove(this);
        if (!(this is TileLink))
            ChanceDeck.PlaceUnderDeck(this);
    }

    /// <summary>
    /// gets the description of the card
    /// </summary>
    /// <returns></returns>
    public virtual string GetDescription()
    {
        return _description;
    }

    /// <summary>
    /// gets image that pops up beside text
    /// </summary>
    public virtual Sprite Icon
    {
        get { return null; }
    }

    public virtual Sprite BackIcon
    {
        get { return Resources.Load<Sprite>("communityChanceBackCover"); }
    }
}
