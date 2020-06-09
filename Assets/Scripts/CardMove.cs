using static EnumsForCards.cardMove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMove : Card
{
    //tile to move to
    Tile _move;
    //type of cardMove, found inside class EnumsForCards
    EnumsForCards.cardMove _type;

    /// <summary>
    /// Constructor for cards that move player
    /// </summary>
    /// <param name="does">Card description</param>
    /// <param name="move">Tile to move to</param>
    /// <param name="type">Type of movement</param>
    public CardMove(string does, Tile move, EnumsForCards.cardMove type) : base(does)
    {
        _type = type;
        _move = move;
    }


    public CardMove(string does, EnumsForCards.cardMove type) : base(does)
    {
        _type = type;
    }

    public override void Use()
    {
        if (_type == EnumsForCards.cardMove.directMove) //direct movement
        {
            Owner.Move(_move);
        }
        else if (_type == EnumsForCards.cardMove.closestUtility) //move to closest Utility. tile12, tile28
        {
            AdvClosestUtility();
        }
        else if (_type == EnumsForCards.cardMove.closestTrainStation) //move to closest train station. tile35, tile25, tile15, tile05
        {
            AdvClosestStation();
        }
        else if (_type == EnumsForCards.cardMove.moveBackThreeTiles)
        {
            int currPos = GetPlayerPosIndex();
            Owner.Move(GameManager.Tiles[currPos- 3]);
        }
        else // go to jail
        {
            Jail.GoToJail();
        }
        base.Use();
        GameManager.EndOfRollOptions();
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
                Owner.Move(GameManager.Tiles[pos]);
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
                Owner.Move(GameManager.Tiles[pos]);
            }

        }

    }

    /// <summary>
    /// Gets the index of the playerPosition rather than the tile
    /// </summary>
    /// <returns>PlayerPosIndex</returns>
    private int GetPlayerPosIndex()
    {
        return System.Array.IndexOf(GameManager.Tiles, Owner.Position);
    }

    public EnumsForCards.cardMove Type
    {
        get { return _type; }
    }

    public override Sprite Icon
    {
        get 
        { 
            if(_type != EnumsForCards.cardMove.goToJail)
                return Resources.Load<Sprite>("movementCard");
            return Resources.Load<Sprite>("goToJailCard");
        }
    }
}