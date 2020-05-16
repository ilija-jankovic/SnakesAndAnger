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
        if (property != null)
        {
            //check if the player stepped on an unowned property
            if (property.Owner == null)
                MenuManager.SwitchToMenuWithInventory(MenuManager.EndOfTurnOptions);
            else
            {
                //check if player can pay
                int funds = CurrentPlayer.GetTotalPotentialBalance();
                ushort propertyPayment = property.PaymentPrice();
                if (funds >= propertyPayment)
                {
                    MenuManager.SwitchToMenuWithInventory(MenuManager.PaymentOptions);
                    //disable pay button so player must mortgage
                    if (CurrentPlayer.GetBalance() < propertyPayment)
                        GameObject.FindGameObjectWithTag("PayButton").GetComponent<Button>().enabled = false;
                }
                else
                {
                    //player loses the game
                }
            }
            MenuManager.UpdateCardInfo(CurrentPlayer.Position.GetComponent<Property>());
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
