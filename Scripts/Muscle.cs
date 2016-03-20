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
	private bool isContracting = false;
	private Node left;
	private Node right;
	private MuscleRenderer muscleRenderer;
	#endregion


	#region Constructor
	public Muscle (Node left, Node right, float strength, float extendedLength, float contractedLength, float changeTime, bool beginWithContraction, Color color, Transform parent) {
		this.left = left;
		this.right = right;
		this.strength = strength;
		this.extendedLength = Mathf.Max(extendedLength, Constants.minRandom);
		this.contractedLength = Mathf.Max (contractedLength, Constants.minRandom);
		this.changeTime = changeTime;
		this.beginWithContraction = beginWithContraction;

		//Create muscle renderer
		muscleRenderer = (new GameObject()).AddComponent<MuscleRenderer> ();
		muscleRenderer.gameObject.name = "Muscle from " + left.id.ToString () + " to " + right.id.ToString ();
		muscleRenderer.maxLength = this.extendedLength;
		muscleRenderer.minLength = this.contractedLength;
		muscleRenderer.color = color;
		muscleRenderer.transform.parent = parent;
		muscleRenderer.Initialize ();
		LateUpdate ();
	}
	#endregion

	#region Update and LateUpdate
	public void Update (float time) {
//
//		float l0;
//		if ((time > changeTime && !beginWithContraction) || (time < changeTime && beginWithContraction))
//			l0 = contractedLength;
//		else
//			l0 = extendedLength;
//
//		var l = Vector2.Distance (left.position, right.position);
//
////		float mul = 1;
////		if ((l < l0 && l0 == contractedLength) || (l > l0 && l0 == extendedLength))
////			mul = strength;
//		
//		var center = (left.position + right.position) / 2;
////		var force = strength * Mathf.Clamp(l - l0, -1, 1) * mul;
//		var force = strength * (l - l0);
//		isContracting = force > 0;
//		left.AddForce (force * (center - left.position).normalized);
//		right.AddForce (force * (center - right.position).normalized);
//
//		if ((l < l0 && l0 == contractedLength) || (l > l0 && l0 == extendedLength)) {
//			left.AddVelocity ((center - left.position).normalized);
//			right.AddVelocity ((center - right.position).normalized);
//		}
		float l0;
		if ((time > changeTime && !beginWithContraction) || (time < changeTime && beginWithContraction))
			l0 = contractedLength;
		else
			l0 = extendedLength;

		//TODO: Square
		var l = Vector2.Distance (left.position, right.position);
		var center = (left.position + right.position) / 2;

		//TODO: Remove normalized
		left.AddForce (strength * (l - l0) * (center - left.position).normalized);
		right.AddForce (strength * (l - l0) * (center - right.position).normalized);
	}

	public void LateUpdate () {
		var distance = Vector2.Distance (left.position, right.position);

		//Width
		var width = Mathf.Lerp (0.1f, 1, contractedLength / distance);
		muscleRenderer.SetWidthAndColor (width, isContracting);

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

	public static Muscle RandomMuscle (Node left, Node right, float cycleDuration, Color color, Transform parent) {
		var distance = Vector2.Distance (left.position, right.position);
		var contractedLength = distance - Random.Range (Constants.minRandom, Constants.contractedDistanceMultiplier);
		var extendedLength = distance + Random.Range (Constants.minRandom, Constants.extendedDistanceMultiplier);
		var strength = Random.Range (Constants.minStrength, Constants.strengthAmplitude);
		var muscleCycleDuration = Random.Range (Constants.minRandom, cycleDuration);
		//HACK
		muscleCycleDuration = cycleDuration / 2;
		var beginWithContraction = (Random.value > 0.5f);

		return new Muscle (left, right, strength, extendedLength, contractedLength, muscleCycleDuration, beginWithContraction, color, parent);
	}
}