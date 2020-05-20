using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : TileLink
{

    public Ladder()
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

    public override string GetDescription()
    {
        return "Ladder can be placed up to " + _maxLength + " tiles.";
    }

    public override Texture2D Icon
    {
        get { return GetTextureFromSprite("ladder"); }
    }
}
