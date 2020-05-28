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
        Jail.LeaveJail();
        base.Use();
    }

}
