using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GameManager
{
    static Player[] players;
    static Player curPlayer;
    //stores all Monopoly tiles in order from go to last
    static Tile[] tiles;
    static Die[] dice = GameObject.FindObjectsOfType<Die>();

    [RuntimeInitializeOnLoadMethod]
    static void Initialise()
    {
        MenuManager.DisplayMainMenu();
        Tile[] unsortedTiles = GameObject.FindObjectsOfType<Tile>();
        Array.Sort(unsortedTiles, delegate (Tile t1, Tile t2) { return t1.name.CompareTo(t2.name); });
    }

    static void ResetBoard()
    {

    }

    static void FillPlayerList()
    {
        players = GameObject.FindObjectsOfType<Player>();
        List<Player> playing = new List<Player>();
        foreach(Player player in players)
          if(player.Playing)
              playing.Add(player);
        players = new Player[playing.Count];
        for(byte i = 0; i < playing.Count; i++)
          players[i] = playing[i];
    }

    static void Turn()
    {

    }
}
