using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieZoneCheck : MonoBehaviour {
	[SerializeField]
	Die die;
	Vector3 diceVelocity;

	// Update is called once per frame
	void FixedUpdate ()
    {
		diceVelocity = die.Velocity;     //Get the dice velocity.
	}

	void OnTriggerStay(Collider col)        //Once the dice has entered.
	{
		if (col.transform.parent == die.transform && (diceVelocity.x) == 0f && (diceVelocity.y) == 0f && (diceVelocity.z == 0f))     //Check the dice has stopped moving.
		{
			switch (col.gameObject.name)        //Check which dice collider has entered.
            {
			    case "side01":
                    die.number = 6;
				    break;
			    case "side02":
					die.number = 5;
				    break;
			    case "side03":
					die.number = 4;
				    break;
			    case "side04":
					die.number = 3;
				    break;
			    case "side05":
					die.number = 2;
				    break;
			    case "side06":
					die.number = 1;
				    break;
			}
		}
	}
}
