using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Generate : MonoBehaviour {

    public GameObject node;

	float cycleDuration;

	public Text cycleText;
	public Text distanceText;
	public Text timeText;

	private int numberOfNodes;
	private int numberOfMuscles;

	private List<Node> nodes;
	private List<Muscle> muscles;

	private float currentTime = 0;

	void Start ()
    {
		if(Constants.randomNumbers) {
			numberOfNodes = Random.Range (3, 6);
			numberOfMuscles = Random.Range (numberOfNodes, numberOfNodes * 4);
		} else {
			numberOfNodes = Constants.numberOfNodes;
			numberOfMuscles = Constants.numberOfMuscles;
		}
		
		cycleDuration = 10;//Random.value * Constants.cycleDurationMultiplier + 0.01f;

		nodes = new List<Node> ();
		for (var k = 0; k < numberOfNodes; k++)
        {
			nodes.Add (new Node ((Random.value + 1) * Constants.frictionAmplitude, new Vector2(Random.value, Random.value*0 + 1) * 10, 1, nodes.Count));
        }

//		nodes.Add (new Node (Random.value * Constants.frictionAmplitude, Vector2.up + Vector2.right * 5, 0));
//		nodes.Add (new Node (Random.value * Constants.frictionAmplitude, Vector2.up * 8, 1));
//		nodes.Add (new Node (Random.value * Constants.frictionAmplitude, Vector2.up + Vector2.left * 5, 1));

		float s = 0;
		foreach(var n in nodes) {
			s += n.position.x;
		}
		s /= nodes.Count;

		foreach(var n in nodes) {
			var p = n.position;
			p.x -= s;
			n.position = p;
		}

		muscles = new List<Muscle> ();
		var x = new List<Tuple> ();
		int sec = 0;
		for(int k = 0; k < numberOfMuscles; k++) {
			var a = new Tuple (Random.Range (0, numberOfNodes), Random.Range (0, numberOfNodes));
			if (k < numberOfNodes)
				a.a = k;
			if(!x.Contains(a) && a.a != a.b) {
				x.Add (a);
				x.Add (a.Reverse());
				var contractedLength = (Random.value + 1) * 3;
				var extendedLength = contractedLength + (Random.value + 1) * 1;
				muscles.Add (new Muscle (
					nodes [a.a],
					nodes [a.b],
					(Random.value + 0.01f) * Constants.strengthAmplitude,
					extendedLength,
					contractedLength,
					cycleDuration / 2//Random.value * cycleDuration
				));
			} else {
				if (sec < 100) {
					k -= 1;
					sec++;
				}
			}
		}
//		cycleDuration = 2;
//		muscles.Add (new Muscle (nodes [0], nodes [1], 10, 10, 1, cycleDuration / 2));
//		muscles.Add (new Muscle (nodes [2], nodes [1], 10, 10, 1, cycleDuration / 2));
//		muscles.Add (new Muscle (nodes [2], nodes [0], 10, 10, 1, cycleDuration / 2));
		print (muscles.Count);
	}

	void Update () {
		var time = (Time.time - cycleDuration * (Mathf.FloorToInt (Time.time / cycleDuration))) / Constants.timeMultiplier;

		foreach(var m in muscles) {
			m.Update (time);
		}
		foreach(var n in nodes) {
			n.Update (Time.deltaTime * Constants.timeMultiplier);
		}
		foreach(var m in muscles) {
			m.LateUpdate ();
		}
		foreach(var n in nodes) {
			n.LateUpdate ();
		}

		float s = 0;
		foreach(var n in nodes) {
			s += n.position.x;
		}
		s /= nodes.Count;

		distanceText.text = "Distance : " + s.ToString ();

		timeText.text = "Time : " + currentTime.ToString ();

		cycleText.text = (Mathf.Ceil (time * Constants.timeMultiplier / cycleDuration * 100)).ToString () + " %";

		currentTime += Time.deltaTime * Constants.timeMultiplier;

		if (Input.GetKeyDown (KeyCode.R))
			Restart ();
	}

	public void Restart () {
		foreach(var m in muscles) {
			m.Destroy ();
		} 
		foreach(var n in nodes) {
			n.Destroy ();
		}
		muscles.Clear ();
		nodes.Clear ();
		currentTime = 0;
		Start ();
	}
}

public struct Tuple {
	public int a;
	public int b;
	public Tuple (int a, int b) {
		this.a = a;
		this.b = b;
	}

	public Tuple Reverse () {
		return new Tuple (b, a);
	}

	public override bool Equals (object obj)
	{
		if (obj.GetType () == typeof(Tuple))
			return ((Tuple)obj) == this;
		else
			return false;
	}

	public override int GetHashCode ()
	{
		return a.GetHashCode () + b.GetHashCode ();
	}

	public static bool operator ==(Tuple x, Tuple y) {
		return x.a == y.a && x.b == y.b;
	}

	public static bool operator !=(Tuple x, Tuple y) {
		return x.a != y.a || x.b != y.b;
	}
}