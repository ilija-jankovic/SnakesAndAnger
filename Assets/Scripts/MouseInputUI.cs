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
    protected Text toolTip;
    public bool clickEnabled = true;
    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {

            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry();
            // Pointer Enter
            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { if (Interactible) EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);

            //Pointer Exit
            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry();
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { if (Interactible) ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);

            //Pointer Click
            EventTrigger.Entry clickUI = new EventTrigger.Entry();
            clickUI.eventID = EventTriggerType.PointerClick;
            clickUI.callback.AddListener((eventData) => { if (Interactible && clickEnabled) ClickUI(); });
            eventTrigger.triggers.Add(clickUI);
        }
    }

    private bool Interactible
    {
        get
        {
            bool ai = GameManager.CurrentPlayer.GetComponent<AI>();
            return !ai || TradingSystem.HumanCounterOffering;
        }
    }

    //when mouse enters gameobject
    public virtual void EnterUI()
    {
        toolTip = new GameObject().AddComponent<Text>();
        toolTip.rectTransform.SetParent(gameObject.GetComponent<Image>().rectTransform);
        toolTip.rectTransform.localPosition = Vector2.zero;
        toolTip.rectTransform.sizeDelta = new Vector2(100, 200);
        toolTip.fontSize = 15;
        toolTip.color = Color.black;
        toolTip.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        toolTip.alignment = TextAnchor.MiddleCenter;
    }
    //when mouse exits gameobject
    public virtual void ExitUI()
    {
        //update detailed card display
        Property property = GameManager.CurrentPlayer.Position.GetComponent<Property>();
        if (GameManager.ActiveCard != null)
            MenuManager.UpdateCardInfo(GameManager.ActiveCard);
        else if (property != null)
            MenuManager.UpdateCardInfo(property);

        Destroy(toolTip);
        toolTip = null;
    }
    //when mouse clicks gameobject
    public virtual void ClickUI()
    {
        if (!clickEnabled)
            return;
    }
}
