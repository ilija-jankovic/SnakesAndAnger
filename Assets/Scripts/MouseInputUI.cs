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

    //when mouse enters gameobject
    public abstract void EnterUI();
    //when mouse exits gameobject
    public abstract void ExitUI();
    //when mouse clicks gameobject
    public abstract void ClickUI();
}
