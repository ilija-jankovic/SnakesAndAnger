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

        cardToolTip.text = "Click to use";

        MenuManager.UpdateCardInfo(card);

        GameObject imageObj = GameObject.FindGameObjectWithTag("CardSprite");
        RawImage img = imageObj.GetComponent<RawImage>();
        Texture2D icon = card.Icon;
        if (icon != null)
            img.texture = icon;
    }

    public override void ExitUI()
    {
        Destroy(cardToolTip);
        cardToolTip = null;
    }

    public override void ClickUI()
    {

    }
}
