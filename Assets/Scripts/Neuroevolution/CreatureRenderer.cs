using UnityEngine;
using System.Collections.Generic;
using FVector2 = Microsoft.Xna.Framework.Vector2;
using FarseerPhysics.Dynamics;


namespace Assets.Scripts.Neuroevolution
{
    public class CreatureRenderer
    {
        private List<GameObject> nodes;
        private List<LineRenderer> lines;
        private Material nodesMaterial;
        private Material linesMaterial;
        private Material rotationNodeMaterial;
        private Material anchorNodeMaterial;
        private Material revolutesNodesMaterial;


        public CreatureRenderer()
        {
            nodesMaterial = new Material(Shader.Find("Diffuse"));
            nodesMaterial.color = Color.white;
            linesMaterial = new Material(Shader.Find("Diffuse"));
            linesMaterial.color = Color.black;
            rotationNodeMaterial = new Material(Shader.Find("Diffuse"));
            rotationNodeMaterial.color = Color.yellow;
            anchorNodeMaterial = new Material(Shader.Find("Diffuse"));
            anchorNodeMaterial.color = Color.blue;
            revolutesNodesMaterial = new Material(Shader.Find("Diffuse"));
            revolutesNodesMaterial.color = Color.green;

            nodes = new List<GameObject>();
            lines = new List<LineRenderer>();
        }


        private static Vector2 ToVector2(FVector2 fvector2)
        {
            return new Vector2(fvector2.X, fvector2.Y);
        }


        public void Update(CreatureStruct creature, int firstNode, int anchorNode, int secondNode)
        {
            Update(creature, null);
            if (firstNode != -1)
            {
                nodes[firstNode].GetComponent<Renderer>().material = revolutesNodesMaterial;
            }
            if (anchorNode != -1)
            {
                nodes[anchorNode].GetComponent<Renderer>().material = anchorNodeMaterial;
            }
            if (secondNode != -1)
            {
                nodes[secondNode].GetComponent<Renderer>().material = revolutesNodesMaterial;
            }
        }

        public void Update(CreatureStruct creature, List<Body> bodies)
        {
            AdjustSizes(creature.Positions.Count, creature.DistanceJoints.Count);
            UpdatePositions(creature, bodies);
            ResetMaterials();
            if (creature.RotationNode != -1)
            {
                nodes[creature.RotationNode].GetComponent<Renderer>().material = rotationNodeMaterial;
            }
        }

        private void AdjustSizes(int nodesSize, int linesSize)
        {
            while (nodes.Count > nodesSize)
            {
                Object.Destroy(nodes[0]);
                nodes.RemoveAt(0);
            }
            while (nodes.Count < nodesSize)
            {
                var go = Object.Instantiate(Resources.Load("Circle")) as GameObject;
                go.GetComponent<SpriteRenderer>().material = nodesMaterial;
                go.name = nodes.Count.ToString();
                nodes.Add(go);
            }

            while (lines.Count > linesSize)
            {
                Object.Destroy(lines[0]);
                lines.RemoveAt(0);
            }
            while (lines.Count < linesSize)
            {
                var l = (new GameObject()).AddComponent<LineRenderer>();
                l.material = linesMaterial;
                l.SetWidth(0.5f, 0.5f);
                lines.Add(l);
            }
        }

        private void UpdatePositions(CreatureStruct creature, List<Body> bodies)
        {
            if (bodies == null)
            {
                //Nodes
                for (var i = 0; i < creature.Positions.Count; i++)
                {
                    nodes[i].transform.position = ToVector2(creature.Positions[i]);
                }
                //Lines
                for (var i = 0; i < creature.DistanceJoints.Count; i++)
                {
                    lines[i].SetPosition(0, (Vector3)ToVector2(creature.Positions[creature.DistanceJoints[i].a]) + Vector3.forward);
                    lines[i].SetPosition(1, (Vector3)ToVector2(creature.Positions[creature.DistanceJoints[i].b]) + Vector3.forward);
                }
            }
            else
            {
                //Nodes
                for (var i = 0; i < creature.Positions.Count; i++)
                {
                    nodes[i].transform.position = ToVector2(bodies[i].Position);
                }
                //Lines
                for (var i = 0; i < creature.DistanceJoints.Count; i++)
                {
                    lines[i].SetPosition(0, (Vector3)ToVector2(bodies[creature.DistanceJoints[i].a].Position) + Vector3.forward);
                    lines[i].SetPosition(1, (Vector3)ToVector2(bodies[creature.DistanceJoints[i].b].Position) + Vector3.forward);
                }
            }
        }

        private void ResetMaterials()
        {
            foreach (var node in nodes)
            {
                node.GetComponent<Renderer>().material = nodesMaterial;
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
