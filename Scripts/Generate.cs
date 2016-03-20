using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

	private bool isCreatingMuscle;
	private Node nodeBeeingAssociated;
	#endregion


	#region Start and Update
	void Start ()
	{
		//Initialize arrays && variables
		nodes = new List<Node> ();
		muscles = new List<Muscle> ();
		generated = false;
		isCreatingMuscle = false;

		//Cycle duration
		cycleDuration = 10;//(Random.value + 0.1f) * Constants.cycleDurationMultiplier;

		/////////////////////////////////////////////
		//HACK
		/////////////////////////////////////////////
		generated = true;
		Constants.gravityMultiplier = 0;
		Constants.generate = false;

		GenerateNode (Vector2.up * 10);
		GenerateNode (Vector2.up * 20);
		GenerateNode (Vector2.one * 20);

		Constants.strengthAmplitude = 0;
		Constants.extendedDistanceMultiplier = 0;
		Constants.contractedDistanceMultiplier = 0;
//		GenerateMuscle (nodes[0], nodes[1]);
//		GenerateMuscle (nodes[1], nodes[2]);

		nodes.Add (new ChildNode (0.5f, nodes [0], nodes [1], nodes.Count));
		nodes.Add (new ChildNode (0.5f, nodes [1], nodes [2], nodes.Count));
		Constants.strengthAmplitude = 1000;
		Constants.extendedDistanceMultiplier = 2;
		Constants.contractedDistanceMultiplier = 10;
		GenerateMuscle (nodes[3], nodes[4]);

		InitializeController ();


		/////////////////////////////////////////////
		/////////////////////////////////////////////
		/////////////////////////////////////////////

		//Generate
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
			if(isCreatingMuscle) {
				Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePosition.z = 0;
					
				var l = GetComponent<LineRenderer> ();
				if (l == null)
					l = gameObject.AddComponent<LineRenderer> ();
				
				l.SetPosition (0, nodeBeeingAssociated.position);
				l.SetPosition (1, mousePosition);
			}
			if (EventSystem.current.currentSelectedGameObject == null) {
				if (Input.GetMouseButtonDown (0)) {
					var hasDoneSmthg = false;

					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
					if (hit.collider != null) {
						if (hit.collider.GetComponent<NodeRenderer> () != null) {
							var hitNode = nodes [hit.transform.GetComponent<NodeRenderer> ().id];

							if (isCreatingMuscle && !IsMuscleAlreadyAdded (hitNode, nodeBeeingAssociated))
								GenerateMuscle (hitNode, nodeBeeingAssociated);
						
							nodeBeeingAssociated = hitNode;

							hasDoneSmthg = true;
						}
					}
					if (!hasDoneSmthg) {
						Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
						GenerateNode (mousePosition);

						var lastNode = nodes [nodes.Count - 1];

						if (isCreatingMuscle && !IsMuscleAlreadyAdded (lastNode, nodeBeeingAssociated))
							GenerateMuscle (lastNode, nodeBeeingAssociated);
					
						nodeBeeingAssociated = lastNode;
					}
					isCreatingMuscle = true;
				}
				if (Input.GetMouseButtonDown (1)) {
					if (isCreatingMuscle) {
						isCreatingMuscle = false;
						Destroy (GetComponent<LineRenderer> ());
					}
				}
				if (Input.GetKeyDown (KeyCode.Space)) {
					generated = true;
					if (GetComponent<LineRenderer> () != null)
						Destroy (GetComponent<LineRenderer> ());
					InitializeController ();
				}
				if (Input.GetKeyDown (KeyCode.Return)) {
					var k = 0;
					var alreadyAdded = muscles.Count;
					while (k < (nodes.Count - 1) * nodes.Count / 2 - alreadyAdded) {
						//Random connection
						var t = new Tuple (Random.Range (0, nodes.Count), Random.Range (0, nodes.Count));


						if (!IsMuscleAlreadyAdded (t)) {
							GenerateMuscle (t.a, t.b);
							k++;
						}
					}
				}
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

	#region Generate muscle && node && check if muscle add
	private void GenerateMuscle (Node left, Node right) {
		var distance = Vector2.Distance (left.position, right.position);
		var contractedLength = distance - Random.Range (Constants.minRandom, Constants.contractedDistanceMultiplier);
		var extendedLength = distance + Random.Range (Constants.minRandom, Constants.extendedDistanceMultiplier);
		var strength = Random.Range (Constants.minStrength, Constants.strengthAmplitude);
		var cycleDuration = Random.Range (Constants.minRandom, this.cycleDuration);
		cycleDuration = this.cycleDuration / 2;
		var beginWithContraction = (Random.value > 0.5f);

		muscles.Add (new Muscle (left, right, strength, extendedLength, contractedLength, cycleDuration, beginWithContraction));
	}

	private void GenerateMuscle (int a, int b) {
		GenerateMuscle (nodes [a], nodes [b]);
	} 

	private void GenerateNode (Vector2 position) {
		var friction = Random.Range (Constants.minRandom, Constants.frictionAmplitude);
		var mass = Random.Range (Constants.minMass, Constants.maxMass);

		nodes.Add (new Node (friction, position, mass, Constants.bounciness, nodes.Count));
	}

	private void GenerateNode () {
		GenerateNode (new Vector2 (Random.Range (-40f, 40f), Random.Range (0f, 20f)));
	}

	private bool IsMuscleAlreadyAdded (Node left, Node right) {
		return IsMuscleAlreadyAdded (left.id, right.id);
	}

	private bool IsMuscleAlreadyAdded (Tuple t) {
		return IsMuscleAlreadyAdded (t.a, t.b);
	} 

	private bool IsMuscleAlreadyAdded (int left, int right) {
		if (left == right)
			return true;
		foreach(var muscle in muscles) {
			if (muscle.Equals (new Tuple (left, right)))
				return true;
		}
		return false;
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
		for (var i = 0; i < numberOfNodes; i++)
		{
			GenerateNode (new Vector2 (Random.value, Random.value + 1) * 10);
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
		var k = 0;
		while (k < (nodes.Count - 1) * nodes.Count/2 && k < numberOfMuscles) {
			//Random connection
			var t = new Tuple (Random.Range (0, nodes.Count), Random.Range (0, nodes.Count));


			if (!IsMuscleAlreadyAdded (t)) {
				GenerateMuscle (t.a, t.b);
				k++;
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