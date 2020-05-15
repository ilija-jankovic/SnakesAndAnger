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
        }
    }

    public void EnterUI()
    {
        BlockedByUI = true;
            Debug.Log("hovering");
            MenuManager.ShowMenu(MenuManager.CardInfo);
            GameObject.FindGameObjectWithTag("PropertyInfo").GetComponent<Text>().text = property.Description();
            GameObject.FindGameObjectWithTag("PropertyTitle").GetComponent<Text>().text = property.GetComponent<Property>().Title;

            //set colour of card
            Image streetColour = GameObject.FindGameObjectWithTag("StreetColour").GetComponent<Image>();
            Street street = property.GetComponent<Street>();
            if (street != null)
                streetColour.color = new Color(street.Colour.r, street.Colour.g, street.Colour.b, 1);
            else
                streetColour.color = Vector4.zero;
    }
    public void ExitUI()
    {
        BlockedByUI = false;
    }

}
