using UnityEngine;
using System.Collections.Generic;

namespace Evolution
{
    public class Creature : System.IComparable<Creature>
    {
        readonly List<Muscle> muscles;
        readonly List<Node> nodes;
        readonly Transform transform;
        readonly Matrix synapse_0;
        readonly Matrix synapse_1;
        float cycleDuration;
        float time;
        int hiddenSize = 32;


        public Creature(List<Muscle> muscles, List<Node> nodes, float cycleDuration, Transform transform)
        {
            new Creature(muscles, nodes, cycleDuration, transform, Matrix.Random(2 * muscles.Count, hiddenSize), Matrix.Random(hiddenSize, muscles.Count));
        }
        public Creature(List<Muscle> muscles, List<Node> nodes, float cycleDuration, Transform transform, Matrix synapse_0, Matrix synapse_1)
        {
            this.muscles = muscles;
            this.nodes = nodes;
            this.cycleDuration = cycleDuration;
            this.transform = transform;
            foreach (var n in nodes)
            {
                n.NodeRenderer.GetComponent<Collider2D>().enabled = false;
            }
        }


        public static Creature RandomCreature(float cycleDuration)
        {
            // Set number of muscles and nodes
            int numberOfNodes;
            int numberOfMuscles;
            if (Constants.RandomNumbers)
            {
                numberOfNodes = Random.Range(3, 6);
                numberOfMuscles = Random.Range(numberOfNodes, numberOfNodes * 4);
            }
            else
            {
                numberOfNodes = Constants.NumberOfNodes;
                numberOfMuscles = Constants.NumberOfMuscles;
            }
            return RandomCreature(cycleDuration, numberOfNodes, numberOfMuscles);
        }

        public static Creature RandomCreature(float cycleDuration, int numberOfNodes, int numberOfMuscles)
        {
            Color color = Random.ColorHSV();
            // Create creature
            var parent = new GameObject().transform;
            parent.name = "Creature " + Random.Range(0, 10000);

            // Define arrays
            var nodes = new List<Node>(numberOfNodes);
            var muscles = new List<Muscle>(numberOfMuscles);

            // Generate nodes
            for (var i = 0; i < numberOfNodes; i++)
            {
                nodes.Add(Node.RandomNode(new Vector2(Random.Range(-10f, 10f), Random.Range(10f, 20f)), parent, color, i));
            }

            // Recenter nodes
            float s = 0;
            foreach (var n in nodes)
            {
                s += n.Position.x;
            }
            s /= nodes.Count;
            foreach (var n in nodes)
            {
                var p = n.Position;
                p.x -= s;
                n.Position = p;
            }

            // Generate muscles
            var k = 0;
            while (k < (nodes.Count - 1) * nodes.Count / 2 && k < numberOfMuscles)
            {
                //Random connection
                var t = new Tuple(Random.Range(0, nodes.Count), Random.Range(0, nodes.Count));

                bool alreadyAdded = false;
                if (t.a == t.b)
                {
                    alreadyAdded = true;
                }
                else
                {
                    foreach (var muscle in muscles)
                    {
                        if (muscle.Equals(t))
                        {
                            alreadyAdded = true;
                            break;
                        }
                    }
                }

                if (!alreadyAdded)
                {
                    muscles.Add(Muscle.RandomMuscle(nodes[t.a], nodes[t.b], cycleDuration, false, color, parent));
                    k++;
                }
            }

            // Update graphics
            foreach (var m in muscles)
            {
                m.UpdateGraphics();
            }
            foreach (var n in nodes)
            {
                n.UpdateGraphics();
            }

            return new Creature(muscles, nodes, cycleDuration, parent);
        }

        public static Creature CloneCreature(Creature creature, float variation, Color color)
        {
            var numberOfNodes = creature.nodes.Count;
            var numberOfMuscles = creature.muscles.Count;
            //		Color color = Random.ColorHSV ();
            // Create creature
            var parent = new GameObject().transform;
            parent.name = "Creature " + Random.Range(0, 10000);

            // Define arrays
            var nodes = new List<Node>(numberOfNodes);
            var muscles = new List<Muscle>(numberOfMuscles);

            // Generate nodes
            for (var i = 0; i < numberOfNodes; i++)
            {
                nodes.Add(new Node(creature.nodes[i].Position, parent, color, i));
            }

            // Generate muscles
            for (var j = 0; j < numberOfMuscles; j++)
            {
                var m = creature.muscles[j];
                muscles.Add(Muscle.CloneMuscle(m, nodes[m.Left.Id], nodes[m.Right.Id], variation, color, parent));
            }

            // Update graphics
            foreach (var m in muscles)
            {
                m.UpdateGraphics();
            }
            foreach (var n in nodes)
            {
                n.UpdateGraphics();
            }

            var newSyn0 = creature.synapse_0 + Matrix.Random(creature.synapse_0.M, creature.synapse_0.N, Constants.Variation, new System.Random());
            var newSyn1 = creature.synapse_0 + Matrix.Random(creature.synapse_1.M, creature.synapse_1.N, Constants.Variation, new System.Random());
            return new Creature(muscles, nodes, creature.cycleDuration, parent, newSyn0, newSyn1);
        }

        public int CompareTo(Creature other)
        {
            return GetFitness().CompareTo(other.GetFitness());
        }

        public void Update(float deltaTime)
        {
            // Time modulo cycle duration
            time = (time - cycleDuration * (Mathf.FloorToInt(time / cycleDuration)));

            // Update muscles and nodes
            if (Constants.NeuralNetwork)
            {
                Train();
            }
            else
            {
                foreach (var m in muscles)
                {
                    if ((time > m.ChangeTime && !m.BeginWithContraction) || (time < m.ChangeTime && m.BeginWithContraction))
                        m.Contract = true;
                    else
                        m.Contract = false;
                    m.Update();
                }
            }
            foreach (var n in nodes)
            {
                n.Update(deltaTime);
            }

            // Update current time
            time += deltaTime;
        }

        public void UpdateGraphics()
        {
            foreach (var m in muscles)
            {
                m.UpdateGraphics();
            }
            foreach (var n in nodes)
            {
                n.UpdateGraphics();
            }
        }

        public int GetCyclePercentage()
        {
            return Mathf.CeilToInt(time / cycleDuration * 100);
        }

        public float GetAveragePosition()
        {
            var averagePosition = 0f;
            foreach (var n in nodes)
            {
                averagePosition += n.Position.x;
            }
            return averagePosition / nodes.Count;
        }

        public float GetFitness()
        {
            return GetAveragePosition();
        }

        public void Reset()
        {
            foreach (var n in nodes)
            {
                n.Reset();
            }
        }

        public void Destroy()
        {
            foreach (var m in muscles)
            {
                m.Destroy();
            }
            foreach (var n in nodes)
            {
                n.Destroy();
            }
            Object.Destroy(transform.gameObject);
        }

        void Train()
        {
            var input = new Matrix(1, 2 * muscles.Count);

            for (var i = 0; i < muscles.Count; i++)
            {
                input[0][2 * i] = muscles[i].Ratio;
                input[0][2 * i + 1] = (muscles[i].Contract) ? 1 : -1;
            }

            var hd1 = Sigma(Matrix.Dot(input, synapse_0));
            var output = Matrix.Dot(hd1, synapse_1);

            for (var i = 0; i < muscles.Count; i++)
            {
                muscles[i].Contract = output[0][2 * i + 1] > 0;
            }
        }

        Matrix Sigma(Matrix m)
        {
            for (var x = 0; x < m.M; x++)
            {
                for (var y = 0; y < m.N; y++)
                {
                    m[x][y] = 1 / (1 + Mathf.Exp(-m[x][y]));
                }
            }
            return m;
        }
    }
}
