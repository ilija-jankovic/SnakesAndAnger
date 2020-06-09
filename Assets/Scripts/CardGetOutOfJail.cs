using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGetOutOfJail : Card
{

    public CardGetOutOfJail(string does) : base(does)
    {

    }

    public override void Use()
    {
        //makes sure player is in jail before they can use card
        if (Jail.InJail())
        {
            Jail.LeaveJail();
            base.Use();
            GameManager.EndOfRollOptions();
        }
    }

    public override Sprite Icon
    {
        get { return Resources.Load<Sprite>("getOutOfJailCard"); }
    }
}
