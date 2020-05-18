using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : TileLink
{
    /// <summary>
    /// Constructor for the Snake Class
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public Snake(string description, Tile head, Tile tail) : base(description, head, tail)
    {

    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }
}
