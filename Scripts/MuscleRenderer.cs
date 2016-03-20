﻿using UnityEngine;
using System.Collections;

public class MuscleRenderer : MonoBehaviour {private Material material;
	private LineRenderer lineRenderer;
	private LineRenderer max;
	private LineRenderer min;
	private bool isContracting = false;

	public float maxLength;
	public float minLength;

	public Color color;

	public void Initialize () {
		lineRenderer = gameObject.AddComponent<LineRenderer> ();
		material = new Material(Shader.Find("Diffuse"));
		material.color = color;
		lineRenderer.material = material;

		max = new GameObject ().AddComponent<LineRenderer> ();
		min = new GameObject ().AddComponent<LineRenderer> ();

		max.transform.parent = transform;
		min.transform.parent = transform;

		max.SetWidth (0.25f, 0.25f);
		min.SetWidth (0.25f, 0.25f);

		max.material = new Material (Shader.Find ("Transparent/Diffuse"));
		max.material.color = new Color (0, 1, 0, 0.5f);
		min.material = new Material (Shader.Find ("Transparent/Diffuse"));
		min.material.color = new Color (0, 1, 0, 0.5f);
	}

	public void SetWidthAndColor(float width, bool contracting) {
		lineRenderer.SetWidth (width, width);
		if(isContracting != contracting) {
			isContracting = contracting;
			if(contracting) {
				max.material.color = new Color (1, 0, 0, 0.5f);
				min.material.color = new Color (1, 0, 0, 0.5f);
			} else {
				max.material.color = new Color (0, 1, 0, 0.5f);
				min.material.color = new Color (0, 1, 0, 0.5f);
			}
		}
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