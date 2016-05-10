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
        readonly World world;
        List<RevoluteJoint> revoluteJoints;
        float time;
        float timeModulo = 2;

        public Creature(List<Vector2> positions, List<DistanceJointStruct> distanceJoints,
            List<RevoluteJointStruct> revoluteJoints, List<Matrix> synapses, int generation)
        {
            this.revoluteJoints = new List<RevoluteJoint>();
            world = new World(new Vector2(0, -9.8f));
            InitialPositions = positions;
            Synapses = synapses;
            Generation = generation;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;

            //Add nodes
            foreach (var p in positions)
            {
                var body = BodyFactory.CreateCircle(world, 1, 1, p, null);
                body.CollidesWith = Category.Cat1;
                body.CollisionCategories = Category.Cat2;
                body.IsStatic = false;
                body.IgnoreCCD = true;
                body.Friction = 100;
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
        }


        public static Creature CloneCreature(Creature creature, float variation)
        {
            var synapses = new List<Matrix>();
            for (var k = 0; k < creature.Synapses.Count; k++)
            {
                synapses.Add(creature.Synapses[k] + Matrix.Random(creature.Synapses[k].M, creature.Synapses[k].N) * variation);
            }
            return new Creature(creature.InitialPositions, creature.DistanceJoints, creature.RevoluteJoints,
                synapses, creature.Generation + 1);
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
            j.SetLimits(r.lowerLimit, r.upperLimit);
            j.Enabled = true;
            j.MotorEnabled = true;
            j.MaxMotorTorque = 1000;
            world.AddJoint(j);
            revoluteJoints.Add(j);
        }
        public void Update(float dt)
        {
            world.Step(dt);
            time += dt;
            Train();
        }

        void Train()
        {
            //for (var k = 0; k < revoluteJoints.Count; k++)
            //{
            //    if (time % timeModulo > 5 && revoluteJoints[k].MotorSpeed >= 0)
            //    {
            //        revoluteJoints[k].MotorSpeed = -RevoluteJoints[k].speed;
            //    }
            //    else if (time % timeModulo < 5 && revoluteJoints[k].MotorSpeed <= 0)
            //    {
            //        revoluteJoints[k].MotorSpeed = RevoluteJoints[k].speed;
            //    }

            //}
            //return;
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
            neuralNetwork[0][2 * revoluteJoints.Count] = 2 * (time % timeModulo) / timeModulo - 1;

            //Process
            for (var k = 0; k < Synapses.Count; k++)
            {
                neuralNetwork = Sigma(Matrix.Dot(neuralNetwork, Synapses[k]));
            }

            //Change speeds
            for (var i = 0; i < revoluteJoints.Count; i++)
            {
                revoluteJoints[i].MotorSpeed = neuralNetwork[0][i] * RevoluteJoints[i].speed;
            }
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

        public void Reset()
        {
            for (var k = 0; k < InitialPositions.Count; k++)
            {
                world.BodyList[k].Position = InitialPositions[k];
                world.BodyList[k].ResetDynamics();
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
