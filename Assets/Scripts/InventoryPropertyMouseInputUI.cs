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
    private Text mortgageToolTip;
    public override void EnterUI()
    {
        MenuManager.UpdateCardInfo(property);

        //display how to mortgage/unmortgage
        mortgageToolTip = new GameObject().AddComponent<Text>();
        mortgageToolTip.rectTransform.SetParent(gameObject.GetComponent<Image>().rectTransform);
        mortgageToolTip.rectTransform.localPosition = Vector2.zero;
        mortgageToolTip.rectTransform.sizeDelta = new Vector2(100, 200);
        mortgageToolTip.fontSize = 15;
        mortgageToolTip.color = Color.black;
        mortgageToolTip.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        mortgageToolTip.alignment = TextAnchor.MiddleCenter;

        Street street = property.GetComponent<Street>();
        if (MenuManager.BuildHouseMode)
        {
            if (street == null)
                mortgageToolTip.text = "Cannot build house on this type of property";
            else if (!street.CanBuildHouse())
                mortgageToolTip.text = "House requirements not met";
            else
                mortgageToolTip.text = "Build house for $" + street.HousePrice;
        }
        else if (property.CanMortagage())
            mortgageToolTip.text = "Click to mortgage";
        else if (street != null && street.CanSellHouse())
            mortgageToolTip.text = "Click to sell house for $" + street.SellHousePrice;
        else if (property.Mortgaged && MenuManager.PaymentOptions.enabled == false)
            mortgageToolTip.text = "Click to unmortgage";
        else if (MenuManager.PaymentOptions.enabled == true)
            mortgageToolTip.text = "Cannot unmortgage as a payment is due";
        else
            mortgageToolTip.text = "House selling requirements not met";

        //change camera to property
        CameraFollow.target = property.transform;
    }

    public override void ExitUI()
    {
        base.ExitUI();
        if(mortgageToolTip != null)
        {
            Destroy(mortgageToolTip);
            mortgageToolTip = null;
        }

        //prevent memory leaks
        Resources.UnloadUnusedAssets();

        //change camera back to player
        CameraFollow.target = property.Owner.transform;
    }

    //mortgage
    public override void ClickUI()
    {
        Street street = property.GetComponent<Street>();
        if (!MenuManager.BuildHouseMode)
        {
            if (property.CanMortagage())
            {
                property.Mortgage();
                MenuManager.UpdateCardInfo(property);

                GameManager.UpdatePayButtonInteractibility();
                GameManager.UpdateBuyButtonInteractibility();
            }
            else if (street != null && street.CanSellHouse())
                street.SellHouse();
            //can only unmortgage if player does not need to pay anything
            else if (property.Mortgaged && MenuManager.PaymentOptions.enabled == false)
            {
                property.UnMortgage();
                MenuManager.UpdateCardInfo(property);
            }
        }
        else if (MenuManager.BuildHouseMode)
            if (street != null)
                street.BuildHouse();

        MenuManager.UpdateInventoryData(property.Owner);

        //updates pay button incase player has received enough from mortgage/selling houses to pay off something
        GameManager.UpdatePayButtonInteractibility();
        GameManager.UpdateBuyButtonInteractibility();
    }
}
