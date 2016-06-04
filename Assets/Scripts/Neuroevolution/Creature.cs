using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Mathf = UnityEngine.Mathf;

namespace Assets.Scripts.Neuroevolution
{
    public class Creature
    {
        //External
        public readonly CreatureStruct CreatureStruct;
        public readonly int Generation;
        public readonly int Genome;
        public readonly int Species;
        public readonly int Parent;
        public bool IsDead;

        //Internal
        private readonly World world;
        private readonly Body ground;
        private readonly List<RevoluteJoint> revoluteJoints;

        private readonly float initialRotation;
        private readonly bool useRotation;

        private Matrix neuralNetwork;
        private int count;

        //Stats
        private float energy;
        private float time;

        //Globals
        private float currentFriction;
        private float maxTorque;
        private float currentRestitution;




        public Creature(CreatureStruct creature, int generation, int genome, int species, int parent)
        {
            Genome = genome;
            Species = species;
            Parent = parent;
            Generation = generation;
            CreatureStruct = creature;
            revoluteJoints = new List<RevoluteJoint>();
            world = new World(new Vector2(0, Globals.WorldYGravity));

            //Add nodes
            foreach (var p in creature.Positions)
            {
                var body = BodyFactory.CreateCircle(world, 0.5f, Globals.BodyDensity, p, null);
                body.CollidesWith = Category.Cat1;
                body.CollisionCategories = Category.Cat2;
                body.IsStatic = false;
                body.Friction = Globals.BodyFriction;
                body.Restitution = Globals.Restitution;
                world.AddBody(body);
            }
            currentFriction = Globals.BodyFriction;
            maxTorque = Globals.MaxMotorTorque;
            currentFriction = Globals.Restitution;
            //Add ground
            ground = BodyFactory.CreateRectangle(world, 1000000, 1, 1, Vector2.Zero, null);
            ground.IsStatic = true;
            ground.CollisionCategories = Category.Cat1;
            ground.CollidesWith = Category.Cat2;
            ground.Rotation = Globals.GroundRotation;
            world.AddBody(ground);
            //Compute bodies
            world.ProcessChanges();

            //Add joints
            foreach (var r in creature.RevoluteJoints)
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
            foreach (var d in creature.DistanceJoints)
            {
                world.AddJoint(JointFactory.CreateDistanceJoint(world, world.BodyList[d.a], world.BodyList[d.b], Vector2.Zero, Vector2.Zero));
            }
            world.ProcessChanges();

            //Rotation node
            if (CreatureStruct.RotationNode != -1)
            {
                useRotation = true;
                initialRotation = world.BodyList[CreatureStruct.RotationNode].Rotation;
            }
            Train();
        }



        public Creature GetChild(float variation)
        {
            variation = Counters.GetVariation(Species, variation);
            var synapses = new List<Matrix>();
            for (var k = 0; k < CreatureStruct.Synapses.Count; k++)
            {
                synapses.Add(CreatureStruct.Synapses[k] + Matrix.Random(CreatureStruct.Synapses[k].M, CreatureStruct.Synapses[k].N) * variation);
            }
            var c = CreatureStruct;
            c.Synapses = synapses;
            return new Creature(c, Generation + 1, Counters.GenomeCount, Species, Genome);
        }

        public Creature GetCopy()
        {
            return new Creature(CreatureStruct, Generation, Genome, Species, Parent);
        }

        public Creature GetRandomClone()
        {
            return CreatureFactory.CreateCreature(CreatureStruct, CreatureStruct.Synapses[0].N, CreatureStruct.Synapses.Count - 1);
        }



        public void Update(float dt)
        {
            if (!IsDead)
            {
                //Get and apply new settings
                ApplyGlobals();
                //Do the simulation
                world.Step(dt);
                //Increment counters
                time += dt;
                count++;
                //Train
                if (count >= Globals.TrainCycle)
                {
                    count = 0;
                    Train();
                    //Apply changes
                    if (Globals.Stable)
                    {
                        ChangeSpeeds();
                    }
                }
                if (!Globals.Stable)
                {
                    ChangeSpeeds();
                }
                //Kill if needed
                if ((world.BodyList[0].Position.Y > Globals.MaxYPosition) || (useRotation && Globals.KillFallen && GetAngle() > Globals.MaxAngle))
                {
                    IsDead = true;
                }
            }



        }

        private void ApplyGlobals()
        {
            if (currentFriction != Globals.BodyFriction)
            {
                foreach (var b in world.BodyList)
                {
                    b.Friction = Globals.BodyFriction;
                }
                currentFriction = Globals.BodyFriction;
            }
            if (currentRestitution != Globals.Restitution)
            {
                foreach (var b in world.BodyList)
                {
                    b.Restitution = Globals.Restitution;
                }
                currentRestitution = Globals.Restitution;
            }
            if (ground.Rotation != Globals.GroundRotation)
            {
                ground.Rotation = Globals.GroundRotation;
            }
            if (maxTorque != Globals.MaxMotorTorque)
            {
                foreach (var r in revoluteJoints)
                {
                    r.MaxMotorTorque = Globals.MaxMotorTorque;
                }
                maxTorque = Globals.MaxMotorTorque;
            }
            world.Gravity.Y = Globals.WorldYGravity;
        }

        private void ChangeSpeeds()
        {
            for (var i = 0; i < revoluteJoints.Count; i++)
            {
                float x;
                if (Globals.Debug)
                {
                    x = Globals.DeltaTime * Globals.MotorTorque * ((time % Globals.CycleDuration > Globals.CycleDuration / 2) ? 1 : -1);
                }
                else
                {
                    x = Globals.DeltaTime * neuralNetwork[0][i] * Globals.MotorTorque;
                }
                var r = revoluteJoints[i];
                energy += Mathf.Abs(x);
                if (Globals.NoImpulse)
                {
                    r.MotorSpeed = 0;
                }
                r.MotorSpeed += x;
                if ((Mathf.Abs(r.JointAngle - r.LowerLimit - r.ReferenceAngle) < 0.1f && r.MotorSpeed < 0)
                    || (Mathf.Abs(r.JointAngle - r.UpperLimit - r.ReferenceAngle) < 0.1f && r.MotorSpeed > 0))
                {
                    r.MotorSpeed = 0;
                }
            }
        }

        private void Train()
        {
            neuralNetwork = new Matrix(1, 2 * revoluteJoints.Count + 1);

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
            for (var k = 0; k < CreatureStruct.Synapses.Count; k++)
            {
                neuralNetwork = Sigma(Matrix.Dot(neuralNetwork, CreatureStruct.Synapses[k]));
            }
        }
        private static Matrix Sigma(Matrix m)
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



        public List<Body> GetBodies()
        {
            return world.BodyList.GetRange(0, world.BodyList.Count - 1);
        }

        public Vector2 GetAveragePosition()
        {
            var a = Vector2.Zero;
            foreach (var b in world.BodyList)
            {
                if (!b.IsStatic)
                {
                    a += b.Position;
                }
            }
            return a / (world.BodyList.Count - 1); //-1 : ground
        }

        public float GetFitness()
        {
            if (IsDead)
            {
                return float.NegativeInfinity;
            }
            else
            {
                var x = 0f;
                if (useRotation)
                {
                    x = GetAngle();
                }
                return GetAveragePosition().X * Globals.DistanceImpact + x * Globals.AngleImpact + GetPower() * Globals.EnergyImpact;
            }
        }

        public float GetPower()
        {
            return energy / time;
        }

        public float GetEnergy()
        {
            return energy;
        }

        public float GetAngle()
        {
            if (CreatureStruct.RotationNode == -1)
            {
                return 0;
            }
            else
            {
                return Mathf.Abs(world.BodyList[CreatureStruct.RotationNode].Rotation - initialRotation);
            }
        }
    }
}
