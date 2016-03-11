using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {
	
	#region public variables
	public float friction;
	public float mass;
	public float nodeRadius;
	public float coefficientOfRestitution;
	public int id;

	public Vector2 position;
	#endregion

	#region private variables
	private NodeRenderer nodeRenderer;
	private Vector2 forcesSum = Vector3.zero;
	private List<Vector2> constraints = new List<Vector2> ();
	private Vector2 previousSpeed = Vector2.zero;
	#endregion


	#region Constructor
	public Node (float friction, Vector2 position, float mass, float coefficientOfRestitution, int id) {
		this.friction = friction;
		this.position = position;
		this.mass = mass;
		this.coefficientOfRestitution = coefficientOfRestitution;
		this.id = id;

		//Create node renderer
		var go = GameObject.Instantiate (Resources.Load ("Circle"), position, Quaternion.identity) as GameObject;
		go.name = "Node " + id.ToString ();
		go.GetComponent<SpriteRenderer> ().color = new Color (friction / Constants.frictionAmplitude, 0, 0);
		nodeRenderer = go.AddComponent<NodeRenderer> ();
		nodeRenderer.id = id;

		nodeRadius = go.GetComponent<SpriteRenderer> ().bounds.extents.x;
	}
	#endregion

	#region Update and LateUpdate
	public void Update (float deltaTime) {
		//Weight
		AddForce (Vector2.down * Constants.gravityMultiplier * mass);


//		if (position.y < nodeRadius + Constants.tolerance) {
//			//Support
////			forcesSum.y = -previousSpeed.y / deltaTime;
//			var Rn = -forcesSum.y - previousSpeed.y / deltaTime;
//			AddForce (Vector2.up * Rn);
//			//Friction
////			if (previousSpeed.x > Constants.tolerance)
////				AddForce (Vector2.right * friction * Rn);
//		}


		//forcesSum = mass * acceleration
		var acceleration = forcesSum / mass;

		//v = a * dt + c
		var velocity = acceleration * deltaTime + previousSpeed;

		foreach(var c in constraints) {
			velocity -= Vector2.Dot (velocity, c) * c;
		}

		if ((velocity * deltaTime + position).y < nodeRadius) {
			if (position.y > nodeRadius + Constants.tolerance)
				velocity.y = (nodeRadius - position.y) / deltaTime;
			else
				velocity.y -= (1 + coefficientOfRestitution) * velocity.y;
		}

		if (position.y < nodeRadius + Constants.tolerance)
			velocity.x = 0;// (1 + friction);

		velocity /= Constants.musclesReaction;

		//delta position = v * dt
		var deltaPosition = velocity * deltaTime;

		position += deltaPosition;
			
		forcesSum = Vector2.zero;

		previousSpeed = velocity;
	}

	public void LateUpdate () {
		nodeRenderer.SetPosition (position);
	}
	#endregion

	#region AddForce and other
	public void AddForce (Vector2 force) {
		forcesSum += force;
	}

	public void AddConstraint (Vector2 constraint) {
		constraints.Add (constraint);
	}
	#endregion


	#region Destroy
	public void Destroy () {
		MonoBehaviour.Destroy (nodeRenderer.gameObject);
	}
	#endregion
}