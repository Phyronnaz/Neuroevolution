using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;

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
		InvokeRepeating ("CustomUpdate", 0, deltaTime);
	}

	void CustomUpdate ()
	{
		var numberOfThreads = Mathf.Min (Constants.NumberOfThreads, Creatures.Count);
		var threads = new List<Thread> ();
		var count = Creatures.Count - Creatures.Count % numberOfThreads;
		int h = count / numberOfThreads;
		for (var k = 0; k < numberOfThreads; k++) {
			var creaturesToProcess = Creatures.GetRange (k * h, h);
			threads.Add (new Thread (() => ThreadedJob (creaturesToProcess, Constants.TimeMultiplier)));
			threads [threads.Count - 1].Start ();
		}
		if (count != Creatures.Count) {
			var endCreaturesToProcess = Creatures.GetRange (count, Creatures.Count - count);
			threads.Add (new Thread (() => ThreadedJob (endCreaturesToProcess, Constants.TimeMultiplier)));
			threads [threads.Count - 1].Start ();
		}

		foreach (var t in threads) {
			t.Join ();
		}
	}

	static void ThreadedJob (List<Creature> creatures, int time)
	{
//		print ("Bug");
		foreach (var c in creatures) {
			for (var k = 0; k < time; k++) {
				c.Update (deltaTime);
			}
		}
	}

	void Update ()
	{
		// Update graphics
		foreach (var c in Creatures) {
			c.UpdateGraphics ();
		}
	
		// Get fastest creature
		float max = currentCreature.GetAveragePosition ();
		foreach (var c in Creatures) {
			if (c.GetAveragePosition () > max) {
				max = c.GetAveragePosition ();
				currentCreature = c;
			}
		}

		// Remove the slowest
		foreach (var c in Creatures) {
			if (c.GetAveragePosition () < max - 50)
				c.Destroy ();
		}
		Creatures.RemoveAll (c => c.GetAveragePosition () < max - 50);

		// Update average position
		var tmp = transform.position;
		tmp.x = Mathf.Lerp (tmp.x, currentCreature.GetAveragePosition (), Time.deltaTime * Constants.TimeMultiplier / 10);
		transform.position = tmp;

		// Update UI
		currentCreature.UpdateUI (DistanceText, CycleText);
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