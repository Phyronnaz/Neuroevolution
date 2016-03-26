using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildNode : Node {
	
	#region public variables
	public Node left;
	public Node right;
	public float normalizedDistanceFromLeft;
	#endregion


	#region Constructor
	public ChildNode (float normalizedDistanceFromLeft, Node left, Node right, int id) {
		this.normalizedDistanceFromLeft = normalizedDistanceFromLeft;
		this.left = left;
		this.right = right;
		this.id = id;

		position = left.position + (right.position - left.position) / 2;

		//Create node renderer
		var go = GameObject.Instantiate (Resources.Load ("Circle"), position, Quaternion.identity) as GameObject;
		go.name = "Node " + id.ToString ();
		go.GetComponent<SpriteRenderer> ().color = Color.blue;
		nodeRenderer = go.AddComponent<NodeRenderer> ();
		nodeRenderer.id = id;

		nodeRadius = go.GetComponent<SpriteRenderer> ().bounds.extents.x;
	}
	#endregion

	#region Update
	public override void Update (float deltaTime) {
		position = left.position + (right.position - left.position) / 2;

		left.AddVelocity (velocitySum * (1 - normalizedDistanceFromLeft));
		right.AddVelocity (velocitySum * normalizedDistanceFromLeft);

		left.AddConstraint (constraintSum * (1 - normalizedDistanceFromLeft));
		right.AddConstraint (constraintSum * normalizedDistanceFromLeft);

		forcesSum = Vector2.zero;
	}
	#endregion
}