using UnityEngine;
using System.Collections;

public class MuscleRenderer : MonoBehaviour {private Material material;
	private LineRenderer lineRenderer;
	private LineRenderer max;
	private LineRenderer min;

	public float maxLength;
	public float minLength;

	public void Initialize () {
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
		max.SetPosition (0, (Vector3)(center - left).normalized * maxLength / 2 + (Vector3)center + Vector3.back);
		max.SetPosition (1, (Vector3)(center - right).normalized * maxLength / 2 + (Vector3)center + Vector3.back);

		min.SetPosition (0, (Vector3)(center - left).normalized * minLength / 2 + (Vector3)center + Vector3.back);
		min.SetPosition (1, (Vector3)(center - right).normalized * minLength / 2 + (Vector3)center + Vector3.back);
	}
}