using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCardMouseInputUI : MouseInputUI
{
    public Card card;
    private Text cardToolTip;
    public override void EnterUI()
    {
        cardToolTip = new GameObject().AddComponent<Text>();
        cardToolTip.rectTransform.SetParent(gameObject.GetComponent<Image>().rectTransform);
        cardToolTip.rectTransform.localPosition = Vector2.zero;
        cardToolTip.rectTransform.sizeDelta = new Vector2(100, 200);
        cardToolTip.fontSize = 15;
        cardToolTip.color = Color.black;
        cardToolTip.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        cardToolTip.alignment = TextAnchor.MiddleCenter;

        //can only use card if enabled
        if(MenuManager.TurnOptions.enabled == true)
            cardToolTip.text = "Click to use";
        else
            cardToolTip.text = "Can only use before rolling";

        MenuManager.UpdateCardInfo(card);
    }

    public override void ExitUI()
    {
        base.ExitUI();
        Destroy(cardToolTip);
        cardToolTip = null;

        //prevent memory leaks
        Resources.UnloadUnusedAssets();
    }

    public override void ClickUI()
    {
        //can only use card if enabled
        //if (MenuManager.TurnOptions.enabled == true)
    }
}
