using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

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
                player.GetComponent<Renderer>().enabled = false;                 //make this better

        //add active players to players array and reset their values
        _players = new Player[playing.Count];
        for (byte i = 0; i < playing.Count; i++)
        {
            Player player = playing[i];
            _players[i] = player;
            player.Reset();
        }

        //sets first player. Should probably randomise this later, initialises the camera also
        _curPlayer = _players[0];
        CameraFollow.target = _curPlayer.transform;
        MenuManager.UpdateInventoryData();
    }

    private static void RemoveActivePlayer(Player player)
    {
        if (!player.GetComponent<Renderer>().enabled)
            throw new MissingReferenceException("A player must be playing in order to be removed.");

        Player[] newPlayers = new Player[_players.Length - 1];

        //check for winner
        if(newPlayers.Length == 1)
        {
            MenuManager.SwitchToMenu(MenuManager.WinMenu);
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

    public static void EndOfRollOptions()
    {
        Property property = CurrentPlayer.Position.GetComponent<Property>();
        //check if the player stepped on an unowned property
        if (property != null && property.Owner == null)
        {
            MenuManager.SwitchToMenuWithInventory(MenuManager.EndOfTurnOptions);
            MenuManager.ShowMenu(MenuManager.CardInfo);
            GameObject.FindGameObjectWithTag("PropertyInfo").GetComponent<Text>().text = CurrentPlayer.Position.GetComponent<Property>().Description();
            GameObject.FindGameObjectWithTag("PropertyTitle").GetComponent<Text>().text = CurrentPlayer.Position.GetComponent<Property>().Title;

            //set colour of card
            Image streetColour = GameObject.FindGameObjectWithTag("StreetColour").GetComponent<Image>();
            Street street = property.GetComponent<Street>();
            if (street != null)
                streetColour.color = new Color(street.Colour.r,street.Colour.g,street.Colour.b,1);
            else
                streetColour.color = Vector4.zero;
        }
        else
        {
            //add player payment in here if player steps on someone's property
            NextPlayer();
        }
    }

    public static void NextPlayer()
    {
        MenuManager.SwitchToMenuWithInventory(MenuManager.TurnOptions);
        _curPlayer = _players[(Array.IndexOf(_players, _curPlayer) + 1) % _players.Length];
        //change the target for the camera to the current player
        CameraFollow.target = _curPlayer.transform;
        MenuManager.UpdateInventoryData();
    }

    public static Tile[] Tiles
    {
        get { return _tiles; }
    }

    public static Player CurrentPlayer
    {
        get { return _curPlayer; }
    }

    public static Player[] Players
    {
        get { return _players; }
    }
}
