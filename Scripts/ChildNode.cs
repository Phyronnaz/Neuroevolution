using UnityEngine;

public class ChildNode : Node
{
	readonly Node left;
	readonly Node right;
	readonly float normalizedDistanceFromLeft;

	public ChildNode (float normalizedDistanceFromLeft, Node left, Node right, int id) : base (id)
	{
		this.normalizedDistanceFromLeft = normalizedDistanceFromLeft;
		this.left = left;
		this.right = right;

		Position = left.Position + (right.Position - left.Position) / 2;

		//Create node renderer
		var go = Object.Instantiate (Resources.Load ("Circle"), Position, Quaternion.identity) as GameObject;
		go.name = "Node " + id;
		go.GetComponent<SpriteRenderer> ().color = Color.blue;
		NodeRenderer = go.AddComponent<NodeRenderer> ();
		NodeRenderer.Id = Id;
	}

	public override void Update (float deltaTime)
	{
		Position = left.Position + (right.Position - left.Position) / 2;

		left.AddVelocity (VelocitySum * (1 - normalizedDistanceFromLeft));
		right.AddVelocity (VelocitySum * normalizedDistanceFromLeft);

		left.AddConstraint (ConstraintSum * (1 - normalizedDistanceFromLeft));
		right.AddConstraint (ConstraintSum * normalizedDistanceFromLeft);

		ForcesSum = Vector2.zero;
	}
}