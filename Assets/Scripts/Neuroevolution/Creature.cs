using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Mathf = UnityEngine.Mathf;

namespace Assets.Scripts.Neuroevolution
{
    public class Creature : IComparable
    {
        public List<Vector2> InitialPositions;
        public List<DistanceJointStruct> DistanceJoints;
        public List<RevoluteJointStruct> RevoluteJoints;
        public int RotationNode;
        public List<Matrix> Synapses;
        public int Generation;
        public int Genome;
        public int Parent;
        public float Energy;
        World world;
        List<RevoluteJoint> revoluteJoints;
        float time;
        float initialRotation;
        bool useRotation;
        bool isDead;

        static int genomeCount;

        static int GenomeCount
        {
            get
            {
                genomeCount++;
                return genomeCount - 1;
            }
        }

        public Creature(List<Vector2> positions, List<DistanceJointStruct> distanceJoints,
            List<RevoluteJointStruct> revoluteJoints, int rotationNode, List<Matrix> synapses, int generation, int genome, int parent)
        {
            Initialize(positions, distanceJoints, revoluteJoints, rotationNode, synapses, generation, genome, parent);
        }

        public Creature(List<Vector2> positions, List<DistanceJointStruct> distanceJoints,
            List<RevoluteJointStruct> revoluteJoints, int rotationNode, List<Matrix> synapses)
        {
            Initialize(positions, distanceJoints, revoluteJoints, rotationNode, synapses, 0, GenomeCount, -1);
        }

        public Creature(List<Vector2> positions, List<DistanceJointStruct> distanceJoints,
           List<RevoluteJointStruct> revoluteJoints, int rotationNode, int hiddenSize, int hiddenLayersCount)
        {
            hiddenSize = Mathf.Max(hiddenSize, revoluteJoints.Count * 2 + 1);
            var synapses = new List<Matrix>();
            synapses.Add(Matrix.Random(revoluteJoints.Count * 2 + 1, hiddenSize));
            for (var k = 1; k < hiddenLayersCount; k++)
            {
                synapses.Add(Matrix.Random(hiddenSize, hiddenSize));
            }
            synapses.Add(Matrix.Random(hiddenSize, revoluteJoints.Count));

            Initialize(positions, distanceJoints, revoluteJoints, rotationNode, synapses, 0, GenomeCount, -1);
        }

        void Initialize(List<Vector2> positions, List<DistanceJointStruct> distanceJoints,
        List<RevoluteJointStruct> revoluteJoints, int rotationNode, List<Matrix> synapses, int generation, int genome, int parent)
        {
            Genome = genome;
            Parent = parent;
            this.revoluteJoints = new List<RevoluteJoint>();
            world = new World(new Vector2(0, Globals.WorldYGravity));
            InitialPositions = positions;
            Synapses = synapses;
            Generation = generation;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;
            RotationNode = rotationNode;

            //Add nodes
            foreach (var p in positions)
            {
                var body = BodyFactory.CreateCircle(world, 0.5f, Globals.BodyDensity, p, null);
                body.CollidesWith = Category.Cat1;
                body.CollisionCategories = Category.Cat2;
                body.IsStatic = false;
                body.Friction = Globals.BodyFriction;
                world.AddBody(body);
            }
            //Add ground
            var ground = BodyFactory.CreateRectangle(world, 1000000, 1, 1, Vector2.Zero, null);
            ground.IsStatic = true;
            ground.CollisionCategories = Category.Cat1;
            ground.CollidesWith = Category.Cat2;
            world.AddBody(ground);
            //Compute bodies
            world.ProcessChanges();

            //Add joints
            foreach (var r in revoluteJoints)
            {
                AddRevoluteJoint(r);
            }
            foreach (var d in distanceJoints)
            {
                AddDistanceJoint(d);
            }
            world.ProcessChanges();
            if (rotationNode != -1)
            {
                useRotation = true;
                initialRotation = world.BodyList[rotationNode].Rotation;
            }
        }

        void AddDistanceJoint(DistanceJointStruct d)
        {
            world.AddJoint(JointFactory.CreateDistanceJoint(world, world.BodyList[d.a], world.BodyList[d.b], Vector2.Zero, Vector2.Zero));
        }

        void AddRevoluteJoint(RevoluteJointStruct r)
        {
            var anchorA = world.BodyList[r.anchor].Position - world.BodyList[r.a].Position;
            var anchorB = world.BodyList[r.anchor].Position - world.BodyList[r.b].Position;
            var j = JointFactory.CreateRevoluteJoint(world, world.BodyList[r.a], world.BodyList[r.b], anchorA, anchorB, false);
            j.LimitEnabled = true;
            j.SetLimits(-r.lowerLimit, r.upperLimit);
            j.Enabled = true;
            j.MotorEnabled = true;
            j.MaxMotorTorque = Globals.MaxMotorTorque;
            world.AddJoint(j);
            revoluteJoints.Add(j);
        }



        public static Creature CloneCreature(Creature creature, float variation)
        {
            var synapses = new List<Matrix>();
            for (var k = 0; k < creature.Synapses.Count; k++)
            {
                synapses.Add(creature.Synapses[k] + Matrix.Random(creature.Synapses[k].M, creature.Synapses[k].N) * variation);
            }
            return new Creature(creature.InitialPositions, creature.DistanceJoints, creature.RevoluteJoints, creature.RotationNode,
                synapses, creature.Generation + 1, GenomeCount, creature.Genome);
        }

        public static Creature DuplicateCreature(Creature creature)
        {
            return new Creature(creature.InitialPositions, creature.DistanceJoints, creature.RevoluteJoints, creature.RotationNode,
               creature.Synapses, creature.Generation, creature.Genome, creature.Parent);
        }



        Matrix Sigma(Matrix m)
        {
            for (var x = 0; x < m.M; x++)
            {
                for (var y = 0; y < m.N; y++)
                {
                    m[x][y] = 1 / (1 + Mathf.Exp(-m[x][y])) * 2 - 1;
                }
            }
            return m;
        }



        public void Update(float dt)
        {
            if (!isDead)
            {
                world.Step(dt);
                time += dt;
                Train();
            }
            if (world.BodyList[0].Position.Y > Globals.MaxYPosition)
            {
                isDead = true;
            }
        }

        void Train()
        {
            if (Globals.Debug)
            {
                foreach (var r in revoluteJoints)
                {
                    if (time % 10 > 5)
                    {
                        r.MotorSpeed = Globals.MotorTorque;
                    }
                    else
                    {
                        r.MotorSpeed = -Globals.MotorTorque;
                    }

                }
            }
            else
            {
                var neuralNetwork = new Matrix(1, 2 * revoluteJoints.Count + 1);

                //Revolute joints entries
                for (var i = 0; i < revoluteJoints.Count; i++)
                {
                    var r = revoluteJoints[i];
                    //Angle between -1 and 1 based on limits
                    neuralNetwork[0][2 * i] = (r.JointAngle - r.LowerLimit) / (r.UpperLimit - r.LowerLimit) * 2 - 1;
                    //Rotation direction
                    neuralNetwork[0][2 * i + 1] = (revoluteJoints[i].MotorSpeed > 0) ? 1 : -1;
                }

                //Time entry
                neuralNetwork[0][2 * revoluteJoints.Count] = 2 * (time % Globals.CycleDuration) / Globals.CycleDuration - 1;

                //Process
                for (var k = 0; k < Synapses.Count; k++)
                {
                    neuralNetwork = Sigma(Matrix.Dot(neuralNetwork, Synapses[k]));
                }

                //Change speeds
                for (var i = 0; i < revoluteJoints.Count; i++)
                {
                    var x = Controller.DeltaTime * neuralNetwork[0][i] * Globals.MotorTorque;
                    revoluteJoints[i].MotorSpeed = x;
                    Energy += Mathf.Abs(x);
                }
            }
        }




        public List<Body> GetBodies()
        {
            return world.BodyList.GetRange(0, world.BodyList.Count - 1);
        }

        public List<Joint> GetJoints()
        {
            return world.JointList;
        }

        public float GetAveragePosition()
        {
            var a = 0f;
            foreach (var b in world.BodyList)
            {
                if (!b.IsStatic)
                {
                    a += b.Position.X;
                }
            }
            return a / (world.BodyList.Count - 1); //-1 : ground
        }



        public float GetFitness()
        {
            return 1 / Energy;
            if (isDead)
            {
                return -1000000000;
            }
            else
            {
                var x = 0f;
                if (useRotation)
                {
                    x = (Mathf.Abs(world.BodyList[RotationNode].Rotation - initialRotation) > Globals.MaxAngle) ? -1 : 0;
                }
                return (GetAveragePosition() + x * Globals.BadAngleImpact) / Energy;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is Creature)
            {
                return GetFitness().CompareTo(((Creature)obj).GetFitness());
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
