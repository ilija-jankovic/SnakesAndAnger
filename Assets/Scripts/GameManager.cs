using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GameManager
{
    //static Player[] players;
    static Property[] properties = GameObject.FindObjectsOfType<Property>();
    //static Die[] dice = GameObject.FindObjectsOfType<Die>();

    [RuntimeInitializeOnLoadMethod]
    static void Initialise()
    {

    }

    static void ResetBoard()
    {

    }

    static void FillPlayerList()
    {
        //players = GameObject.FindObjectsOfType<Player>();
        //List<Player> active = new List<Player>();
        //foreach(Player player in players)
        //  if(player.active)
        //      active.add(player);
        //players = new Player[active.Length];
        //for(byte i = 0; i < active.Length; i++)
        //  players[i] = active.get(i);
    }

    static void Turn(/*Player curPlayer*/)
    {

    }
}
