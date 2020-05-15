using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private static byte _result;
    static Rigidbody rb;
    public static Vector3 diceVelocity;

    public static void Initialise()
    {
        rb = GetComponent<Rigidbody>();
    }

    public static void Update()
    {
        diceVelocity = rb.Velocity;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DiceNumberTextScript.diceNumber = 0;

            float xDirection = Random.Range(0, 500);
            float yDirection = Random.Range(0, 500);
            float zDirection = Random.Range(0, 500);

            transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.identity;

            rb.AddForce(transform.up * 500);
            rb.AddTorque(dirX, dirY, dirZ);
        }
    }

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
