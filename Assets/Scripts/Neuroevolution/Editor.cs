using System.Collections.Generic;
using FVector2 = Microsoft.Xna.Framework.Vector2;
using UnityEngine;
using UnityEngine.EventSystems;

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
        int firstNodeIndex = -1;
        int anchorNodeIndex = -1;
        int secondNodeIndex = -1;
        float upperLimit = 1;
        float lowerLimit = 1;
        AngleUI lowerLimitUI;
        AngleUI upperLimitUI;
        EditMode editMode;
        Color color = Color.black;

        public Editor()
        {
            lines = new List<LineRenderer>();
            nodesPositions = new List<FVector2>();
            nodesGameObjects = new List<GameObject>();
            distanceJoints = new List<DistanceJointStruct>();
            revoluteJoints = new List<RevoluteJointStruct>();
            editMode = EditMode.Nodes;
            AddLine();
            lowerLimitUI = new AngleUI(Vector2.zero, 100, 2, Color.blue);
            upperLimitUI = new AngleUI(Vector2.zero, 100, 3, Color.red);
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
            if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
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

        void EditRotationMuscles()
        {
            //Render
            if (secondNodeIndex != -1)
            {
                var p = new Vector2(nodesPositions[anchorNodeIndex].X, nodesPositions[anchorNodeIndex].Y);
                lowerLimitUI.SetPosition(p);
                lowerLimitUI.SetActive(true);
                upperLimitUI.SetPosition(p);
                upperLimitUI.SetActive(true);
                lowerLimitUI.SetAngle(lowerLimit);
                upperLimitUI.SetAngle(upperLimit);
            }
            //Cancel edit
            if (Input.GetMouseButtonDown(1))
            {
                if (firstNodeIndex != -1)
                {
                    nodesGameObjects[firstNodeIndex].GetComponent<SpriteRenderer>().color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
                }
                if (anchorNodeIndex != -1)
                {
                    nodesGameObjects[anchorNodeIndex].GetComponent<SpriteRenderer>().color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
                }
                if (secondNodeIndex != -1)
                {
                    nodesGameObjects[secondNodeIndex].GetComponent<SpriteRenderer>().color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
                }
                firstNodeIndex = -1;
                secondNodeIndex = -1;
                anchorNodeIndex = -1;
            }
            //Create muscle
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                int nodeIndex;
                if (hit.collider != null && int.TryParse(hit.collider.name, out nodeIndex))
                {
                    if (firstNodeIndex == -1)
                    {
                        firstNodeIndex = nodeIndex;
                        nodesGameObjects[nodeIndex].GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else if (anchorNodeIndex == -1)
                    {
                        anchorNodeIndex = nodeIndex;
                        nodesGameObjects[nodeIndex].GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                    else
                    {
                        secondNodeIndex = nodeIndex;
                        nodesGameObjects[nodeIndex].GetComponent<SpriteRenderer>().color = Color.green;
                    }
                }
            }
            if (secondNodeIndex != -1)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    upperLimit += 0.1f;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    upperLimit -= 0.1f;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    lowerLimit += 0.1f;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    lowerLimit -= 0.1f;
                }
                var a = new Vector2(nodesPositions[firstNodeIndex].X, nodesPositions[firstNodeIndex].Y);
                var b = new Vector2(nodesPositions[secondNodeIndex].X, nodesPositions[secondNodeIndex].Y);
                var anchor = new Vector2(nodesPositions[anchorNodeIndex].X, nodesPositions[anchorNodeIndex].Y);
                var degAngle = Vector2.Angle(a - anchor, b - anchor);
                lowerLimit = Mathf.Clamp(lowerLimit, Mathf.Deg2Rad * degAngle, Mathf.PI * 2);
                upperLimit = Mathf.Clamp(upperLimit, lowerLimit, Mathf.PI * 2);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //TODO: replace speed
                revoluteJoints.Add(new RevoluteJointStruct(firstNodeIndex, secondNodeIndex, anchorNodeIndex, lowerLimit, upperLimit, 200));
                if (firstNodeIndex != -1)
                {
                    nodesGameObjects[firstNodeIndex].GetComponent<SpriteRenderer>().color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
                }
                if (anchorNodeIndex != -1)
                {
                    nodesGameObjects[anchorNodeIndex].GetComponent<SpriteRenderer>().color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
                }
                if (secondNodeIndex != -1)
                {
                    nodesGameObjects[secondNodeIndex].GetComponent<SpriteRenderer>().color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
                }
                firstNodeIndex = -1;
                secondNodeIndex = -1;
                anchorNodeIndex = -1;
            }
        }

        public void Update()
        {
            lowerLimitUI.SetActive(false);
            upperLimitUI.SetActive(false);
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
                    EditRotationMuscles();
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
                Object.Destroy(n);
            }
            foreach (var l in lines)
            {
                Object.Destroy(l.gameObject);
            }
            lowerLimitUI.Destroy();
            upperLimitUI.Destroy();
        }
    }
}
