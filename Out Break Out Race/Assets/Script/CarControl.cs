using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// This script will assume how joystick works, the varible for that will be controlled by mouseOri and movedMouse
/// </summary>


public class CarControl : MonoBehaviour {
	public Rigidbody2D backWheel; //To control the movment of the car
	public Text carSta, carHealthDis;
	public Transform[] wheels;
	public float jumpHeight, carHealth, carAr;
	public bool haveSpike;

	private float speed, mouseMovedOff, slowed, score, kms, nitrate;
	private Vector2 mouseOri, movedMouse;
	private RaycastHit2D isFloored;
	private int spikeSpateEndu;

	// Use this for initialization
	void Start () {
		mouseOri = Camera.main.ScreenToViewportPoint (Input.mousePosition);
		score = 0;
		slowed = 1; //affect the car's current speed
		nitrate = 1;
		spikeSpateEndu = 10;
		DisplayHealthScore(carHealth, carAr, score);
	}
	
	// Update is called once per frame
	void Update () {
		movedMouse = Camera.main.ScreenToViewportPoint (Input.mousePosition);

		if (movedMouse.x != mouseOri.x) {//signified the player has moved the moue
			if (movedMouse.x > mouseOri.x) {
				mouseMovedOff = Vector2.Distance(movedMouse, mouseOri); //the offset from the original mouse position.
				speed = mouseMovedOff * 100;
				//Illusion for the wheels
				wheels[0].Rotate(Vector3.forward * -speed, Space.Self);
				wheels[1].Rotate(Vector3.forward * -speed, Space.Self);

				//The car can only jumps when is moving
				if (Input.GetMouseButtonDown(0)) {
					isFloored = Physics2D.Raycast(transform.position, Vector2.up, -2, 
					                              1 << LayerMask.NameToLayer("Road")); //Check to see if the car is grounded.
					Debug.Log(isFloored.collider);

					if (isFloored.collider != null) {
						backWheel.AddRelativeForce(Vector2.up * jumpHeight, ForceMode2D.Impulse); //makes the car jump
					}
				}
			}

			if (movedMouse.x < mouseOri.x) {// direction for the brakes
				speed = 0; 
			}

			//creates a graduation of slowing down from using the nitrate
			if (nitrate > 1) {
				nitrate -= Time.smoothDeltaTime;
			}

			//using the nitrate
			if (Input.GetMouseButtonDown(1)) {
				nitrate = 3;
			}
		}

		backWheel.AddRelativeForce (Vector2.right * speed * slowed * nitrate, ForceMode2D.Force); //moving the car
		kms = speed * slowed * nitrate;
		carSta.text = kms.ToString ("F2") + "km/s"; //Displaying the car's status
		Debug.Log (nitrate);

		//disable the spike
		if (spikeSpateEndu <= 0) {
			haveSpike = false;
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		PenaltyRewards (c.GetComponent<Collider2D> ().gameObject.tag);
	}

	void PenaltyRewards (string hitObTag) {
		float remainDam = 0; //The remaining damage once absorbed by the armour

		switch (hitObTag) {
		case "ZombieS": //A single zombie is hit
			score += 10;
			DisplayHealthScore(carHealth, carAr, score);
			break;
		case "ZombieH": //A horge of zombies is hit
			if (!haveSpike) { //This block won't happens if the player have equipped a spike.
				if (carAr > 0) {
					remainDam = 200 * 0.2f;
					carAr -= 200 * 0.8f;
					carHealth -= remainDam; 
				}

				if (carAr <= 0) { //armour not active
					carHealth -= 200;
				}

				slowed = 0.5f;
			}

			if (haveSpike) {
				spikeSpateEndu--;
			}

			score += 50;
			DisplayHealthScore(carHealth, carAr, score);

			break;
		case "Barricade": //hit a barricard
			if (!haveSpike) { ////This block won't happens if the player have equipped a spike.
				slowed = 0.1f;
			}

			if (haveSpike) {
				spikeSpateEndu--;
			}
			break;
		}
	}

	void DisplayHealthScore (float h, float a, float s) {
		carHealthDis.text = "health: " + h.ToString () + "\n Scrap Metal: " + a.ToString () 
			+ "\n Exp: " + s.ToString() + "\n SP: " + spikeSpateEndu.ToString();
	}
}
