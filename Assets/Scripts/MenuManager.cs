using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class MenuManager
{
    private static Canvas[] allMenus;
    private static Canvas _startOfTurn = GameObject.FindGameObjectWithTag("TurnCanvas").GetComponent<Canvas>();
    private static Canvas _endOfTurn = GameObject.FindGameObjectWithTag("EndOfTurnCanvas").GetComponent<Canvas>();
    //make this at some point
    private static Canvas _winMenu;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        allMenus = new Canvas[] { TurnOptions, EndOfTurnOptions };

        //call Roll method in Die class when 'RollButton' is clicked
        GameObject.FindGameObjectWithTag("RollButton").GetComponent<Button>().onClick.AddListener(Die.Roll);
        //call Player's Buy method when 'BuyButton' is clicked
        GameObject.FindGameObjectWithTag("BuyButton").GetComponent<Button>().onClick.AddListener(delegate { GameManager.CurrentPlayer.Purchase(); });

        SwitchToMenu(TurnOptions);
    }

    private static void DisableAllMenus()
    {
        foreach (Canvas canvas in allMenus)
            canvas.enabled = false;
    }

    public static void SwitchToMenu(Canvas menu)
    {
        DisableAllMenus();
        foreach (Canvas canvas in allMenus)
            if (canvas == menu)
                menu.enabled = true;
    }

    public static Canvas TurnOptions
    {
        get { return _startOfTurn; }
    }

    public static Canvas EndOfTurnOptions
    {
        get { return _endOfTurn; }
    }

    public static Canvas WinMenu
    {
        get { return _winMenu; }
    }
}
