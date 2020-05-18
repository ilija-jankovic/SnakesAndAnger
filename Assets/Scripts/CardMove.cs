using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMove : Card
{
    //tile to move to
    Tile _move;
    //type of move
    byte _type;
    // type 1 = direct move,
    // type 2 = closest Utility
    // type 3 = train station
    // type 4 = Back three Tiles
    // type 5 = go to jail

    /// <summary>
    /// Constructor for cards that move player
    /// </summary>
    /// <param name="does">Card description</param>
    /// <param name="move">Tile to move to</param>
    /// <param name="type">Type of movement</param>
    public CardMove(string does, Tile move, byte type) : base(does)
    {
        _type = type;
        _move = move;
    }


    public CardMove(string does, byte type) : base(does)
    {
        _type = type;
    }

    public override void Use()
    {
        if (_type == 1) //direct movement
        {
            _owner.Move(_move);
        }
        else if (_type == 2) //move to closest Utility. tile12, tile28
        {
            AdvClosestUtility();
        }
        else if (_type == 3) //move to closest train station. tile35, tile25, tile15, tile05
        {
            AdvClosestStation();
        }
        else if (_type == 4)
        {
            int currPos = GetPlayerPosIndex();
            _owner.Move(GameManager.Tiles[currPos- 3]);
        }
        else // go to jail
        {

        }



    }

    /// <summary>
    /// Adcances to nearest Utility
    /// </summary>
    private void AdvClosestUtility()
    {
        int pos = GetPlayerPosIndex();
        bool fordFound = false;
        while(!fordFound)
        {
            pos++;
            if(pos > 39)
            {
                pos = 0;
            }
            if(GameManager.Tiles[pos] is Utility)
            {
                fordFound = true;
                _owner.Move(GameManager.Tiles[pos]);
            }

        }
    }

    /// <summary>
    /// Advances to nearest Station
    /// </summary>
    private void AdvClosestStation()
    {
        int pos = GetPlayerPosIndex();
        bool fordFound = false;
        while (!fordFound)
        {
            pos++;
            if (pos > 39)
            {
                pos = 0;
            }
            if (GameManager.Tiles[pos] is RailwayStation)
            {
                fordFound = true;
                _owner.Move(GameManager.Tiles[pos]);
            }

        }

    }

    /// <summary>
    /// Gets the index of the playerPosition rather than the tile
    /// </summary>
    /// <returns>PlayerPosIndex</returns>
    private int GetPlayerPosIndex()
    {
        return System.Array.IndexOf(GameManager.Tiles, _owner.Position);
    }
}