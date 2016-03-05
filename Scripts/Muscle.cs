using UnityEngine;
using System.Collections;

public class Muscle {
	public float strength;
	public float extendedLength;
	public float contractedLength;

	public float contractionTime;

	public Node left;
	public Node right;

	private MuscleRenderer muscleRenderer;

	public Muscle (Node left, Node right, float strength, float extendedLength, float contractedLength, float contractionTime) {
		this.left = left;
		this.right = right;
		this.strength = strength;
		this.extendedLength = extendedLength;
		this.contractedLength = contractedLength;
		this.contractionTime = contractionTime;

		muscleRenderer = (new GameObject()).AddComponent<MuscleRenderer> ();
		muscleRenderer.gameObject.name = "Muscle from " + left.id.ToString () + " to " + right.id.ToString ();
		muscleRenderer.maxLength = extendedLength;
		muscleRenderer.minLength = contractedLength;
	}

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

	public void Destroy () {
		MonoBehaviour.Destroy (muscleRenderer.gameObject);
	}
}

public class MuscleRenderer : MonoBehaviour {private Material material;
	private LineRenderer lineRenderer;
	private LineRenderer max;
	private LineRenderer min;

	public float maxLength;
	public float minLength;

	void Start () {
		lineRenderer = gameObject.AddComponent<LineRenderer> ();
		material = new Material(Shader.Find("Diffuse"));
		material.color = new Color (Random.value, Random.value, Random.value);
		lineRenderer.material = material;

		max = new GameObject ().AddComponent<LineRenderer> ();
		min = new GameObject ().AddComponent<LineRenderer> ();

		max.transform.parent = transform;
		min.transform.parent = transform;

		max.SetWidth (0.25f, 0.25f);
		min.SetWidth (0.25f, 0.25f);

		max.material = new Material (Shader.Find ("Transparent/Diffuse"));
		max.material.color = new Color (1, 0, 0, 0.5f);
		min.material = new Material (Shader.Find ("Transparent/Diffuse"));
		max.material.color = new Color (0, 1, 0, 0.5f);
	}

	public void SetWidth(float width) {
		lineRenderer.SetWidth (width, width);
	}

	public void SetPosition (Vector2 left, Vector2 right) {
		lineRenderer.SetPosition (0, (Vector3)left + Vector3.forward);
		lineRenderer.SetPosition (1, (Vector3)right + Vector3.forward);
		var center = (left + right) / 2;
		max.SetPosition (0, (Vector3)center - Vector3.left * maxLength / 2);
		max.SetPosition (1, (Vector3)center - Vector3.right * maxLength / 2);

		min.SetPosition (0, (Vector3)center - Vector3.left * minLength / 2);
		min.SetPosition (1, (Vector3)center - Vector3.right * minLength / 2);

		var r = Quaternion.FromToRotation (Vector3.right, right - left);
		max.transform.rotation = r;
		min.transform.rotation = r;
	}
}
