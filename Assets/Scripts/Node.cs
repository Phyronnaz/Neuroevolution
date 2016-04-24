using UnityEngine;
using Microsoft.Xna.Framework;

namespace Evolution
{
    public class Node
    {
        public readonly int Id;
        public FSBodyComponent Body;

        readonly Vector2 savedPosition;

        public Node(Vector2 position, Transform parent, Color color, int id)
        {
            savedPosition = position;
            Id = id;

            //Create node renderer
            var go = Object.Instantiate(Resources.Load("Circle"), position, Quaternion.identity) as GameObject;
            go.name = "Node " + id;
            go.GetComponent<SpriteRenderer>().color = new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b);
            go.transform.parent = parent;
            go.transform.position = position;
            go.AddComponent<FSShapeComponent>();
            go.GetComponent<FSShapeComponent>().SType = FarseerPhysics.Collision.Shapes.ShapeType.Circle;
            Body = go.AddComponent<FSBodyComponent>();
        }

        public virtual void Update(float deltaTime)
        {
            Body.Update();
        }

        public void UpdateGraphics()
        {
            ;
        }

        public virtual void Reset()
        {
            Body.transform.position = savedPosition;
            Body.PhysicsBody.LinearVelocity = FVector2.Zero;
        }

        public void Destroy()
        {
            Object.Destroy(NodeRenderer.gameObject);
        }

        public override bool Equals(object obj)
        {
            var n = (Node)obj;
            if (n != null)
            {
                return n.Id == Id;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}