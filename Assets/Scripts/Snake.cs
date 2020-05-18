using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : TileLink
{
    private Tile _head;
    private Tile _tail;

    /// <summary>
    /// Constructor for the Snake Class
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public Snake(Tile head, Tile tail)
    {
        _head = head;
        _tail = tail;
    }

    /// <summary>
    /// Get and Set property for the Head.
    /// </summary>
    public Tile Head
    {
        get { return _head; }
        set { _head = value; }
    }

    /// <summary>
    /// Get and Set property for the Tail
    /// </summary>
    public Tile Tail
    {
        get { return _tail; }
        set { _tail = value; }
    }
    
    /// <summary>
    /// This is to display the Snake on the board.
    /// </summary>
    public void DisplaySnake()
    {
        //displays snake
    }

    public void MovePlayer()
    {
        if(_head.hasPlayer())
        {
            _head.Player.Move(_tail);
        }

    }

    
}
