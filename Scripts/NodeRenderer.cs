using UnityEngine;
using System.Collections;


public class NodeRenderer : MonoBehaviour {
	public int id;

	public void SetPosition (Vector2 position) {
		transform.position = (Vector3)position;
	}
}