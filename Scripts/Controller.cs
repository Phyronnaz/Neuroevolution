using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class Controller
{
	public List<Creature> Creatures;
	public float CurrentTime;
	public const float DeltaTime = 0.001f;


	public Controller (List<Creature> creatures)
	{
		Creatures = creatures;
	}

	public void Update (int time)
	{
		// Update creatures
		var numberOfThreads = Mathf.Min (Constants.NumberOfThreads, Creatures.Count);
		var threads = new List<Thread> ();
		var count = Creatures.Count - Creatures.Count % numberOfThreads;
		int h = count / numberOfThreads;
		for (var k = 0; k < numberOfThreads; k++) {
			var creaturesToProcess = Creatures.GetRange (k * h, h);
			threads.Add (new Thread (() => ThreadedJob (creaturesToProcess, time)));
			threads [threads.Count - 1].Start ();
		}
		if (count != Creatures.Count) {
			var endCreaturesToProcess = Creatures.GetRange (count, Creatures.Count - count);
			threads.Add (new Thread (() => ThreadedJob (endCreaturesToProcess, time)));
			threads [threads.Count - 1].Start ();
		}
		// Wait for threads to end
		foreach (var t in threads) {
			t.Join ();
		}

		// Update graphics
		foreach (var c in Creatures) {
			c.UpdateGraphics ();
		}

		// Update time
		CurrentTime += DeltaTime * time;
	}

	public void Update ()
	{
		Update (Constants.TimeMultiplier);
	}


	static void ThreadedJob (List<Creature> creatures, int time)
	{
		foreach (var c in creatures) {
			for (var k = 0; k < time; k++) {
				c.Update (DeltaTime);
			}
		}
	}

	public void TrainNextGeneration ()
	{
		Creatures.Sort ();

	}

	public float GetMaxPosition ()
	{
		float max = Creatures [0].GetAveragePosition ();
		foreach (var c in Creatures) {
			if (c.GetAveragePosition () > max)
				max = c.GetAveragePosition ();
		}
		return max;
	}


	public void RemoveCreaturesFartherThan (float distance)
	{
		RemoveCreaturesFartherThan (distance, GetMaxPosition ());
	}

	public void RemoveCreaturesFartherThan (float distance, float max)
	{
		foreach (var c in Creatures) {
			if (c.GetAveragePosition () < max - distance)
				c.Destroy ();
		}
		Creatures.RemoveAll (c => c.GetAveragePosition () < max - distance);
	}

	public void ResetCreatures ()
	{
		foreach (var c in Creatures) {
			c.Reset ();
		}
	}

	public int GetCyclePercentageOfTheFarthestCreatures ()
	{
		var max = Creatures [0].GetAveragePosition ();
		var bestCreature = Creatures [0];
		foreach (var c in Creatures) {
			if (c.GetAveragePosition () > max) {
				max = c.GetAveragePosition ();
				bestCreature = c;
			}
		}
		return bestCreature.GetCyclePercentage ();
	}


	public void Destroy ()
	{
		foreach (var c in Creatures) {
			c.Destroy ();
		}
		Creatures.Clear ();
		GC.Collect ();
	}
}