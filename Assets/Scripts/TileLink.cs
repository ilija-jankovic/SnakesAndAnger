using System;

public abstract class TileLink : Card
{
    Tuple<Tile, Tile> tiles;
    public TileLink(string description, Tile t1, Tile t2) : base(description)
    {
        tiles = new Tuple<Tile, Tile>(t1, t2);
    }

    public Tile Head
    {
        get { return tiles.Item1; }
    }

    public Tile Tail
    {
        get { return tiles.Item2; }
    }
}
