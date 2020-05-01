using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private static byte _result;

    public static void Roll()
    {

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
