using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : TileLink
{

    public Ladder()
    {
    }

    public override string GetDescription()
    {
        return "Ladder can be placed up to " + _maxLength + " tiles.";
    }

    public override Texture2D Icon
    {
        get { return GameManager.GetTextureFromSprite("ladder"); }
    }
}
