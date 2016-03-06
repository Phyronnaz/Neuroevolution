using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Generate : MonoBehaviour {

	#region public variables
	public Text cycleText;
	public Text distanceText;
	public Text timeText;
	#endregion

	#region private variables
	private List<Node> nodes;
	private List<Muscle> muscles;

	private Controller controller;

	private float cycleDuration;

	private bool generated = false;

	private bool muscleEdit;
	private Node nodeEdit;
	#endregion


	#region Start and Update
	void Start ()
	{
		//Initialize arrays
		nodes = new List<Node> ();
		muscles = new List<Muscle> ();

		//Cycle duration
		cycleDuration = (Random.value + 0.1f) * Constants.cycleDurationMultiplier;

		//Generate
		generated = false;
		muscleEdit = false;
		if (Constants.generate) {
			GenerateRandomly ();
			InitializeController ();
			generated = true;
		}
	}

	void Update () {
		//Restart if asked
		if (Input.GetKeyDown (KeyCode.R))
			Restart ();
		//Edit
		if(!generated) {
			if(muscleEdit) {
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var l = GetComponent<LineRenderer> ();
				if (l == null)
					l = gameObject.AddComponent<LineRenderer> ();
				pos.z = 0;
				l.SetPosition (0, nodeEdit.position);
				l.SetPosition (1, pos);
			}
			if(Input.GetMouseButtonDown(0)) {
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				nodes.Add(new Node ((Random.value + 1) * Constants.frictionAmplitude, pos, 1, 0.6f, nodes.Count));
				if(muscleEdit) {
					muscleEdit = false;
					var node = nodes [nodes.Count - 1];

					var t = true;
					foreach(var m in muscles) {
						if (m.Equals (new Tuple (node.id, nodeEdit.id))) {
							t = false;
							break;
						}
					}
					if (t) {
						var distance = Vector2.Distance (node.position, nodeEdit.position);
						var contractedLength = distance - Random.value * Constants.contractedDistanceMultiplier;
						var extendedLength = distance + Random.value * Constants.extendedDistanceMultiplier;

						muscles.Add (new Muscle (
							node,
							nodeEdit,
							(Random.value + 0.01f) * Constants.strengthAmplitude,
							extendedLength,
							contractedLength,
							cycleDuration / 2//Random.value * cycleDuration
						));
					}

					Destroy (GetComponent<LineRenderer> ());
				}
			}
			if (Input.GetMouseButtonDown (1)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
				if (hit.collider != null) {
					if (hit.collider.GetComponent<NodeRenderer> () != null) {
						if (!muscleEdit) {
							muscleEdit = true;
							nodeEdit = nodes [hit.transform.GetComponent<NodeRenderer> ().id];
						} else if (hit.transform.GetComponent<NodeRenderer> ().id != nodeEdit.id) {
							muscleEdit = false;
							var node = nodes [hit.transform.GetComponent<NodeRenderer> ().id];

							var t = true;
							foreach(var m in muscles) {
								if (m.Equals (new Tuple (node.id, nodeEdit.id))) {
									t = false;
									break;
								}
							}
							if (t) {
								var distance = Vector2.Distance (node.position, nodeEdit.position);
								var contractedLength = distance - Random.value * Constants.contractedDistanceMultiplier;
								var extendedLength = distance + Random.value * Constants.extendedDistanceMultiplier;

								muscles.Add (new Muscle (
									node,
									nodeEdit,
									(Random.value + 0.01f) * Constants.strengthAmplitude,
									extendedLength,
									contractedLength,
									cycleDuration / 2//Random.value * cycleDuration
								));
							}

							Destroy (GetComponent<LineRenderer> ());
						}
					}
				}
			}
			if (Input.GetKeyDown (KeyCode.Space)) {
				generated = true;
				if (GetComponent<LineRenderer> () != null)
					Destroy (GetComponent<LineRenderer> ());
				InitializeController ();
			}
				
		}
	}
	#endregion
		

	#region Restart
	public void Restart () {
		/*
		 * Destroy all muscles and nodes
		 */
		foreach(var m in muscles) {
			m.Destroy ();
		} 
		foreach(var n in nodes) {
			n.Destroy ();
		}
		/*
		 * Clear arrays
		 */
		muscles.Clear ();
		nodes.Clear ();
		/*
		 * Destroy controller
		 */
		DestroyImmediate (controller);
		/*
		 * Reset UI
		 */
		cycleText.text = "";
		timeText.text = "";
		distanceText.text = "";
		if (GetComponent<LineRenderer> () != null)
			Destroy (GetComponent<LineRenderer> ());
		/*
		 * Regenerate
		 */
		Start ();
	}
	#endregion

	#region Generate
	private void GenerateRandomly () {
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

		/*
		 * Generate nodes
     		 */
		for (var k = 0; k < numberOfNodes; k++)
		{
			nodes.Add (new Node ((Random.value + 1) * Constants.frictionAmplitude, new Vector2(Random.value, Random.value + 1) * 10, 1, 0.6f, nodes.Count));
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
		//list of connected nodes
		var x = new List<Tuple> ();
		//prevent infinite loop
		int sec = 0;
		for(int k = 0; k < numberOfMuscles; k++) {
			//Random connection
			var a = new Tuple (Random.Range (0, numberOfNodes), Random.Range (0, numberOfNodes));
			//Try to connect all muscles
			if (k < numberOfNodes)
				a.a = k;
			//If muscle not already added
			if (!x.Contains (a) && a.a != a.b) {
				x.Add (a);
				x.Add (a.Reverse ());
				var distance = Constants.averageDistance + (Random.value - 0.5f) * 2 * Constants.distanceAmplitude;
				var contractedLength = distance - Random.value * Constants.contractedDistanceMultiplier;
				var extendedLength = distance + Random.value * Constants.extendedDistanceMultiplier;
				muscles.Add (new Muscle (
					nodes [a.a],
					nodes [a.b],
					(Random.value + 0.01f) * Constants.strengthAmplitude,
					extendedLength,
					contractedLength,
					cycleDuration / 2//Random.value * cycleDuration
				));
			} else if (sec < 100) {
				//Force loop to continue
				k -= 1;
				sec++;
			}
		}

		/*
		 * Update render
		 */
		foreach(var m in muscles) {
			m.LateUpdate ();
		}
		foreach(var n in nodes) {
			n.LateUpdate ();
		}
	}
	#endregion

	#region Initialize controller
	private void InitializeController () {
		controller = gameObject.AddComponent<Controller> ();
		controller.cycleText = cycleText;
		controller.distanceText = distanceText;
		controller.timeText = timeText;
		controller.nodes = nodes;
		controller.muscles = muscles;
		controller.cycleDuration = cycleDuration;
	}
	#endregion
}