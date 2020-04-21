using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all Monopoly tiles.
public abstract class Tile : MonoBehaviour
{
    private TileLink _tileLink;
    //options to display when the player lands on this tile
    public abstract void DisplayOptions();

    //removes snake/ladder
    public void RemoveTileLink()
    {
        _tileLink = null;
    }

    //adds snake/ladder
    public bool AddTileLink(TileLink link)
    {
        bool canAdd = !HasTileLink();
        if(canAdd)
            _tileLink = link;
        return canAdd;
    }

    //checks if there already is a snake or ladder connected to tile
    public bool HasTileLink()
    {
        return _tileLink != null;
    }

    public TileLink TileLink
    {
        get { return _tileLink; }
    }

    /*
    public void MoveThroughTileLink(Player player)
    {
        if(HasTileLink())
            _tileLink.Move(player)
    }
    */
}
