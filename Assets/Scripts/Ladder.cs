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

    public override void Use()
    {
        throw new System.NotImplementedException();
    }
}
