using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Creature {
	#region public variables
	public Text cycleText;
	public Text distanceText;

	public bool CreatureUI = false;
	public bool enableGraphics = false;
	#endregion
	#region private variables
	private List<Muscle> muscles;
	private List<Node> nodes;
	private Transform transform;

	private float cycleDuration;
	private float time;
	#endregion

	public Creature (List<Muscle> muscles, List<Node> nodes, float cycleDuration, Transform transform) {
		this.muscles = muscles;
		this.nodes = nodes;
		this.cycleDuration = cycleDuration;
		this.transform = transform;
		time = 0;
	}

	public void Update (float deltaTime) {
		/*
		 * Time modulo cycle duration
		 */
		time = (time - cycleDuration * (Mathf.FloorToInt (time / cycleDuration)));

		/*
		 * Update muscles and nodes
		 */
		foreach (var m in muscles) {
			m.Update (time);
		}
		foreach (var n in nodes) {
			n.Update (deltaTime);
		}

		/*
		* Update current time
		*/
		time += deltaTime;
	}

	public void UpdateGraphics () {
		if (enableGraphics) {
			foreach (var m in muscles) {
				m.UpdateGraphics ();
			}
			foreach (var n in nodes) {
				n.UpdateGraphics ();
			}
		}


		/*
		* Update Creature UI
		*/
		if (CreatureUI) {
			distanceText.text = "Distance : " + GetAveragePosition ().ToString ();
			cycleText.text = (Mathf.Ceil (time / cycleDuration * 100)).ToString () + " %";
		}
	}

	public float GetAveragePosition () {
		var averagePosition = 0f;
		foreach (var n in nodes) {
			averagePosition += n.position.x;
		}
		return averagePosition / nodes.Count;
	}

	public static Creature RandomCreature (float cycleDuration) {
		/*
		 * Set number of muscles and nodes
		 */
		int numberOfNodes;
		int numberOfMuscles;
		if(Constants.randomNumbers) {
			numberOfNodes = Random.Range (3, 6);
			numberOfMuscles = Random.Range (numberOfNodes, numberOfNodes * 4);
		} else {
			numberOfNodes = Constants.numberOfNodes;
			numberOfMuscles = Constants.numberOfMuscles;
		}
		return RandomCreature (cycleDuration, numberOfNodes, numberOfMuscles);
	}

	public static Creature RandomCreature (float cycleDuration, int numberOfNodes, int numberOfMuscles) {
		/*
		 * Create creature
		 */
		var parent = new GameObject ().transform;
		parent.name = "Creature " + Random.Range (0, 10000).ToString();
		/*
		 * Define arrays
		 */ 
		var nodes = new List<Node> (numberOfNodes);
		var muscles = new List<Muscle> (numberOfMuscles);

		/*
		 * Generate nodes
     		 */
		for (var i = 0; i < numberOfNodes; i++)
		{
			nodes.Add(Node.RandomNode(new Vector2 (Random.value, Random.value + 1) * 10, parent, i));
		}

		/*
		 * Recenter nodes
		 */
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

		/*
		 * Generate muscles
		 */
		Color color = Random.ColorHSV ();
		var k = 0;
		while (k < (nodes.Count - 1) * nodes.Count/2 && k < numberOfMuscles) {
			//Random connection
			var t = new Tuple (Random.Range (0, nodes.Count), Random.Range (0, nodes.Count));

			bool alreadyAdded = false;

			if (t.a == t.b) {
				alreadyAdded = true;
			} else {
				foreach (var muscle in muscles) {
					if (muscle.Equals (t)) {
						alreadyAdded = true;
						break;
					}
				}
			}

			if (!alreadyAdded) {
				muscles.Add(Muscle.RandomMuscle(nodes[t.a], nodes[t.b], cycleDuration, color, parent));
				k++;
			}
		}

		/*
		 * Update graphics
		 */
		foreach(var m in muscles) {
			m.UpdateGraphics ();
		}
		foreach(var n in nodes) {
			n.UpdateGraphics ();
		}
		return new Creature (muscles, nodes, cycleDuration, parent);
	}

	public void Destroy () {
		foreach(var m in muscles) {
			m.Destroy ();
		} 
		foreach(var n in nodes) {
			n.Destroy ();
		}
		MonoBehaviour.Destroy (transform.gameObject);
	}
}