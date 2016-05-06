using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    enum EditMode { Nodes, DistanceMuscles, RotationMuscles }
    public class Editor
    {
        List<FVector2> nodesPositions;
        List<DistanceJointStruct> distanceJoints;
        List<RevoluteJointStruct> revoluteJoints;
        List<GameObject> nodesGameObjects;
        List<LineRenderer> lines;
        int currentNodeIndex = -1; //Index of the node which is the start of the muscle currently created
        EditMode editMode;
        Color color = Color.blue;

        public Editor()
        {
            lines = new List<LineRenderer>();
            nodesPositions = new List<FVector2>();
            nodesGameObjects = new List<GameObject>();
            distanceJoints = new List<DistanceJointStruct>();
            revoluteJoints = new List<RevoluteJointStruct>();
            editMode = EditMode.Nodes;
            AddLine();
        }

        void AddLine()
        {
            lines.Add((new GameObject()).AddComponent<LineRenderer>());
            lines[lines.Count - 1].material = new Material(Shader.Find("Diffuse"));
            lines[lines.Count - 1].material.color = color;
            lines[lines.Count - 1].name = "Line " + lines.Count;
        }


        void EditNodes()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                if (hit.collider == null)
                {
                    var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    p.z = 0;
                    var go = Object.Instantiate(Resources.Load("Circle"), p, Quaternion.identity) as GameObject;
                    go.name = nodesPositions.Count.ToString();
                    go.GetComponent<SpriteRenderer>().color = new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b);
                    nodesGameObjects.Add(go);
                    nodesPositions.Add(new FVector2(p.x, p.y));
                }
            }
        }

        void EditDistanceMuscles()
        {
            //Render muscle
            if (currentNodeIndex != -1)
            {
                lines[lines.Count - 1].enabled = true;
                var p = nodesPositions[currentNodeIndex];
                var q = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                q.z = 0;
                lines[lines.Count - 1].SetPosition(0, new Vector2(p.X, p.Y));
                lines[lines.Count - 1].SetPosition(1, q);
            }
            //Cancel edit
            if (Input.GetMouseButtonDown(1))
            {
                currentNodeIndex = -1;
            }
            //Create muscle
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                int nodeIndex;
                if (hit.collider != null && int.TryParse(hit.collider.name, out nodeIndex))
                {
                    if (currentNodeIndex == -1)
                    {
                        currentNodeIndex = nodeIndex;
                    }
                    else
                    {
                        distanceJoints.Add(new DistanceJointStruct(currentNodeIndex, nodeIndex));
                        lines[lines.Count - 1].SetPosition(1, new Vector2(hit.transform.position.x, hit.transform.position.y));
                        AddLine();
                        currentNodeIndex = -1;
                    }
                }
            }
        }

        public void Update()
        {
            lines[lines.Count - 1].enabled = false;
            switch (editMode)
            {
                case EditMode.Nodes:
                    EditNodes();
                    break;
                case EditMode.DistanceMuscles:
                    EditDistanceMuscles();
                    break;
                case EditMode.RotationMuscles:
                    break;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                editMode = EditMode.Nodes;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                editMode = EditMode.DistanceMuscles;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                editMode = EditMode.RotationMuscles;
            }
        }

        public List<FVector2> GetPositions()
        {
            return nodesPositions;
        }

        public List<DistanceJointStruct> GetDistanceJoints()
        {
            return distanceJoints;
        }

        public List<RevoluteJointStruct> GetRevoluteJoints()
        {
            return revoluteJoints;
        }

        public void Destroy()
        {
            foreach (var n in nodesGameObjects)
            {
                Object.DestroyImmediate(n);
            }
            foreach (var l in lines)
            {
                Object.DestroyImmediate(l);
            }
        }
    }
}
