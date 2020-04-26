using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GameManager
{
    static Player[] players;
    static Player curPlayer;
    static Property[] properties = GameObject.FindObjectsOfType<Property>();
    static Die[] dice = GameObject.FindObjectsOfType<Die>();

    [RuntimeInitializeOnLoadMethod]
    static void Initialise()
    {
        MenuManager.DisplayMainMenu();
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
