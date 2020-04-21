using System;

public abstract class TileLink
{
    Tuple<Tile, Tile> tiles;
    public TileLink(Tile t1, Tile t2)
    {
        tiles = new Tuple<Tile, Tile>(t1, t2);
    }
}
