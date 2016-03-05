using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {
	
	public float friction;
	public float nodeRadius;
	public Vector2 position;
	public float mass;

	public int id;

	private NodeRenderer nodeRenderer;
	private Vector2 forcesSum = Vector3.zero;

	private Vector2 previousSpeed = Vector2.zero;

	public Node (float friction, Vector2 position, float mass, int id) {
		this.friction = friction;
		this.position = position;
		this.mass = mass;
		this.id = id;

		var go = GameObject.Instantiate (Resources.Load ("Circle"), position, Quaternion.identity) as GameObject;
		go.name = "Node " + id.ToString ();
		nodeRenderer = go.AddComponent<NodeRenderer> ();
		go.GetComponent<SpriteRenderer> ().color = new Color (1 - friction / 2 / Constants.frictionAmplitude, 0, 0);

		nodeRadius = go.GetComponent<SpriteRenderer> ().bounds.extents.x;
	}

	public void Update (float deltaTime) {
		//Weight
		AddForce (Vector2.down * Constants.gravityMultiplier);


		if (position.y < nodeRadius + Constants.tolerance) {
			//Support
//			forcesSum.y = -previousSpeed.y / deltaTime;
			var Rn = -forcesSum.y - previousSpeed.y / deltaTime;
			AddForce (Vector2.up * Rn);
			//Friction
//			if (previousSpeed.x > Constants.tolerance)
//				AddForce (Vector2.right * friction * Rn);
		}


		//forcesSum = mass * acceleration
		var acceleration = forcesSum / mass;

		//v = a * dt + c
		var speed = acceleration * deltaTime + previousSpeed;

		//delta position = v * dt
		var deltaPosition = speed * deltaTime;

		position += deltaPosition;
			
		forcesSum = Vector2.zero;
		previousSpeed = speed;

//		position.y = Mathf.Max (nodeRadius, position.y);
	}

	public void LateUpdate () {
		nodeRenderer.SetPosition (position);
	}

	public void AddForce (Vector2 force) {
		forcesSum += force;
	}

	public void Destroy () {
		MonoBehaviour.Destroy (nodeRenderer.gameObject);
	}
}

public class NodeRenderer : MonoBehaviour {
	public void SetPosition (Vector2 position) {
		transform.position = (Vector3)position;
	}
}
