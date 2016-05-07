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
        public readonly List<Vector2> InitialPositions;
        public readonly List<DistanceJointStruct> DistanceJoints;
        public readonly List<RevoluteJointStruct> RevoluteJoints;
        public readonly List<Matrix> Synapses;
        public readonly int Generation;
        public readonly List<float> Speeds;
        readonly World world;
        List<RevoluteJoint> revoluteJoints;

        public float currentTime;


        public Creature(List<Vector2> positions, List<DistanceJointStruct> distanceJoints, List<RevoluteJointStruct> revoluteJoints, List<Matrix> synapses, int generation)
        {
            this.revoluteJoints = new List<RevoluteJoint>();
            world = new World(new Vector2(0, -9.8f));
            world.ProcessChanges();
            Speeds = new List<float>();
            InitialPositions = positions;
            Synapses = synapses;
            Generation = generation;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;

            foreach (var p in positions)
            {
                world.AddBody(BodyFactory.CreateCircle(world, 1, 1, p, null));
            }
            world.ProcessChanges();
            foreach (var b in world.BodyList)
            {
                b.CollidesWith = Category.Cat1;
                b.CollisionCategories = Category.Cat10;
                b.IsStatic = false;
                b.IgnoreCCD = true;
            }
            world.ProcessChanges();
            var ground = BodyFactory.CreateRectangle(world, 1000000, 1, 1, Vector2.Zero, null);
            ground.IsStatic = true;
            ground.CollisionCategories = Category.Cat1;
            ground.CollidesWith = Category.Cat10;
            world.AddBody(ground);
            world.ProcessChanges();

            foreach (var r in revoluteJoints)
            {
                AddRevoluteJoint(r.a, r.b, r.anchor, r.lowerLimit, r.upperLimit, r.speed);
            }
            foreach (var d in distanceJoints)
            {
                AddDistanceJoint(d.a, d.b);
            }
            world.ProcessChanges();
        }


        public static Creature CloneCreature(Creature creature, float variation)
        {
            var synapses = new List<Matrix>();
            for (var k = 0; k < creature.Synapses.Count; k++)
            {
                synapses.Add(creature.Synapses[k] + Matrix.Random(creature.Synapses[k].M, creature.Synapses[k].N) * variation);
            }
            return new Creature(creature.InitialPositions, creature.DistanceJoints, creature.RevoluteJoints, synapses, creature.Generation + 1);
        }

        public void Update(float dt)
        {
            world.Step(dt);
            Train();
            currentTime += dt;
        }

        void Train()
        {
            var neuralNetwork = new Matrix(1, 2 * revoluteJoints.Count);

            for (var i = 0; i < revoluteJoints.Count; i++)
            {
                neuralNetwork[0][2 * i] = revoluteJoints[i].JointAngle / Mathf.PI;
                neuralNetwork[0][2 * i + 1] = (revoluteJoints[i].MotorSpeed > 0) ? 1 : -1;
            }

            for (var k = 0; k < Synapses.Count; k++)
            {
                UnityEngine.Debug.Log(neuralNetwork.N);
                UnityEngine.Debug.Log(Synapses[k].M);
                neuralNetwork = Sigma(Matrix.Dot(neuralNetwork, Synapses[k]));
            }

            for (var i = 0; i < revoluteJoints.Count; i++)
            {
                revoluteJoints[i].MotorSpeed = neuralNetwork[0][i] * Speeds[i];
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

        public void Reset()
        {
            for (var k = 0; k < InitialPositions.Count; k++)
            {
                world.BodyList[k].Position = InitialPositions[k];
            }
        }

        void AddDistanceJoint(int a, int b)
        {
            world.AddJoint(JointFactory.CreateDistanceJoint(world, world.BodyList[a], world.BodyList[b], Vector2.Zero, Vector2.Zero));
        }

        void AddRevoluteJoint(int a, int b, int anchor, float lowerLimit, float upperLimit, float speed)
        {
            Speeds.Add(speed);
            var anchorA = world.BodyList[anchor].Position - world.BodyList[a].Position;
            var anchorB = world.BodyList[anchor].Position - world.BodyList[b].Position;
            var j = JointFactory.CreateRevoluteJoint(world, world.BodyList[a], world.BodyList[b], anchorA, anchorB, false);
            j.SetLimits(lowerLimit, upperLimit);
            j.LimitEnabled = true;
            j.MotorEnabled = true;
            j.Enabled = true;
            world.AddJoint(j);
            revoluteJoints.Add(j);
        }

        public List<Body> GetBodies()
        {
            UnityEngine.Debug.Log(currentTime);
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
                a += b.Position.X;
            }
            return a / world.BodyList.Count;
        }

        public int CompareTo(object obj)
        {
            if (obj is Creature)
            {
                return GetAveragePosition().CompareTo(((Creature)obj).GetAveragePosition());
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
