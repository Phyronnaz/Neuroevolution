using System.Collections.Generic;
using FVector2 = Microsoft.Xna.Framework.Vector2;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Neuroevolution
{
    enum EditMode { Nodes, DistanceMuscles, RotationMuscles, RotationNode }
    public class Editor
    {
        List<FVector2> positions;
        List<DistanceJointStruct> distanceJoints;
        List<RevoluteJointStruct> revoluteJoints;
        List<Object> objects;
        EditMode editMode;
        //Nodes
        bool clamp;
        List<GameObject> grid;
        //Distance
        int currentMuscleNodeIndex = -1; //Index of the node which is the start of the muscle currently created
        LineRenderer currentLine;
        //Rotation
        int firstNodeIndex = -1;
        int anchorNodeIndex = -1;
        int secondNodeIndex = -1;
        GameObject firstNodeGameObject;
        GameObject secondNodeGameObject;
        GameObject anchorNodeGameObject;
        float upperLimit = 1;
        float lowerLimit = 1;
        AngleUI lowerLimitUI;
        AngleUI upperLimitUI;
        //Rotation node
        int rotationNodeIndex = -1;
        GameObject rotationNodeGameObject;


        public Editor()
        {
            positions = new List<FVector2>();
            objects = new List<Object>();
            distanceJoints = new List<DistanceJointStruct>();
            revoluteJoints = new List<RevoluteJointStruct>();
            editMode = EditMode.Nodes;
            AddLine();
            lowerLimitUI = new AngleUI(Vector2.zero, 100, 3, Color.blue, true);
            upperLimitUI = new AngleUI(Vector2.zero, 100, 3f, Color.red, false);
            lowerLimitUI.SetActive(false);
            upperLimitUI.SetActive(false);
            grid = new List<GameObject>();
            for (var x = -40; x < 40; x++)
            {
                for (var y = 1; y < 30; y++)
                {
                    var go = Object.Instantiate(Resources.Load("LittleCircle"), new Vector2(x, y), Quaternion.identity) as GameObject;
                    if (x % 10 == 0 ^ y % 10 == 0)
                    {
                        go.GetComponent<SpriteRenderer>().color = Color.cyan;
                    }
                    else if (x % 5 == 0 ^ y % 5 == 0)
                    {
                        go.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                    else if (x % 10 == 0 && y % 10 == 0)
                    {
                        go.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else
                    {
                        go.GetComponent<SpriteRenderer>().color = Color.grey;
                    }
                    go.SetActive(false);
                    grid.Add(go);
                    objects.Add(go);
                }
            }
            GameObject.Find("HidePanel").GetComponent<MeshRenderer>().enabled = false;
        }

        public void AddPrefabs()
        {
            rotationNodeIndex = 0;
            AddLine();
            editMode = EditMode.RotationMuscles;
            var a = new FVector2(-2, 15);
            var b = new FVector2(0, 5);
            var c = new FVector2(2, 10);



            distanceJoints.Add(new DistanceJointStruct(0, 1));
            distanceJoints.Add(new DistanceJointStruct(2, 1));

            currentLine.SetPosition(0, ToVector2(a));
            currentLine.SetPosition(1, ToVector2(b));
            AddLine();
            currentLine.SetPosition(0, ToVector2(c));
            currentLine.SetPosition(1, ToVector2(b));
            AddLine();

            firstNodeGameObject = Object.Instantiate(Resources.Load("Circle"), ToVector2(a), Quaternion.identity) as GameObject;
            firstNodeGameObject.GetComponent<SpriteRenderer>().color = Color.green;
            firstNodeGameObject.name = positions.Count.ToString();
            firstNodeIndex = positions.Count;
            positions.Add(a);
            objects.Add(firstNodeGameObject);

            anchorNodeGameObject = Object.Instantiate(Resources.Load("Circle"), ToVector2(b), Quaternion.identity) as GameObject;
            anchorNodeGameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            anchorNodeGameObject.name = positions.Count.ToString();
            anchorNodeIndex = positions.Count;
            positions.Add(b);
            objects.Add(anchorNodeGameObject);

            secondNodeGameObject = Object.Instantiate(Resources.Load("Circle"), ToVector2(c), Quaternion.identity) as GameObject;
            secondNodeGameObject.GetComponent<SpriteRenderer>().color = Color.green;
            secondNodeGameObject.name = positions.Count.ToString();
            secondNodeIndex = positions.Count;
            positions.Add(c);
            objects.Add(secondNodeGameObject);

        }
        public Vector2 ToVector2(FVector2 fvector2)
        {
            return new Vector2(fvector2.X, fvector2.Y);
        }

        void AddLine()
        {
            currentLine = (new GameObject()).AddComponent<LineRenderer>();
            currentLine.material = new Material(Shader.Find("Diffuse"));
            currentLine.material.color = Color.black;
            currentLine.SetWidth(0.5f, 0.5f);
            objects.Add(currentLine);
        }

        void EditNodes()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                clamp = !clamp;
                foreach (var g in grid)
                {
                    g.SetActive(clamp);
                }
                GameObject.Find("HidePanel").GetComponent<MeshRenderer>().enabled = clamp;
            }
            if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                if (hit.collider == null)
                {
                    var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    p.z = 0;
                    if (clamp)
                    {
                        p.x = Mathf.Round(p.x);
                        p.y = Mathf.Round(p.y);
                    }
                    var go = Object.Instantiate(Resources.Load("Circle"), p, Quaternion.identity) as GameObject;
                    go.name = positions.Count.ToString();
                    go.GetComponent<SpriteRenderer>().color = Color.white;
                    objects.Add(go);
                    positions.Add(new FVector2(p.x, p.y));
                }
            }
        }

        void EditDistanceMuscles()
        {
            //Render muscle
            if (currentMuscleNodeIndex != -1)
            {
                currentLine.enabled = true;
                var p = ToVector2(positions[currentMuscleNodeIndex]);
                var q = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                q.z = 0;
                currentLine.SetPosition(0, p);
                currentLine.SetPosition(1, q);
            }
            //Cancel edit
            if (Input.GetMouseButtonDown(1))
            {
                currentMuscleNodeIndex = -1;
            }
            //Create muscle
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                int nodeIndex;
                if (hit.collider != null && int.TryParse(hit.collider.name, out nodeIndex))
                {
                    if (currentMuscleNodeIndex == -1)
                    {
                        currentMuscleNodeIndex = nodeIndex;
                    }
                    else if (currentMuscleNodeIndex != nodeIndex)
                    {
                        distanceJoints.Add(new DistanceJointStruct(currentMuscleNodeIndex, nodeIndex));
                        currentLine.SetPosition(1, new Vector2(hit.transform.position.x, hit.transform.position.y));
                        AddLine();
                        currentMuscleNodeIndex = -1;
                    }
                }
            }
        }

        void EditRotationMuscles()
        {
            //Render
            if (secondNodeIndex != -1)
            {
                var p = new Vector2(positions[anchorNodeIndex].X, positions[anchorNodeIndex].Y);
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
                firstNodeGameObject.GetComponent<SpriteRenderer>().color = Color.white;
                anchorNodeGameObject.GetComponent<SpriteRenderer>().color = Color.white;
                secondNodeGameObject.GetComponent<SpriteRenderer>().color = Color.white;
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
                        firstNodeGameObject = hit.transform.gameObject;
                        firstNodeGameObject.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else if (anchorNodeIndex == -1 && nodeIndex != firstNodeIndex)
                    {
                        anchorNodeIndex = nodeIndex;
                        anchorNodeGameObject = hit.transform.gameObject;
                        anchorNodeGameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                    else if (nodeIndex != firstNodeIndex && nodeIndex != anchorNodeIndex)
                    {
                        secondNodeIndex = nodeIndex;
                        secondNodeGameObject = hit.transform.gameObject;
                        secondNodeGameObject.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                }
            }
            if (secondNodeIndex != -1)
            {
                upperLimit += 0.01f * Input.GetAxis("Vertical");
                lowerLimit += 0.01f * Input.GetAxis("Horizontal");

                lowerLimit = Mathf.Clamp(lowerLimit, 0, Mathf.PI);
                upperLimit = Mathf.Clamp(upperLimit, 0, Mathf.PI);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                var a = ToVector2(positions[firstNodeIndex]);
                var b = ToVector2(positions[secondNodeIndex]);
                var anchor = ToVector2(positions[anchorNodeIndex]);
                var angle = Vector2.Angle(a - anchor, b - anchor) * Mathf.Deg2Rad;
                revoluteJoints.Add(new RevoluteJointStruct(firstNodeIndex, secondNodeIndex, anchorNodeIndex, lowerLimit, upperLimit));

                firstNodeGameObject.GetComponent<SpriteRenderer>().color = Color.white;
                anchorNodeGameObject.GetComponent<SpriteRenderer>().color = Color.white;
                secondNodeGameObject.GetComponent<SpriteRenderer>().color = Color.white;
                firstNodeIndex = -1;
                secondNodeIndex = -1;
                anchorNodeIndex = -1;
            }
        }

        void EditRotationNode()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                int nodeIndex;
                if (hit.collider != null && int.TryParse(hit.collider.name, out nodeIndex))
                {
                    rotationNodeIndex = nodeIndex;
                    if (rotationNodeGameObject != null)
                    {
                        rotationNodeGameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    rotationNodeGameObject = hit.transform.gameObject;
                    rotationNodeGameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }
        }

        public void Update()
        {
            lowerLimitUI.SetActive(false);
            upperLimitUI.SetActive(false);
            currentLine.enabled = false;
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
                case EditMode.RotationNode:
                    EditRotationNode();
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
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                editMode = EditMode.RotationNode;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                AddPrefabs();
            }
        }

        public List<FVector2> GetPositions()
        {
            return positions;
        }

        public List<DistanceJointStruct> GetDistanceJoints()
        {
            return distanceJoints;
        }

        public List<RevoluteJointStruct> GetRevoluteJoints()
        {
            return revoluteJoints;
        }

        public int GetRotationNode()
        {
            return rotationNodeIndex;
        }
        public void Destroy()
        {
            foreach (var o in objects)
            {
                Object.Destroy(o);
            }
            lowerLimitUI.Destroy();
            upperLimitUI.Destroy();
            GameObject.Find("HidePanel").GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
