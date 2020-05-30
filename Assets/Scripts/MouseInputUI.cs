using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public abstract class MouseInputUI : MonoBehaviour
{
    private EventTrigger eventTrigger;
    public bool clickEnabled = true;
    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            bool ai = GameManager.CurrentPlayer.GetComponent<AI>();
            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry();
            // Pointer Enter
            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { if (!ai) EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);

            //Pointer Exit
            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry();
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { if (!ai) ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);

            //Pointer Click
            EventTrigger.Entry clickUI = new EventTrigger.Entry();
            clickUI.eventID = EventTriggerType.PointerClick;
            clickUI.callback.AddListener((eventData) => { if (!ai && clickEnabled) ClickUI(); });
            eventTrigger.triggers.Add(clickUI);
        }
    }

    //when mouse enters gameobject
    public abstract void EnterUI();
    //when mouse exits gameobject
    public virtual void ExitUI()
    {
        //update detailed card display
        Property property = GameManager.CurrentPlayer.Position.GetComponent<Property>();
        if (GameManager.ActiveCard != null)
            MenuManager.UpdateCardInfo(GameManager.ActiveCard);
        else if (property != null)
            MenuManager.UpdateCardInfo(property);
    }
    //when mouse clicks gameobject
    public abstract void ClickUI();
}
