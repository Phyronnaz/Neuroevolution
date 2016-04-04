using UnityEngine;


//TODO: Reset && Disable
public class Node
{
	public readonly float Friction;
	public readonly float Mass;
	public readonly float CoefficientOfRestitution;
	public readonly int Id;
	public const float NodeRadius = 0.75f;

	public Vector2 Velocity;
	public Vector2 Position;

	protected NodeRenderer NodeRenderer;
	protected Vector2 ForcesSum = Vector2.zero;
	protected Vector2 VelocitySum = Vector2.zero;
	protected Vector2 ConstraintSum = Vector2.zero;


	public Node (int id)
	{
		Id = id;
	}

	public Node (float friction, Vector2 position, float mass, float coefficientOfRestitution, Transform parent, Color color, int id)
	{
		Friction = friction;
		Position = position;
		Mass = mass;
		CoefficientOfRestitution = coefficientOfRestitution;
		Id = id;

		//Create node renderer
		var go = Object.Instantiate (Resources.Load ("Circle"), position, Quaternion.identity) as GameObject;
		go.name = "Node " + id;
		go.GetComponent<SpriteRenderer> ().color = color; //new Color (friction / Constants.frictionAmplitude, 0, 0);
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
				Velocity.y -= (1 + CoefficientOfRestitution) * Velocity.y;
		}

		//friction
		if (Position.y < NodeRadius + Constants.Tolerance)
			Velocity.x /= (1 + Friction);

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

	public static Node RandomNode (Vector2 position, Transform parent, Color color, int id)
	{
		//var friction = Random.Range (Constants.minRandom, Constants.frictionAmplitude);
		var mass = Random.Range (Constants.MinMass, Constants.MaxMass);

		return new Node (10000, position, mass, Constants.Bounciness, parent, new Color (1.0f - color.r, 1.0f - color.g, 1.0f - color.b), id);
	}
}