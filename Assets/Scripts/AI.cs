using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    enum modes { aggressive, passive, trading };
    void Start()
    {
        
    }

    void Update()
    {
        if (GameManager.CurrentPlayer == gameObject.GetComponent<Player>())
            if (MenuManager.TurnOptions.enabled == true)
            {
                Click(MenuManager.Roll);
            }
            else if (MenuManager.EndOfTurnOptions.enabled == true)
            {
                if(!Click(MenuManager.Buy))
                    if(!Click(MenuManager.Auction))
                        Click(MenuManager.NextTurn);
            }
            else if(MenuManager.PaymentOptions.enabled == true)
            {
                if (!(GameManager.ActiveCard is CardCollect) || CardCollect.currentPayee == GameManager.CurrentPlayer)
                    if (!Click(MenuManager.Pay))
                        Mortgage();
            }
            else if (MenuManager.PaymentTileOptions.enabled == true)
            {
                int potentialFunds = GameManager.CurrentPlayer.GetTotalPotentialBalance();
                if (potentialFunds > 200)                                               //may need to change the 200 for super tax tile
                    Click(MenuManager.PayFixed);
                else
                    Click(MenuManager.PayPercentage);
            }
            else if (MenuManager.CardOptions.enabled == true)
            {
                Click(MenuManager.AcknowledgeCard);
            }
            else if(MenuManager.LoseOptions.enabled == true)
            {
                Click(MenuManager.Bankrupt);
            }
    }

    //mortgage/sell properties when AI has enough potential funds but doesn't have enough in hand
    private void Mortgage()
    {
        foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
            if (property.CanMortagage())
            {
                property.Mortgage();
                return;
            }

        //if cant mortgage AI can still sell houses
        foreach (Property property in GameManager.CurrentPlayer.PropertiesOwned)
        {
            Street street = property.GetComponent<Street>();
            if (street != null && street.CanSellHouse())
            {
                street.SellHouse();
                return;
            }
        }
    }

    private bool Click(Button button)
    {
        if (button.interactable)
            button.onClick.Invoke();
        return button.interactable;
    }
}
