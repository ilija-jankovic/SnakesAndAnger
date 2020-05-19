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

        if (!property.Mortgaged)
            mortgageToolTip.text = "Click to mortgage";
        else if(MenuManager.PaymentOptions.enabled == false)
            mortgageToolTip.text = "Click to unmortgage";
        else
            mortgageToolTip.text = "Cannot unmortgage as a payment is due";
    }

    public override void ExitUI()
    {
        MenuManager.UpdateCardInfo(GameManager.CurrentPlayer.Position.GetComponent<Property>());
        if(mortgageToolTip != null)
        {
            Destroy(mortgageToolTip);
            mortgageToolTip = null;
        }
    }

    //mortgage
    public override void ClickUI()
    {
        if (!property.Mortgaged)
        {
            property.Mortgage();
            MenuManager.UpdateInventoryData();
            MenuManager.UpdateCardInfo(property);

            //updates pay button incase player has received enough mortgage to pay off something
            GameManager.UpdatePayButtonInteractibility();
            GameManager.UpdateBuyButtonInteractibility();
        }
        //can only unmortgage if player does not need to pay anything
        else if(MenuManager.PaymentOptions.enabled == false)
        {
            property.UnMortgage();
            MenuManager.UpdateInventoryData();
            MenuManager.UpdateCardInfo(property);
        }
    }
}
