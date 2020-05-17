using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class InventoryCardMouseInputUI : MonoBehaviour
{
    public static bool BlockedByUI;
    public Property property;
    private EventTrigger eventTrigger;
    private Text mortgageToolTip;

    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry();
            // Pointer Enter
            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);

            //Pointer Exit
            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry();
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);

            //Pointer Click
            EventTrigger.Entry clickUI = new EventTrigger.Entry();
            clickUI.eventID = EventTriggerType.PointerClick;
            clickUI.callback.AddListener((eventData) => { ClickUI(); });
            eventTrigger.triggers.Add(clickUI);
        }
    }

    public void EnterUI()
    {
        BlockedByUI = true;
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
    public void ExitUI()
    {
        BlockedByUI = false;
        MenuManager.UpdateCardInfo(GameManager.CurrentPlayer.Position.GetComponent<Property>());
        if(mortgageToolTip != null)
        {
            Destroy(mortgageToolTip);
            mortgageToolTip = null;
        }
    }

    //mortgage
    public void ClickUI()
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
