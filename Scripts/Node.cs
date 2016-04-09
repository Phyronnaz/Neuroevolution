using UnityEngine;

public class Node
{
	public readonly int Id;
	public NodeRenderer NodeRenderer;
	public Vector2 Velocity;
	public Vector2 Position;

	protected Vector2 VelocitySum;
	protected Vector2 ConstraintSum;

	readonly Vector2 savedPosition;
	readonly float NodeRadius = 0.75f;


	public Node (int id)
	{
		Id = id;
	}

	public Node (Vector2 position, Transform parent, Color color, int id)
	{
		Position = position;
		savedPosition = position;
		Id = id;

		//Create node renderer
		var go = Object.Instantiate (Resources.Load ("Circle"), position, Quaternion.identity) as GameObject;
		go.name = "Node " + id;
		go.GetComponent<SpriteRenderer> ().color = new Color (1.0f - color.r, 1.0f - color.g, 1.0f - color.b);
		NodeRenderer = go.AddComponent<NodeRenderer> ();
		NodeRenderer.Id = id;
		NodeRenderer.transform.parent = parent;
	}


	public static Node RandomNode (Vector2 position, Transform parent, Color color, int id)
	{
		return new Node (position, parent, color, id);
	}

	public virtual void Update (float deltaTime)
	{
		//velocity
		Velocity += VelocitySum;

		//weight
		Velocity.y -= Constants.GravityMultiplier * deltaTime;

		//constraints
		Velocity -= ConstraintSum.normalized * Vector2.Dot (Velocity, ConstraintSum.normalized);

		//collision
		if (Velocity.y * deltaTime + Position.y < NodeRadius) {
			if (Position.y > NodeRadius)
				Position.y = NodeRadius - Velocity.y * deltaTime;
			else
				Velocity.y -= (1 + Constants.Bounciness) * Velocity.y;
		}

		//friction
		if (Position.y < NodeRadius + Constants.Tolerance)
			Velocity.x /= (1 + Constants.Friction);

		//position
		Position += Velocity * deltaTime;
			
		//reset
		VelocitySum = Vector2.zero;
		ConstraintSum = Vector2.zero;
	}

	public void UpdateGraphics ()
	{
		NodeRenderer.SetPosition (Position);
	}

	public void AddVelocity (Vector2 velocity)
	{
		VelocitySum += velocity;
	}

	public void AddConstraint (Vector2 constraint)
	{
		ConstraintSum += constraint;
	}

	public virtual void Reset ()
	{
		Position = savedPosition;
		Velocity = Vector2.zero;
	}

	public void Destroy ()
	{
		Object.Destroy (NodeRenderer.gameObject);
	}

	public override bool Equals (object obj)
	{
		var n = (Node)obj;
		if (n != null) {
			return n.Id == Id;
		} else {
			return false;
		}
	}

	public override int GetHashCode ()
	{
		return Id.GetHashCode ();
	}
}