using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMove : Card
{
    //tile to move to
    Tile _move;
    //type of move
    byte _type;
    // type 1 = direct move,
    // type 2 = closest Utility
    // type 3 = train station
    // type 4 = Back three Tiles

    /// <summary>
    /// Constructor for cards that move player
    /// </summary>
    /// <param name="id">id of Card</param>
    /// <param name="does">Card description</param>
    /// <param name="move">Tile to move to</param>
    /// <param name="type">Type of movement</param>
    public CardMove(byte id, string does, Tile move, byte type) : base(id, does)
    {
        _type = type;
        _move = move;
    }

    public override void Use()
    {
        if (_type == 1) //direct movement
        {
            _owner.Move(_move);
        }
        else if (_type == 2) //move to closest Utility
        {
            //_owner.Move(CheckClosestUtility());
        }
        else if (_type == 3) //move to closest train station
        {
            //_owner.Move(CheckClosestStation());
        }
        else
        {
            //_owner.Move(Player.playerPosition - 3);
        }

    }

    //To Be Made
    //private Tile CheckClosestUtility()
    //{
    //    return Tile;
    //}
    //To Be Made
    //private Tile CheckClosestStation()
    //{
    //    return Tile;
    //}
}