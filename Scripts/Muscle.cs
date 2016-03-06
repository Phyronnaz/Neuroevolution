using UnityEngine;
using System.Collections;

public class Muscle {
	
	#region public variables
	public float strength;
	public float extendedLength;
	public float contractedLength;
	public float contractionTime;
	#endregion

	#region private variables
	private Node left;
	public Node right;
	private MuscleRenderer muscleRenderer;
	#endregion


	#region Constructor
	public Muscle (Node left, Node right, float strength, float extendedLength, float contractedLength, float contractionTime) {
		this.left = left;
		this.right = right;
		this.strength = strength;
		this.extendedLength = extendedLength;
		this.contractedLength = contractedLength;
		this.contractionTime = contractionTime;

		//Create muscle renderer
		muscleRenderer = (new GameObject()).AddComponent<MuscleRenderer> ();
		muscleRenderer.gameObject.name = "Muscle from " + left.id.ToString () + " to " + right.id.ToString ();
		muscleRenderer.maxLength = extendedLength;
		muscleRenderer.minLength = contractedLength;
		muscleRenderer.Initialize ();
		LateUpdate ();
	}
	#endregion

	#region Update and LateUpdate
	public void Update (float time) {
		var distance = Vector2.Distance (left.position, right.position);

		var l = (distance - contractedLength) / (extendedLength - contractedLength) * 2 - 1;
			
		var f = Mathf.Exp (-1 / Mathf.Pow (l, 2)) * Mathf.Pow (l, 1 / 1) + ((time < contractionTime) ? 1 : -1);
//		var f = Mathf.Pow (l, 3) + ((time < contractionTime) ? 1 : -1);

		var center = (left.position + right.position) / 2;
		left.AddForce (f * strength * (center - left.position).normalized);
		right.AddForce (f * strength * (center - right.position).normalized);
	}

	public void LateUpdate () {
		var distance = Vector2.Distance (left.position, right.position);

		//Width
		var width = Mathf.Lerp (0.1f, 1, contractedLength / distance);
		muscleRenderer.SetWidth (width);

		//Position
		muscleRenderer.SetPosition (left.position, right.position);
	}
	#endregion


	#region Destroy
	public void Destroy () {
		MonoBehaviour.Destroy (muscleRenderer.gameObject);
	}
	#endregion

	#region Equals && Hash
	public override bool Equals (object obj)
	{
		if(obj.GetType() == typeof(Tuple)) {
			var t = (Tuple)obj;
			return (left.id == t.a && right.id == t.b) || (left.id == t.b && right.id == t.a);
		} else {
			return false;
		}
	}

	public override int GetHashCode ()
	{
		return left.GetHashCode () + right.GetHashCode ();
	}
	#endregion
}