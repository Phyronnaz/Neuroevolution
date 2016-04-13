using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//using System.Diagnostics;

public class Generate : MonoBehaviour
{
	public Text CycleText, DistanceText, TimeText;

	List<Node> nodes;
	List<Muscle> muscles;
	List<Creature> creatures;
	float cycleDuration;
	bool generated;
	bool isCreatingMuscle;
	Node nodeBeeingAssociated;
	Transform currentCreature;
	Color currentColor;


	public void Start ()
	{
		#region Tests
//		var s = new Stopwatch ();
//		float x = 0;
//		s.Start ();
//		var a = Vector2.up;
//		var b = Vector2.right;
//		for (var k = 0; k < 1000000; k++) {
//			x += (a - b).magnitude;
//		}
//		print (s.Elapsed);
//
//		s = new Stopwatch ();
//		x = 0;
//		s.Start ();
//		for (var k = 0; k < 1000000; k++) {
//			x += (a - b).sqrMagnitude;
//		}
//		print (s.Elapsed);
//
//		s = new Stopwatch ();
//		x = 0;
//		s.Start ();
//		for (var k = 0; k < 1000000; k++) {
//			x += Vector2.Distance (a, b);
//		}
//		print (s.Elapsed);
//
//		s = new Stopwatch ();
//		x = 0;
//		s.Start ();
//		for (var k = 0; k < 1000000; k++) {
//			x += (a - b).normalized.x;
//		}
//		print (s.Elapsed);
		#endregion

		Random.seed = 1000;

		//Initialize arrays && variables
		nodes = new List<Node> ();
		muscles = new List<Muscle> ();
		creatures = new List<Creature> ();
		generated = false;
		isCreatingMuscle = false;

		//Camera
		var p = transform.position;
		p.x = 0;
		transform.position = p;

		//Cycle duration
		cycleDuration = (Random.value + 0.1f) * Constants.CycleDurationMultiplier;

		#region ChildNode
//		generated = true;
//		Constants.GravityMultiplier = 0;
//		Constants.Generate = false;
//
//		GenerateNode (Vector2.up * 10);
//		GenerateNode (Vector2.up * 20);
//		GenerateNode (Vector2.up * 20 + Vector2.right * 10);
//
//		Constants.StrengthAmplitude = 0;
//		Constants.ExtendedDistanceMultiplier = 0;
//		Constants.ContractedDistanceMultiplier = 0;
//		GenerateMuscle (nodes [0], nodes [1]);
//		GenerateMuscle (nodes [1], nodes [2]);
//
//		nodes.Add (new ChildNode (0.5f, nodes [0], nodes [1], nodes.Count));
//		nodes.Add (new ChildNode (0.5f, nodes [1], nodes [2], nodes.Count));
//		Constants.StrengthAmplitude = 10f / 10000f;
//		Constants.ExtendedDistanceMultiplier = 2;
//		Constants.ContractedDistanceMultiplier = 10;
//		GenerateMuscle (nodes [3], nodes [4]);
//		AddCreature ();
//		InitializeController ();
		#endregion


		//Generate
		if (Constants.Generate) {
			for (var k = 0; k < 200; k++) {
				AddRandomCreature ();
			}
			InitializeController ();
			generated = true;
		} else {
			currentCreature = new GameObject ().transform;
			currentCreature.name = "Creature " + Random.Range (0, 10000);
			currentColor = Color.black;//Random.ColorHSV ();
			//HACK
//			GenerateNode (Vector2.up * 10);
//			GenerateNode (Vector2.up * 20 + Vector2.right * 5);
//			GenerateNode (Vector2.up * 10 + Vector2.right * 10);
//
//			GenerateMuscle (nodes [0], nodes [1]);
//			GenerateMuscle (nodes [1], nodes [2]);
//			GenerateMuscle (nodes [2], nodes [0]);
//
//			AddCreature ();
		}
	}

	public void Update ()
	{
		if (!generated) {
			if (isCreatingMuscle)
				RenderMuscleEditor ();

			if (EventSystem.current.currentSelectedGameObject == null)
				CheckInput ();
		}
	}

	public void Restart ()
	{
		// Destroy nodes && muscles
		foreach (var n in nodes) {
			n.Destroy ();
		}
		foreach (var m in muscles) {
			m.Destroy ();
		}
		foreach (var c in creatures) {
			c.Destroy ();
		}
		// Reset UI
		CycleText.text = "";
		TimeText.text = "";
		DistanceText.text = "";
		if (GetComponent<LineRenderer> () != null)
			Destroy (GetComponent<LineRenderer> ());
		// Regenerate
		Start ();
	}

	void RenderMuscleEditor ()
	{
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePosition.z = 0;
		
		var l = GetComponent<LineRenderer> ();
		if (l == null)
			l = gameObject.AddComponent<LineRenderer> ();
		
		l.SetPosition (0, nodeBeeingAssociated.Position);
		l.SetPosition (1, mousePosition);
		if (l.material == null)
			l.material = new Material (Shader.Find ("Diffuse"));
		l.material.color = currentColor;
	}

	void CheckInput ()
	{
		if (Input.GetMouseButtonDown (0)) {
			var hasDoneSmthg = false;
			
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
			if (hit.collider != null) {
				if (hit.collider.GetComponent<NodeRenderer> () != null) {
					var hitNode = nodes [hit.transform.GetComponent<NodeRenderer> ().Id];
					
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
		if (Input.GetMouseButtonDown (1) && isCreatingMuscle) {
			isCreatingMuscle = false;
			Destroy (GetComponent<LineRenderer> ());
		}
		if (Input.GetKeyDown (KeyCode.Space) && isCreatingMuscle) {
			isCreatingMuscle = false;
			Destroy (GetComponent<LineRenderer> ());
			AddCreature ();
		}
		if (Input.GetKeyDown (KeyCode.Escape) && creatures.Count > 0) {
			generated = true;
			Destroy (currentCreature.gameObject);
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
		if (Input.GetKeyDown (KeyCode.G)) {
			var c = creatures [creatures.Count - 1];
			for (var k = 0; k < 10; k++) {
				AddRandomCreature (c, 0.1f);
			}
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			Restart ();
		}
	}

	void GenerateMuscle (Node left, Node right)
	{
		muscles.Add (Muscle.RandomMuscle (left, right, cycleDuration, currentColor, currentCreature));
	}

	void GenerateMuscle (int a, int b)
	{
		GenerateMuscle (nodes [a], nodes [b]);
	}

	void GenerateNode (Vector2 position)
	{
		nodes.Add (Node.RandomNode (position, currentCreature, currentColor, nodes.Count));
	}

	bool IsMuscleAlreadyAdded (Node left, Node right)
	{
		return IsMuscleAlreadyAdded (left.Id, right.Id);
	}

	bool IsMuscleAlreadyAdded (Tuple t)
	{
		return IsMuscleAlreadyAdded (t.a, t.b);
	}

	bool IsMuscleAlreadyAdded (int left, int right)
	{
		if (left == right)
			return true;
		foreach (var muscle in muscles) {
			if (muscle.Equals (new Tuple (left, right)))
				return true;
		}
		return false;
	}

	void InitializeController ()
	{
		if (creatures.Count == 0)
			return;
		var controllerScript = gameObject.AddComponent<ControllerScript> ();
		controllerScript.CycleText = CycleText;
		controllerScript.DistanceText = DistanceText;
		controllerScript.TimeText = TimeText;
		controllerScript.Initialize (creatures);
		foreach (var n in nodes) {
			n.Destroy ();
		}
		foreach (var m in muscles) {
			m.Destroy ();
		}
		nodes.Clear ();
		muscles.Clear ();
	}

	void AddCreature ()
	{
		creatures.Add (new Creature (muscles, nodes, cycleDuration, currentCreature));
		muscles = new List<Muscle> ();
		nodes = new List<Node> ();
		currentCreature = new GameObject ().transform;
		currentCreature.name = "Creature " + Random.Range (0, 10000);
		currentColor = Color.black;//Random.ColorHSV ();
	}

	void AddRandomCreature ()
	{
		creatures.Add (Creature.RandomCreature (cycleDuration));
	}

	void AddRandomCreature (Creature creature, float variation)
	{
		creatures.Add (Creature.RandomCreature (creature, variation, currentColor));
	}
}