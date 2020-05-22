using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandLTile : Tile
{
    GameObject _tileMesh;
    private Color _colour = Color.red;

    public override void Awake()
    {
        base.Awake();
    }
}
