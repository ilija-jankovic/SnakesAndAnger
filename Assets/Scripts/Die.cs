using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour {

	private Rigidbody rb;
	private Vector3 velocity;
	[SerializeField]
	private int initialUpForce;
	public byte number;
	private static Die[] dice;
	private static byte _diceTotal = 0;
	private static bool _rolling;

	// Use this for initialization
	void Start ()
    {
		rb = GetComponent<Rigidbody> ();        //Set rigidbody equal to rb.
		dice = GameObject.FindObjectsOfType<Die>();
	}

	// Update is called once per frame
	void Update()
	{
		velocity = rb.velocity;     //Set the velocity of the dice.

		if (Rolling)
		{
			byte diceTotal = 0;
			foreach (Die die in dice)
			{
				if (die.number == 0)
					return;
				diceTotal += die.number;
			}

			//set total
			_diceTotal = diceTotal;
			_rolling = false;

			//checks if player is in jail or not
			if (!Jail.InJail())
			{
				GameManager.CurrentPlayer.Move((sbyte)Result);
			}
			else if (dice[0].number == dice[1].number)
			{
				Jail.LeaveJail();
				GameManager.CurrentPlayer.Move((sbyte)Result);
			}

			//set the camera to track the current player and enable end of roll options
			CameraFollow.target = GameManager.CurrentPlayer.transform;
			GameManager.EndOfRollOptions();
		}
	}

	public static void Roll()
	{
		MenuManager.SwitchToMenu(null);
		_rolling = true;
		CameraFollow.target = dice[0].transform;

		foreach (Die die in dice)
		{
			die.number = 0;

			//Respawn the dice at a random point.
			float pointX = Random.Range(0, 5);
			float pointY = Random.Range(die.transform.localScale.x / 2, die.transform.localScale.x);
			float pointZ = Random.Range(0, 5);

			//Give random movement (toss) to the dice.
			float dirX = Random.Range(0, 500);
			float dirY = Random.Range(0, 500);
			float dirZ = Random.Range(0, 500);

			Vector3 boardPos = GameObject.Find("Board").transform.position;

			//Set dice.
			die.transform.position = new Vector3(boardPos.x + pointX, boardPos.y + pointY, boardPos.z + pointZ);
			die.transform.rotation = Quaternion.identity;

			//Set initial movement.
			die.rb.AddForce(die.transform.up * die.initialUpForce);
			die.rb.AddTorque(dirX, dirY, dirZ);

			//set camera to closest dice
			if (die.transform.position.z < CameraFollow.target.position.z)
				CameraFollow.target = die.transform;
		}
	}

	public Vector3 Velocity
	{
		get { return velocity; }
	}

	public static byte Result
	{
		get { return _diceTotal; }
	}

	public bool Rolling
	{
		get { return _rolling; }
	}
}
