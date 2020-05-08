using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

static class GameManager
{
    //stores players in the current game
    private static Player[] _players;
    //stores all Monopoly tiles in order from go to last
    private static Tile[] _tiles;

    private static Player _curPlayer;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        MenuManager.DisplayMainMenu();

        //order tiles
        _tiles = GameObject.FindObjectsOfType<Tile>();
        Array.Sort(_tiles, delegate (Tile t1, Tile t2) {
            return t1.name.CompareTo(t2.name);
        });

        //
        //remove this method call later
        //
        ResetBoard();
    }

    public static void ResetBoard()
    {
        InitialiseActivePlayers();
    }

    private static void InitialiseActivePlayers()
    {
        _players = GameObject.FindObjectsOfType<Player>();
        List<Player> playing = new List<Player>();

        //get active players
        foreach (Player player in _players)
            if (player.Playing)
                playing.Add(player);
            else
                player.GetComponent<Renderer>().enabled = false;

        //add active players to players array and reset their values
        _players = new Player[playing.Count];
        for (byte i = 0; i < playing.Count; i++)
        {
            Player player = playing[i];
            _players[i] = player;
            player.Reset();
        }

        //sets first player. Should probably randomise this later
        _curPlayer = _players[0];
    }

    private static void RemoveActivePlayer(Player player)
    {
        if (!player.GetComponent<Renderer>().enabled)
            throw new MissingReferenceException("A player must be playing in order to be removed.");

        Player[] newPlayers = new Player[_players.Length - 1];

        //check for winner
        if(newPlayers.Length == 1)
        {
            MenuManager.DisplayWinMenu(player);
            //clean up
            _players = new Player[0];
            _curPlayer = null;
            return;
        }

        //update player array
        int newIndex = 0;
        for(int i = 0; i < _players.Length; i++)
        {
            if (_players[i] == player)
            {
                player.GetComponent<Renderer>().enabled = false;
                continue;
            }
            _players[newIndex] = _players[i];
            newIndex++;
        }
    }

    public static void NextPlayer()
    {
        _curPlayer = _players[(Array.IndexOf(_players, _curPlayer) + 1) % _players.Length];
    }

    public static Tile[] Tiles
    {
        get { return _tiles; }
    }

    public static Player CurrentPlayer
    {
        get { return _curPlayer; }
    }
}
