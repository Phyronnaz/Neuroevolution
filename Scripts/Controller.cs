using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {

	#region variables
	public Text cycleText;
	public Text distanceText;
	public Text timeText;
	public List<Creature> creatures;

	public bool train = false;// || true;

	private float currentTime = 0;
	private float deltaTime = 0.001f;
	private Creature currentCreature;
	#endregion


	#region Start && Update && CustomUpdate
	void Start () {
		currentCreature = creatures [0];
		foreach (var c in creatures) {
			c.distanceText = distanceText;
			c.cycleText = cycleText;
		}
		currentCreature.CreatureUI = true;
		InvokeRepeating ("CustomUpdate", 0, deltaTime);
		if (train) {
			foreach (var c in creatures) {
				for (var k = 0; k < 30000; k++) {
					c.Update (deltaTime);
				}
			}
			float max = currentCreature.GetAveragePosition();
			var l = 0;
			while (l < creatures.Count) {
				var c = creatures [l];
				if (c.GetAveragePosition() > max) {
					max = c.GetAveragePosition();
					currentCreature.Destroy ();
					creatures.Remove (currentCreature);
					currentCreature = c;
				} else if (c.GetAveragePosition() < max) {
					c.Destroy ();
					creatures.Remove (c);
				} else {
					l++;
				}
			}
			currentCreature.CreatureUI = true;
		}
		foreach(var c in creatures) {
			c.enableGraphics = true;
		}
	}

	void CustomUpdate () {
		foreach (var c in creatures) {
			for (var k = 0; k < Constants.timeMultiplier; k++) {
				c.Update (deltaTime);
			}
		}
	}

	void Update () {
		/*
		 * Update graphics
		 */
		foreach(var c in creatures) {
			c.UpdateGraphics ();
		}
	
		/*
		 * Get fastest creature
		 */
		float max = currentCreature.GetAveragePosition();
		foreach(var c in creatures) {
			if (c.GetAveragePosition() > max) {
				max = c.GetAveragePosition();
				currentCreature.CreatureUI = false;
				currentCreature = c;
				currentCreature.CreatureUI = true;
			}
		}
		/*
		 * Update average position
		 */
		var tmp = transform.position;
		tmp.x = Mathf.Lerp(tmp.x, currentCreature.GetAveragePosition(), Time.deltaTime);
		transform.position = tmp;

		/*
		 * Update UI
		 */
		currentTime += Time.deltaTime * Constants.timeMultiplier;
		timeText.text = "Time : " + currentTime.ToString ();
	}
	#endregion

	public void Destroy () {
		foreach(var c in creatures) {
			c.Destroy ();
		}
	}
}