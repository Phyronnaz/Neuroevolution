using UnityEngine;
using System.Collections.Generic;

namespace Evolution
{
    public class Creature : System.IComparable<Creature>
    {
        readonly List<Node> nodes;
        readonly Transform transform;
        float cycleDuration;
        float time;


        public Creature(List<Node> nodes, float cycleDuration, Transform transform)
        {
            this.nodes = nodes;
            this.cycleDuration = cycleDuration;
            this.transform = transform;
            foreach (var n in nodes)
            {
                n.NodeRenderer.GetComponent<Collider2D>().enabled = false;
            }
            //		matrix = new Matrix (new float[][] {
            //			new float[]{ 0, 0, 1 },
            //			new float[]{ 0, 1, 1 },
            //			new float[]{ 1, 0, 1 },
            //			new float[]{ 1, 1, 1 }
            //		});
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
            else {
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
                else {
                    ;
                }

                if (!alreadyAdded)
                {
                    //muscles.Add(IntelligentMuscle.RandomMuscle(nodes[t.a], nodes[t.b], cycleDuration, color, parent));
                    //k++;
                }
            }

            // Update graphics
            foreach (var n in nodes)
            {
                n.UpdateGraphics();
            }

            return new Creature(nodes, cycleDuration, parent);
        }

        public Creature Clone(float variation, Color color)
        {
            //var numberOfNodes = this.nodes.Count;
            //var numberOfMuscles = this.muscles.Count;
            ////		Color color = Random.ColorHSV ();
            //// Create creature
            var parent = new GameObject().transform;
            parent.name = "Creature " + Random.Range(0, 10000);

            //// Define arrays
            //var nodes = new List<Node>(numberOfNodes);
            //var muscles = new List<Muscle>(numberOfMuscles);

            //// Generate nodes
            //for (var i = 0; i < numberOfNodes; i++)
            //{
            //    nodes.Add(new Node(nodes[i].Position, parent, color, i));
            //}

            //// Generate muscles
            //for (var j = 0; j < numberOfMuscles; j++)
            //{
            //    var m = muscles[j];
            //    muscles.Add(m.Clone(nodes[m.Left.Id], nodes[m.Right.Id], variation, color, parent));
            //}

            //// Update graphics
            //foreach (var m in muscles)
            //{
            //    m.UpdateGraphics();
            //}
            //foreach (var n in nodes)
            //{
            //    n.UpdateGraphics();
            //}

            return new Creature(nodes, cycleDuration, parent);
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

            }
            else
            {
                foreach (var n in nodes)
                {
                    n.Update(deltaTime);
                }
            }

            // Update current time
            time += deltaTime;
        }

        public void UpdateGraphics()
        {
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
            foreach (var n in nodes)
            {
                n.Destroy();
            }
            Object.Destroy(transform.gameObject);
        }
    }
}

#region Comment

//
//	float Sigma (float x)
//	{
//		return 1 / (1 + Mathf.Exp (-x));
//	}
//
//	void Train ()
//	{
//		var hiddenSize = 32;
//
////
////X = np.array([  MuscleN.taille “””float””,
////MuscleN.etat “””1. si muscle étendu, 0. sinon”””,
////MuscleN.relation”””float”””])
////
////
////
////# seed random numbers to make calculation
////# deterministic (just a good practice)
////synapse_0 = 2*np.random.random((3,hiddenSize)) - 1
////synapse_1 = 2*np.random.random((hiddenSize,1)) - 1
////
////
////
////# initialize weights randomly with mean 0
////syn0 = 2*np.random.random((3,1)) - 1
////
////for iter in range(10000):
////
////# forward propagation
////l0 = X
////l1 = nonlin(np.dot(l0,syn0))
////
////print "Output After Training:"
////
////print l1
//	}
#endregion