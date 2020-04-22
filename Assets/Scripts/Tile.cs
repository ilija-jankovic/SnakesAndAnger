using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all Monopoly tiles.
public abstract class Tile : MonoBehaviour
{
    private TileLink _tileLink;
    private Player _player;
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

    //returns the player that is on this current tile
    public Player Player
    {
        get { return _player; }
    }

    //checks if there is a player on this tile
    public bool hasPlayer()
    {
        return _player == null;
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
