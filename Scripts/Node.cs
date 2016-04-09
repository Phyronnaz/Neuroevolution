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
	const float NodeRadius = 0.75f;


	public Node (int id)
	{
		Id = id;
	}

	public Node (Vector2 position, NodeRenderer nodeRenderer, int id)
	{
		Position = position;
		savedPosition = position;
		Id = id;
		NodeRenderer = nodeRenderer;
	}

	public Node (Vector2 position, Transform parent, Color color, int id)
	{
		Position = position;
		savedPosition = position;
		Id = id;

		//Create node renderer
		var go = Object.Instantiate (Resources.Load ("Circle"), position, Quaternion.identity) as GameObject;
		go.name = "Node " + id;
		go.GetComponent<SpriteRenderer> ().color = color;
		NodeRenderer = go.AddComponent<NodeRenderer> ();
		NodeRenderer.Id = id;
		NodeRenderer.transform.parent = parent;
	}

	public virtual void Update (float deltaTime)
	{
		//velocity
		Velocity += VelocitySum;

		//constraints
		Velocity -= ConstraintSum.normalized * Vector2.Dot (Velocity, ConstraintSum.normalized);

		//weight
		Velocity += Vector2.down * Constants.GravityMultiplier * deltaTime;

		//collision
		if ((Velocity * deltaTime + Position).y < NodeRadius) {
			if (Position.y > NodeRadius + Constants.Tolerance)
				Velocity.y = (NodeRadius - Position.y) / deltaTime;
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


	public void Destroy ()
	{
		Object.Destroy (NodeRenderer.gameObject);
	}

	public virtual Node Copy ()
	{
		return new Node (Position, NodeRenderer, Id);
	}

	public virtual void Reset ()
	{
		Position = savedPosition;
		Velocity = Vector2.zero;
	}

	public static Node RandomNode (Vector2 position, Transform parent, Color color, int id)
	{
		return new Node (position, parent, new Color (1.0f - color.r, 1.0f - color.g, 1.0f - color.b), id);
	}
}