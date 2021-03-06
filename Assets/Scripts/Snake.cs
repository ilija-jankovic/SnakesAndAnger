﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Snake : TileLink
{
    /// <summary>
    /// Constructor for the Snake Class
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public Snake()
    {

    }

    public override string GetDescription()
    {
        return "Snake can be placed up to " + _maxLength + " tiles.";
    }

    public override Sprite Icon
    {
        get { return Resources.Load<Sprite>("snakeCardLandscape"); }
    }

    public override Sprite BackIcon
    {
        get { return Resources.Load<Sprite>("snakeCard"); }
    }
}
