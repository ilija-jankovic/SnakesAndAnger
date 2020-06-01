using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class InventoryPropertyMouseInputUI : MouseInputUI
{
    public Property property;
    public override void EnterUI()
    {
        base.EnterUI();

        MenuManager.UpdateCardInfo(property);

        if (!MenuManager.TradingMode)
        {
            Street street = property.GetComponent<Street>();
            if (MenuManager.BuildHouseMode)
            {
                if (street == null)
                    toolTip.text = "Cannot build house on this type of property";
                else if (!street.CanBuildHouse())
                    toolTip.text = "House requirements not met";
                else
                    toolTip.text = "Build house for $" + street.HousePrice;
            }
            else if (property.CanMortagage())
                toolTip.text = "Click to mortgage";
            else if (street != null && street.CanSellHouse())
                toolTip.text = "Click to sell house for $" + street.SellHousePrice;
            else if (property.Mortgaged && MenuManager.PaymentOptions.enabled == false)
                toolTip.text = "Click to unmortgage";
            else if (MenuManager.PaymentOptions.enabled == true)
                toolTip.text = "Cannot unmortgage as a payment is due";
            else
                toolTip.text = "House selling requirements not met";

            //change camera to property
            CameraFollow.target = property.transform;
            return;
        }

        //set tool tip based on whether property offered/who is offering
        Player owner = property.Owner;
        if (owner == GameManager.CurrentPlayer)
        {
            if (!TradingSystem.CurrentPlayerOffer.Contains(property))
            {
                toolTip.text = !TradingSystem.CounterOfferInProgress ? "Add to Offer" : "Add to Demand";
                return;
            }
            toolTip.text = !TradingSystem.CounterOfferInProgress ? "Remove from offer" : "Remove from demand";
            return;
        }
        if (!TradingSystem.TradeeOffer.Contains(property))
        {
            toolTip.text = !TradingSystem.CounterOfferInProgress ? "Add to demand" : "Add to Offer";
            return;
        }
        toolTip.text = !TradingSystem.CounterOfferInProgress ? "Remove from demand" : "Remove from offer";
    }

    public override void ExitUI()
    {
        base.ExitUI();

        //prevent memory leaks
        Resources.UnloadUnusedAssets();

        //change camera back to player
        CameraFollow.target = property.Owner.transform;
    }

    //mortgage
    public override void ClickUI()
    {
        base.ClickUI();

        if (!MenuManager.TradingMode)
        {
            Street street = property.GetComponent<Street>();
            if (!MenuManager.BuildHouseMode)
            {
                if (property.CanMortagage())
                    property.Mortgage();
                else if (street != null && street.CanSellHouse())
                    street.SellHouse();
                //can only unmortgage if player does not need to pay anything
                else if (property.Mortgaged && MenuManager.PaymentOptions.enabled == false)
                    property.UnMortgage();
            }
            else if (MenuManager.BuildHouseMode)
                if (street != null)
                    street.BuildHouse();
            return;
        }

        //check if AI is currently trading, and disable clicking if it is
        if ((TradingSystem.CounterOfferInProgress && TradingSystem.Tradee.gameObject.GetComponent<AI>() != null)
        || (!TradingSystem.CounterOfferInProgress && GameManager.CurrentPlayer.GetComponent<AI>() != null))
            return;

        TradingSystem.ToggleCardInOffer(property);
    }
}
