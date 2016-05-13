using UnityEngine;
using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using Joint = FarseerPhysics.Dynamics.Joints.Joint;
using FarseerPhysics.Dynamics;


namespace Assets.Scripts.Neuroevolution
{
    public class CreatureRenderer
    {
        List<Transform> nodes;
        List<LineRenderer> lines;
        Color color;

        public void Update(List<Body> bodies, List<Joint> joints, Color color)
        {
            //Select distances joints
            var distanceJoints = new List<DistanceJoint>();
            foreach (var j in joints)
            {
                if (j is DistanceJoint)
                {
                    distanceJoints.Add((DistanceJoint)j);
                }
            }

            //Update lines
            if (lines == null || lines.Count != distanceJoints.Count)
            {
                if (lines != null)
                {
                    lines.ForEach(Object.Destroy);
                }
                lines = new List<LineRenderer>();
                var material = new Material(Shader.Find("Diffuse"));
                for (var k = 0; k < distanceJoints.Count; k++)
                {
                    var l = (new GameObject()).AddComponent<LineRenderer>();
                    l.material = material;
                    l.SetColors(color, color);
                    l.SetWidth(0.5f, 0.5f);
                    lines.Add(l);
                }
            }
            for (var i = 0; i < lines.Count; i++)
            {
                lines[i].SetPosition(0, new Vector3(distanceJoints[i].BodyA.Position.X, distanceJoints[i].BodyA.Position.Y, 0));
                lines[i].SetPosition(1, new Vector3(distanceJoints[i].BodyB.Position.X, distanceJoints[i].BodyB.Position.Y, 0));
            }

            //Update nodes
            if (nodes == null || nodes.Count != bodies.Count)
            {
                if (nodes != null)
                {
                    nodes.ForEach(Object.Destroy);
                }
                nodes = new List<Transform>();
                foreach (var b in bodies)
                {
                    var go = Object.Instantiate(Resources.Load("Circle"), new Vector2(b.Position.X, b.Position.Y), Quaternion.identity) as GameObject;
                    go.GetComponent<SpriteRenderer>().color = new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b);
                    nodes.Add(go.transform);
                }
            }
            else
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    nodes[i].position = new Vector2(bodies[i].Position.X, bodies[i].Position.Y);
                }
            }
            if (color != this.color)
            {
                this.color = color;
                foreach (var l in lines)
                {
                    l.material.color = color;
                }
                foreach (var n in nodes)
                {
                    n.GetComponent<SpriteRenderer>().color = new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b);
                }
            }
        }

        public void Destroy()
        {
            foreach (var l in lines)
            {
                Object.Destroy(l.gameObject);
            }
            foreach (var n in nodes)
            {
                Object.Destroy(n.gameObject);
            }
        }
    }
}
