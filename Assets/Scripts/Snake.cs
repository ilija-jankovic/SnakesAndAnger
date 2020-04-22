using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake 
{
    private Tile _head;
    private Tile _tail;

    public Snake(Tile head, Tile tail)
    {
        _head = head;
        _tail = tail;
    }

    public Tile Head
    {
        get { return _head; }
        set { _head = value; }
    }

    public Tile Tail
    {
        get { return _tail; }
        set { _tail = value; }
    }
    
    public void DisplaySnake()
    {
        //displays snake
    }

    public void MovePlayer()
    {
        if(_head.hasPlayer())
        {
        player.Move(_tail);
        }

    }

    
}
