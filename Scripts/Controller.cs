using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
	public Text CycleText;
	public Text DistanceText;
	public Text TimeText;
	public List<Creature> Creatures;

	public bool Train;

	float currentTime;
	const float deltaTime = 0.001f;

	Creature currentCreature;

	void Start ()
	{
		currentCreature = Creatures [0];
		foreach (var c in Creatures) {
			c.DistanceText = DistanceText;
			c.CycleText = CycleText;
		}
		currentCreature.CreatureUI = true;
		InvokeRepeating ("CustomUpdate", 0, deltaTime);
		if (Train) {
			foreach (var c in Creatures) {
				for (var k = 0; k < 30000; k++) {
					c.Update (deltaTime);
				}
			}
			float max = currentCreature.GetAveragePosition ();
			var l = 0;
			while (l < Creatures.Count) {
				var c = Creatures [l];
				if (c.GetAveragePosition () > max) {
					max = c.GetAveragePosition ();
					currentCreature.Destroy ();
					Creatures.Remove (currentCreature);
					currentCreature = c;
				} else if (c.GetAveragePosition () < max) {
					c.Destroy ();
					Creatures.Remove (c);
				} else {
					l++;
				}
			}
			currentCreature.CreatureUI = true;
		}
		foreach (var c in Creatures) {
			c.EnableGraphics = true;
		}
	}

	void CustomUpdate ()
	{
		foreach (var c in Creatures) {
			for (var k = 0; k < Constants.TimeMultiplier; k++) {
				c.Update (deltaTime);
			}
		}
	}

	void Update ()
	{
		/*
		 * Update graphics
		 */
		foreach (var c in Creatures) {
			c.UpdateGraphics ();
		}
	
		/*
		 * Get fastest creature
		 */
		float max = currentCreature.GetAveragePosition ();
		foreach (var c in Creatures) {
			if (c.GetAveragePosition () > max) {
				max = c.GetAveragePosition ();
				currentCreature.CreatureUI = false;
				currentCreature = c;
				currentCreature.CreatureUI = true;
			}
		}
		/*
		 * Remove the slowest
		 */
		// Analysis disable once ForCanBeConvertedToForeach
		for (var k = 0; k < Creatures.Count; k++) {
			var c = Creatures [k];
			if (c.GetAveragePosition () < max - 50) {
				c.Destroy ();
				Creatures.Remove (c);
			}
		}
		/*
		 * Update average position
		 */
		var tmp = transform.position;
		tmp.x = Mathf.Lerp (tmp.x, currentCreature.GetAveragePosition (), Time.deltaTime * Constants.TimeMultiplier / 10);
		transform.position = tmp;

		/*
		 * Update UI
		 */
		currentTime += Time.deltaTime * Constants.TimeMultiplier;
		TimeText.text = "Time : " + currentTime;
	}


	public void Destroy ()
	{
		foreach (var c in Creatures) {
			c.Destroy ();
		}
	}
}