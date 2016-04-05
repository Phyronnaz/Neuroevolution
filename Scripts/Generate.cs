﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Diagnostics;

public class Generate : MonoBehaviour
{
	public Text CycleText, DistanceText, TimeText;

	List<Node> nodes;
	List<Muscle> muscles;
	List<Creature> creatures;

	Controller controller;
	float cycleDuration;
	bool generated;
	bool isCreatingMuscle;
	Node nodeBeeingAssociated;
	Transform currentCreature;
	Color currentColor;


	void Start ()
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

		//HACK
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
			currentColor = Random.ColorHSV ();
		}
	}

	void Update ()
	{
		//Restart if asked
		if (Input.GetKeyDown (KeyCode.R))
			Restart ();
		//Edit
		if (!generated) {
			if (isCreatingMuscle) {
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
			if (EventSystem.current.currentSelectedGameObject == null) {
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
				if (Input.GetMouseButtonDown (1)) {
					if (isCreatingMuscle) {
						isCreatingMuscle = false;
						Destroy (GetComponent<LineRenderer> ());
					}
				}
				if (Input.GetKeyDown (KeyCode.Space)) {
					isCreatingMuscle = false;
					if (GetComponent<LineRenderer> () != null)
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
			}
				
		}
	}


	public void Restart ()
	{
		// Destroy all creatues
		if (controller != null)
			controller.Destroy ();
		
		// Destroy controller
		DestroyImmediate (controller);

		// Destroy nodes && muscles
		foreach (var n in nodes) {
			n.Destroy ();
		}
		foreach (var m in muscles) {
			m.Destroy ();
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
		controller = gameObject.AddComponent<Controller> ();
		controller.CycleText = CycleText;
		controller.DistanceText = DistanceText;
		controller.TimeText = TimeText;
		controller.Creatures = creatures;
		foreach (var n in nodes) {
			n.Destroy ();
		}
		foreach (var m in muscles) {
			m.Destroy ();
		}
	}


	void AddCreature ()
	{
		creatures.Add (new Creature (muscles, nodes, cycleDuration, currentCreature));
		muscles = new List<Muscle> ();
		nodes = new List<Node> ();
		currentCreature = new GameObject ().transform;
		currentCreature.name = "Creature " + Random.Range (0, 10000);
		currentColor = Random.ColorHSV ();
	}

	void AddRandomCreature ()
	{
		creatures.Add (Creature.RandomCreature (cycleDuration));
	}
}