using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Jail
{

    //List of player in Jail
    private static List<Player> _playersInJail;
    
    /// <summary>
    /// Method to Initialise Jail
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void InitialiseJail()
    {
        _playersInJail = new List<Player>();
    }

    /// <summary>
    /// Returns list of players in Jail
    /// </summary>
    public static List<Player> PlayersInJail
    {
        get { return _playersInJail; }
    }

    /// <summary>
    /// Adds player into Jail List
    /// </summary>
    /// <param name="player"></param>
    public static void GoToJail()
    { 
        _playersInJail.Add(GameManager.CurrentPlayer);
        GameManager.CurrentPlayer.Move(GameManager.Tiles[10]);
    }

    /// <summary>
    /// Removes player from Jail List
    /// </summary>
    public static void LeaveJail()
    {
        _playersInJail.Remove(GameManager.CurrentPlayer);
    }

    /// <summary>
    /// checks if jail contains current player
    /// </summary>
    /// <returns></returns>
    public static bool InJail()
    {
        if (_playersInJail.Contains(GameManager.CurrentPlayer))
            return true;
        else
            return false;
    }







}
