using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCardMouseInputUI : MouseInputUI
{
    public Card card;
    public override void EnterUI()
    {
        base.EnterUI();

        //can only use card if enabled
        if (MenuManager.TurnOptions.enabled == true)
            toolTip.text = "Click to use";
        else
            toolTip.text = "Can only use before rolling";

        MenuManager.UpdateCardInfo(card);
    }

    public override void ExitUI()
    {
        base.ExitUI();

        //prevent memory leaks
        Resources.UnloadUnusedAssets();
    }

    public override void ClickUI()
    {
        base.ClickUI();
        if (MenuManager.TurnOptions.enabled == true)
            card.Use();
    }
}
