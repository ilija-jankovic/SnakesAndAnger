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

    public override Sprite Icon
    {
        get { return Resources.Load<Sprite>("ladderCardLandscape"); }
    }

    public override Sprite BackIcon
    {
        get { return Resources.Load<Sprite>("ladderCard"); }
    }
}
