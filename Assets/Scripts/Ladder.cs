using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder
{
    private Tile _head;
    private Tile _tail;

    public Ladder(Tile head, Tile tail)
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
        if (_head.hasPlayer())
        {
            _head.Player.Move(_head);
        }

    }


}
