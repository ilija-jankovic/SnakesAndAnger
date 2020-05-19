using System;
using UnityEngine;

public abstract class TileLink : Card
{
    Tuple<Tile, Tile> tiles;
    protected byte _maxLength;
    public TileLink() : base(null)
    {
        _maxLength = (byte)UnityEngine.Random.Range(2, 10);
    }

    public Tile Head
    {
        get { return tiles.Item1; }
    }

    public Tile Tail
    {
        get { return tiles.Item2; }
    }

    public byte MaxLength
    {
        get { return _maxLength; }
    }
}
