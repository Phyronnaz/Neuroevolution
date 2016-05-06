using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    public class Creature
    {
        public readonly List<FVector2> InitialPositions;
        public readonly List<DistanceJointStruct> DistanceJoints;
        public readonly List<RevoluteJointStruct> RevoluteJoints;
        public readonly List<Matrix> Synapses;
        public readonly int Generation;
        public readonly List<float> Speeds;
        readonly World world;
        List<RevoluteJoint> revoluteJoints;


        public Creature(List<FVector2> positions, List<DistanceJointStruct> distanceJoints, List<RevoluteJointStruct> revoluteJoints, List<Matrix> synapses, int generation)
        {
            this.revoluteJoints = new List<RevoluteJoint>();
            world = new World(new FVector2(0, -9.8f));
            world.ProcessChanges();
            Speeds = new List<float>();
            InitialPositions = positions;
            Synapses = synapses;
            Generation = generation;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;

            foreach (var p in positions)
            {
                world.AddBody(BodyFactory.CreateCircle(world, 1, 1, p));
            }
            world.ProcessChanges();
            foreach (var b in world.BodyList)
            {
                b.CollidesWith = Category.Cat1;
                b.CollisionCategories = Category.Cat10;
                b.LinearVelocity = FVector2.One * (-100);
                b.Awake = true;
            }
            world.ProcessChanges();
            var ground = BodyFactory.CreateRectangle(world, 1000000, 1, 1, FVector2.Zero);
            ground.IsStatic = true;
            ground.CollisionCategories = Category.Cat1;
            ground.CollidesWith = Category.Cat10;
            world.AddBody(ground);
            world.ProcessChanges();

            foreach (var r in revoluteJoints)
            {
                AddRevoluteJoint(r.a, r.b, r.anchor, r.lowerLimit, r.upperLimit, r.speed);
            }
        }


        public static Creature CloneCreature(Creature creature, float variation)
        {
            var synapses = new List<Matrix>();
            for (var k = 0; k < creature.Synapses.Count; k++)
            {
                synapses[k] = creature.Synapses[k] + Matrix.Random(creature.Synapses[k].M, creature.Synapses[k].N) * variation;
            }
            return new Creature(creature.InitialPositions, creature.DistanceJoints, creature.RevoluteJoints, synapses, creature.Generation + 1);
        }

        public void Update(float dt)
        {
            world.Step(dt);
            Train();
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
                neuralNetwork = Sigma(Matrix.Dot(neuralNetwork, Synapses[k]));
            }

            for (var i = 0; i < revoluteJoints.Count; i++)
            {
                revoluteJoints[i].MotorSpeed = neuralNetwork[0][2 * i + 1] * Speeds[i];
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
            world.AddJoint(JointFactory.CreateDistanceJoint(world, world.BodyList[a], world.BodyList[b], FVector2.Zero, FVector2.Zero));
        }

        void AddRevoluteJoint(int a, int b, int anchor, float lowerLimit, float upperLimit, float speed)
        {
            Speeds.Add(speed);
            var j = JointFactory.CreateRevoluteJoint(world.BodyList[a], world.BodyList[b], world.BodyList[anchor].Position);
            j.SetLimits(lowerLimit, upperLimit);
            world.AddJoint(j);
            revoluteJoints.Add(j);
        }

        public List<Body> GetBodies()
        {
            return world.BodyList.GetRange(0, world.BodyList.Count - 1);
        }

        public List<FarseerJoint> GetJoints()
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
    }
}
