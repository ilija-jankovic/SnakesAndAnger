using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private static byte _result;
    static Rigidbody rb;
    public static Vector3 diceVelocity;

    public static void Roll()
    {
        //
        // This is just debugging for now Yaksh. The GameManager.CurrentPlayer.Move method call should stay though.
        //
        sbyte roll = (sbyte)UnityEngine.Random.Range(2, 12);
        Debug.Log("You rolled: " + roll);
        GameManager.CurrentPlayer.Move(roll);
        GameManager.EndOfRollOptions();
    }

    public static bool Rolling()
    {
        return false;
    }

    public static byte Result
    {
        get { return _result; }
    }
}
