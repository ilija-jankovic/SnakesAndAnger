using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : TileLink
{

    public Ladder(string description, Tile head, Tile tail) : base(description, head, tail)
    {
    }



    public void DisplaySnake()
    {
        //displays snake
    }

    public void MovePlayer()
    {
        if (_head.hasPlayer())
        {
            _head.Player.Move(_head);
        }

    }


}
