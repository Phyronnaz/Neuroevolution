using UnityEngine;
using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;


namespace Assets.Scripts.Neuroevolution
{
    class CreatureRenderer : MonoBehaviour
    {
        List<Transform> nodes;
        List<LineRenderer> lines;
        Color color = Random.ColorHSV();

        void Update(List<Body> bodies, List<FarseerJoint> joints)
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
                    foreach (var l in lines)
                    {
                        Destroy(l);
                    }
                }
                lines = new List<LineRenderer>();
                foreach (var j in distanceJoints)
                {
                    var l = new LineRenderer();
                    var material = new Material(Shader.Find("Diffuse"));
                    l.material = material;
                    l.SetColors(color, color);
                    lines.Add(l);
                }
            }
            for(var i = 0; i < lines.Count; i ++)
            {
                lines[i].SetPosition(0, new Vector3(distanceJoints[i].BodyA.Position.X, distanceJoints[i].BodyA.Position.Y, 0));
                lines[i].SetPosition(1, new Vector3(distanceJoints[i].BodyB.Position.X, distanceJoints[i].BodyB.Position.Y, 0));
            }

            //Update nodes
            if (nodes == null || nodes.Count != bodies.Count)
            {
                nodes = new List<Transform>();
                foreach (var b in bodies)
                {
                    var go = Instantiate(Resources.Load("Circle"), new Vector2(b.Position.X, b.Position.Y), Quaternion.identity) as GameObject;
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
        }
    }
}
