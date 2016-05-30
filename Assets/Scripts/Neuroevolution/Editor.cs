using System.Collections.Generic;
using FVector2 = Microsoft.Xna.Framework.Vector2;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Neuroevolution
{
    enum EditMode { Nodes, DistanceMuscles, RotationMuscles, RotationNode }
    public class Editor
    {
        public CreatureStruct Creature;
        private CreatureRenderer creatureRenderer;
        private EditMode editMode;
        //Nodes
        private bool clamp;
        private List<GameObject> grid;
        //Distance
        private int currentMuscleNodeIndex = -1; //Index of the node which is the start of the muscle currently created
        private LineRenderer line;
        //Rotation
        private int firstNode = -1;
        private int anchorNode = -1;
        private int secondNode = -1;
        private float upperLimit = 1;
        private float lowerLimit = 1;
        private AngleUI lowerLimitUI;
        private AngleUI upperLimitUI;


        public Editor() : this(new List<FVector2>(), new List<DistanceJointStruct>(), new List<RevoluteJointStruct>(), -1, new List<Matrix>()) { }

        public Editor(List<FVector2> positions, List<DistanceJointStruct> distanceJoints, List<RevoluteJointStruct> revoluteJoints, int rotationNode, List<Matrix> synapses)
        {
            creatureRenderer = new CreatureRenderer();
            Creature = new CreatureStruct(new List<FVector2>(), new List<DistanceJointStruct>(), new List<RevoluteJointStruct>(), -1);
            editMode = EditMode.Nodes;
            CreateGrid();
            SetGrid(false);
            CreateLimitUI();
            SetLimitUI(false);
            line = (new GameObject()).AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Diffuse")); ;
            line.material.color = Color.black;
            line.SetWidth(0.5f, 0.5f);
        }


        private static Vector2 ToVector2(FVector2 fvector2)
        {
            return new Vector2(fvector2.X, fvector2.Y);
        }


        public void Update()
        {
            creatureRenderer.Update(Creature, firstNode, anchorNode, secondNode);
            SetLimitUI(false);
            SetGrid(false);
            line.enabled = false;
            if (!GameObject.Find("EventSystem").GetComponent<EventSystem>().IsPointerOverGameObject())
            {
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
            }
        }


        private void CreateGrid()
        {
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
                    grid.Add(go);
                }
            }
        }
        private void SetGrid(bool active)
        {
            foreach (var g in grid)
            {
                g.SetActive(active);
            }
            GameObject.Find("HidePanel").GetComponent<MeshRenderer>().enabled = active;
        }

        private void CreateLimitUI()
        {
            lowerLimitUI = new AngleUI(Vector2.zero, 100, 3, Color.blue, true);
            upperLimitUI = new AngleUI(Vector2.zero, 100, 3, Color.red, false);
        }
        private void SetLimitUI(bool active)
        {
            lowerLimitUI.SetActive(false);
            upperLimitUI.SetActive(false);
        }


        private void EditNodes()
        {
            //Grid
            if (Input.GetKeyDown(KeyCode.C))
            {
                clamp = !clamp;
            }
            SetGrid(clamp);

            //Add node
            if (Input.GetMouseButtonDown(0))
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
                    Creature.Positions.Add(new FVector2(p.x, p.y));
                }
            }

            //Remove node
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                if (hit.collider != null)
                {
                    int i;
                    if (int.TryParse(hit.transform.name, out i))
                    {
                        creatureRenderer.Destroy();
                        creatureRenderer = new CreatureRenderer();
                        Creature.Positions.RemoveAt(i);
                        //Remove revolute && distance joints 
                        var x = Creature.DistanceJoints.Count;
                        var j = 0;
                        while (j < x)
                        {
                            if (Creature.DistanceJoints[j].a == i || Creature.DistanceJoints[j].b == i)
                            {
                                Creature.DistanceJoints.RemoveAt(j);
                                j--;
                            }
                            x = Creature.DistanceJoints.Count;
                            j++;
                        }
                        x = Creature.RevoluteJoints.Count;
                        j = 0;
                        while (j < x)
                        {
                            if (Creature.RevoluteJoints[j].a == i || Creature.RevoluteJoints[j].anchor == i || Creature.RevoluteJoints[j].b == i)
                            {
                                Creature.RevoluteJoints.RemoveAt(j);
                                j--;
                            }
                            x = Creature.RevoluteJoints.Count;
                            j++;
                        }
                    }
                }
            }
        }

        private void EditDistanceMuscles()
        {
            //Render muscle
            if (currentMuscleNodeIndex != -1)
            {
                line.enabled = true;
                var p = ToVector2(Creature.Positions[currentMuscleNodeIndex]);
                var q = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                q.z = 0;
                line.SetPosition(0, (Vector3)p + Vector3.forward);
                line.SetPosition(1, q + Vector3.forward);
            }

            //Cancel edit
            if (Input.GetMouseButtonDown(1))
            {
                if (currentMuscleNodeIndex == -1)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                    int nodeIndex;
                    if (hit.collider != null && int.TryParse(hit.transform.name, out nodeIndex))
                    {
                        var x = Creature.DistanceJoints.Count;
                        var i = 0;
                        while (i < x)
                        {
                            if (Creature.DistanceJoints[i].a == nodeIndex || Creature.DistanceJoints[i].b == nodeIndex)
                            {
                                Creature.DistanceJoints.RemoveAt(i);
                                i--;
                            }
                            x = Creature.DistanceJoints.Count;
                            i++;
                        }
                    }

                }
                else
                {
                    currentMuscleNodeIndex = -1;
                }
            }

            //Create muscle
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                int nodeIndex;
                if (hit.collider != null && int.TryParse(hit.transform.name, out nodeIndex))
                {
                    if (currentMuscleNodeIndex == -1)
                    {
                        currentMuscleNodeIndex = nodeIndex;
                    }
                    else if (currentMuscleNodeIndex != nodeIndex)
                    {
                        Creature.DistanceJoints.Add(new DistanceJointStruct(currentMuscleNodeIndex, nodeIndex));
                        currentMuscleNodeIndex = -1;
                    }
                }
            }
        }

        private void EditRotationMuscles()
        {
            //Render
            if (secondNode != -1)
            {
                var p = ToVector2(Creature.Positions[anchorNode]);
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
                firstNode = -1;
                secondNode = -1;
                anchorNode = -1;
            }
            //Create muscle
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                int nodeIndex;
                if (hit.collider != null && int.TryParse(hit.collider.name, out nodeIndex))
                {
                    if (firstNode == -1)
                    {
                        firstNode = nodeIndex;
                    }
                    else if (anchorNode == -1 && nodeIndex != firstNode)
                    {
                        anchorNode = nodeIndex;
                    }
                    else if (nodeIndex != firstNode && nodeIndex != anchorNode)
                    {
                        secondNode = nodeIndex;
                    }
                }
            }
            if (secondNode != -1)
            {
                upperLimit += 0.01f * Input.GetAxis("Vertical");
                lowerLimit += 0.01f * Input.GetAxis("Vertical");

                lowerLimit = Mathf.Clamp(lowerLimit, 0, Mathf.PI);
                upperLimit = Mathf.Clamp(upperLimit, 0, Mathf.PI);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Creature.RevoluteJoints.Add(new RevoluteJointStruct(firstNode, secondNode, anchorNode, lowerLimit, upperLimit));

                firstNode = -1;
                secondNode = -1;
                anchorNode = -1;
            }
        }

        private void EditRotationNode()
        {
            //Add
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                int nodeIndex;
                if (hit.collider != null && int.TryParse(hit.collider.name, out nodeIndex))
                {
                    Creature.RotationNode = nodeIndex;
                }
            }
            //Remove
            if (Input.GetMouseButtonDown(1))
            {
                Creature.RotationNode = -1;
            }
        }

        public void Destroy()
        {
            grid.ForEach(Object.Destroy);
            creatureRenderer.Destroy();
            Object.Destroy(line.gameObject);
            lowerLimitUI.Destroy();
            upperLimitUI.Destroy();
            GameObject.Find("HidePanel").GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
