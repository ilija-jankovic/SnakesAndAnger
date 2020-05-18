﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceTile : Tile
{
    GameObject _tileMesh;
    private Color _colour =  Color.blue;

    private static ChanceDeck _chance;

    public void Awake()
    {
        _tileMesh = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _tileMesh.transform.parent = transform;
        _tileMesh.transform.localScale = new Vector3(transform.localScale.x / 100, 1, transform.localScale.z / 160);
        _tileMesh.transform.localEulerAngles = Vector3.zero;
        _tileMesh.transform.localPosition = new Vector3(0, 0.55f, 0.5f - 5 * _tileMesh.transform.localScale.z);
        
        _tileMesh.GetComponent<Renderer>().material.color = _colour;

        //creates deck of chance cards
        _chance = new ChanceDeck();
    }

    public static ChanceDeck ChanceCards
    {
        get { return _chance; }
    }

    public override void DisplayOptions()
    {
        throw new NotImplementedException();
    }
}
