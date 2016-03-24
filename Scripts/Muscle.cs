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
		//TODO: Square
		var l = Vector2.Distance (left.position, right.position);
		var center = (left.position + right.position) / 2;

		float force;
		if (l > extendedLength)
			force = (l - extendedLength);
		else if (l < contractedLength)
			force = (l - contractedLength);
		else
			force = (time > changeTime && !beginWithContraction) || (time < changeTime && beginWithContraction) ?
				1 * Mathf.Clamp(l-contractedLength, -1, 1) : -1 * Mathf.Clamp(l-extendedLength, -1, 1);
		
		//TODO: Remove normalized
		left.AddVelocity (strength * force * (center - left.position).normalized);
		right.AddVelocity (strength * force * (center - right.position).normalized);


//		var l = (left.position - right.position).sqrMagnitude;
//
//		var extendedLengthSqr = Mathf.Pow (extendedLength, 2);
//		var contractedLengthSqr = Mathf.Pow (contractedLength, 2);
//
//		float force = (time > changeTime && !beginWithContraction) || (time < changeTime && beginWithContraction) ? 1 : -1;
//		if (l > extendedLengthSqr)
//			force += Mathf.Pow (l - extendedLengthSqr + 1, 2);
//		else if (l < contractedLengthSqr)
//			force -= Mathf.Pow (l - contractedLengthSqr - 1, 2);
//
//		var center = (left.position + right.position) / 2;
//		var direction = (center - left.position) / (center - left.position).magnitude;
//
//		left.AddVelocity (strength * force * direction);
//		right.AddVelocity (-strength * force * direction);
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
		var beginWithContraction = (Random.value > 0.5f);

		return new Muscle (left, right, strength, extendedLength, contractedLength, muscleCycleDuration, beginWithContraction, color, parent);
	}
}