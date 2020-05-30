using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public static class SetupManager
{
    private static byte players;
    [RuntimeInitializeOnLoadMethod]
    private static void Initialise()
    {
        GameObject[] playerCountButtons = GameObject.FindGameObjectsWithTag("PlayerCountButton");
        GameObject[] tokenSelectorButtons = GameObject.FindGameObjectsWithTag("TokenSelectorButton");
        GameObject[] toggleAIButtons = GameObject.FindGameObjectsWithTag("ToggleAIButton");

        //sort button array
        Array.Sort(playerCountButtons, delegate (GameObject p1, GameObject p2) {
            return p1.name.CompareTo(p2.name);
        });

        foreach (GameObject buttonObj in playerCountButtons)
        {
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(
                delegate
                {
                    //update player count
                    players = byte.Parse(buttonObj.name[0].ToString());

                    //deactivate all count buttons
                    foreach (GameObject otherButtonObj in playerCountButtons)
                        otherButtonObj.GetComponent<Button>().interactable = false;

                    //activate all token buttons
                    foreach (GameObject otherButtonObj in tokenSelectorButtons)
                        otherButtonObj.GetComponent<Button>().interactable = true;

                    //deactivate pressed button
                    //button.interactable = false;
                });
        }

        foreach (GameObject buttonObj in tokenSelectorButtons)
        {
            Button button = buttonObj.GetComponent<Button>();

            button.onClick.AddListener(
                delegate
                {
                    Player[] players = GameObject.FindObjectsOfType<Player>();
                    byte playing = 0;

                    foreach (Player player in players)
                    {
                        if (player.name == button.GetComponentInChildren<Text>().text)
                            player.Playing = true;
                        if (player.Playing)
                            playing++;
                    }

                    button.interactable = false;

                    if (playing >= SetupManager.players)
                    {
                        //deactivate all setup buttons as game is ready to be played
                        foreach (GameObject otherButtonObj in tokenSelectorButtons)
                            otherButtonObj.GetComponent<Button>().interactable = false;
                        foreach (GameObject otherButtonObj in playerCountButtons)
                            otherButtonObj.GetComponent<Button>().interactable = false;

                        GameObject.Find("StartGameButton").GetComponent<Button>().interactable = true;
                    }

                    //activate ai toggle
                    foreach (GameObject toggleAIButtonObj in toggleAIButtons)
                        if (toggleAIButtonObj.transform.parent == buttonObj.transform)
                            toggleAIButtonObj.GetComponent<Button>().interactable = true;
                });
        }

        foreach(GameObject buttonObj in toggleAIButtons){
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(
                delegate
                {
                    //add/remove ai to token and toggle button text
                    Text textComp = button.GetComponentInChildren<Text>();
                    foreach (Player player in GameObject.FindObjectsOfType<Player>())
                        if (player.name == button.transform.parent.GetComponentInChildren<Text>().text)
                            if (player.GetComponent<AI>() == null)
                            {
                                player.gameObject.AddComponent<AI>();
                                textComp.text = "Disable AI";
                            }
                            else
                            {
                                GameObject.Destroy(player.GetComponent<AI>());
                                textComp.text = "Enable AI";
                            }
                });
        }

        GameObject.Find("StartGameButton").GetComponent<Button>().onClick.AddListener(GameManager.NewGame);

        GameObject.Find("ResetButton").GetComponent<Button>().onClick.AddListener(ResetSetupButtons);
    }

    private static void ResetSetupButtons()
    {
        foreach (GameObject buttonObj in GameObject.FindGameObjectsWithTag("PlayerCountButton"))
            buttonObj.GetComponent<Button>().interactable = true;

        foreach (GameObject buttonObj in GameObject.FindGameObjectsWithTag("TokenSelectorButton"))
            buttonObj.GetComponent<Button>().interactable = false;

        foreach (GameObject buttonObj in GameObject.FindGameObjectsWithTag("ToggleAIButton"))
            buttonObj.GetComponent<Button>().interactable = false;

        GameObject.Find("StartGameButton").GetComponent<Button>().interactable = false;

        foreach (Player player in GameObject.FindObjectsOfType<Player>())
            player.Playing = false;
    }

    public static void LoadSetupMenu()
    {
        MenuManager.SwitchToMenu(MenuManager.SetupOptions);
        ResetSetupButtons();
    }

    public static void StandardGame()
    {
        Player[] players = GameObject.FindObjectsOfType<Player>();

        //deactivate all players
        foreach (Player player in players)
            player.Playing = false;

        //add 4 players to game
        for (int i = 0; i < 4; i++)
            players[i].Playing = true;

        GameManager.NewGame();
    }
}
