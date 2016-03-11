using UnityEngine;
using System.Collections;

public class Muscle {
	
	#region public variables
	public float strength;
	public float extendedLength;
	public float contractedLength;
	public float changeTime;
	public bool beginWithContraction;
	#endregion

	#region private variables
	private Node left;
	public Node right;
	private MuscleRenderer muscleRenderer;
	#endregion


	#region Constructor
	public Muscle (Node left, Node right, float strength, float extendedLength, float contractedLength, float changeTime, bool beginWithContraction) {
		this.left = left;
		this.right = right;
		this.strength = strength;
		this.extendedLength = extendedLength;
		this.contractedLength = contractedLength;
		this.changeTime = changeTime;
		this.beginWithContraction = beginWithContraction;

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

		float l0;
		if ((time > changeTime && !beginWithContraction) || (time < changeTime && beginWithContraction))
			l0 = contractedLength;
		else
			l0 = extendedLength;

		var l = Vector2.Distance (left.position, right.position);
				
		var center = (left.position + right.position) / 2;
		left.AddForce (strength * (l - l0) * (center - left.position).normalized);
		right.AddForce (strength * (l - l0) * (center - right.position).normalized);
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