using System;
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
        private readonly List<Vector2> initialPositions;
        private readonly List<DistanceJointStruct> distanceJointStructs;
        private readonly List<RevoluteJointStruct> revoluteJointStructs;
        private readonly int rotationNode;
        private readonly List<Matrix> synapses;
        private readonly int generation;
        private readonly int genome;
        private readonly int species;
        private readonly int parent;

        private readonly World world;
        private readonly List<RevoluteJoint> revoluteJoints;

        private readonly float initialRotation;
        private readonly bool useRotation;

        private float energy;
        private float time;
        private bool isDead;

        private float currentFriction;
        private float maxTorque;
        private float currentRestitution;

        private Matrix neuralNetwork;
        private int count;

        private static int genomeCount;
        private static int GenomeCount
        {
            get
            {
                genomeCount++;
                return genomeCount - 1;
            }
        }

        private static int speciesCount;
        private static int SpeciesCount
        {
            get
            {
                speciesCount++;
                return speciesCount - 1;
            }
        }



        public Creature(CreatureStruct creature, List<Matrix> synapses, int generation, int genome, int species, int parent) : this(creature, generation, genome, species, parent)
        {
            this.synapses = synapses;
            Train();
        }

        public Creature(CreatureStruct creature, List<Matrix> synapses) : this(creature, 0, GenomeCount, SpeciesCount, -1)
        {
            this.synapses = synapses;
            Train();
        }

        public Creature(CreatureStruct creature, int hiddenSize, int hiddenLayersCount) : this(creature, 0, GenomeCount, SpeciesCount, -1)
        {
            hiddenSize = Mathf.Max(hiddenSize, revoluteJoints.Count * 2 + 1);
            synapses = new List<Matrix>();
            synapses.Add(Matrix.Random(revoluteJoints.Count * 2 + 1, hiddenSize));
            for (var k = 1; k < hiddenLayersCount; k++)
            {
                synapses.Add(Matrix.Random(hiddenSize, hiddenSize));
            }
            synapses.Add(Matrix.Random(hiddenSize, revoluteJoints.Count));
            Train();
        }

        private Creature(CreatureStruct creature, int generation, int genome, int species, int parent)
        {
            this.genome = genome;
            this.species = species;
            this.parent = parent;
            this.generation = generation;
            initialPositions = creature.Positions;
            distanceJointStructs = creature.DistanceJoints;
            revoluteJointStructs = creature.RevoluteJoints;
            rotationNode = creature.RotationNode;
            revoluteJoints = new List<RevoluteJoint>();
            world = new World(new Vector2(0, Globals.WorldYGravity));

            //Add nodes
            foreach (var p in initialPositions)
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
            var ground = BodyFactory.CreateRectangle(world, 1000000, 1, 1, Vector2.Zero, null);
            ground.IsStatic = true;
            ground.CollisionCategories = Category.Cat1;
            ground.CollidesWith = Category.Cat2;
            world.AddBody(ground);
            //Compute bodies
            world.ProcessChanges();

            //Add joints
            foreach (var r in revoluteJointStructs)
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
            foreach (var d in distanceJointStructs)
            {
                world.AddJoint(JointFactory.CreateDistanceJoint(world, world.BodyList[d.a], world.BodyList[d.b], Vector2.Zero, Vector2.Zero));
            }
            world.ProcessChanges();

            //Rotation node
            if (rotationNode != -1)
            {
                useRotation = true;
                initialRotation = world.BodyList[rotationNode].Rotation;
            }
        }



        public Creature Clone(float variation)
        {
            var synapses = new List<Matrix>();
            for (var k = 0; k < this.synapses.Count; k++)
            {
                synapses.Add(this.synapses[k] + Matrix.Random(this.synapses[k].M, this.synapses[k].N) * variation);
            }
            var c = new CreatureStruct(initialPositions, distanceJointStructs, revoluteJointStructs, rotationNode);
            return new Creature(c, synapses, generation + 1, GenomeCount, species, genome);
        }

        public Creature Duplicate()
        {
            var c = new CreatureStruct(initialPositions, distanceJointStructs, revoluteJointStructs, rotationNode);
            return new Creature(c, synapses, generation, genome, species, parent);
        }

        public Creature RandomClone()
        {
            var c = new CreatureStruct(initialPositions, distanceJointStructs, revoluteJointStructs, rotationNode);
            return new Creature(c, synapses[0].N, synapses.Count - 1);
        }



        public void Update(float dt)
        {
            if (!isDead)
            {
                world.Step(dt);
                time += dt;
                count++;
                if (Globals.Debug)
                {
                    foreach (var r in revoluteJoints)
                    {
                        var x = Globals.DeltaTime * Globals.MotorTorque * ((time % Globals.CycleDuration > Globals.CycleDuration / 2) ? 1 : -1);
                        energy += Mathf.Abs(r.MotorSpeed - x);
                        r.MotorSpeed = x;
                    }
                }
                else
                {
                    if (count >= Globals.TrainCycle)
                    {
                        count = 0;
                        Train();
                    }
                    //Change speeds
                    for (var i = 0; i < revoluteJoints.Count; i++)
                    {
                        var x = Globals.DeltaTime * neuralNetwork[0][i] * Globals.MotorTorque;
                        energy += Mathf.Abs(revoluteJoints[i].MotorSpeed - x);
                        revoluteJoints[i].MotorSpeed = x;
                    }
                }
            }

            if ((world.BodyList[0].Position.Y > Globals.MaxYPosition) || (useRotation && Globals.KillFallen && GetAngle() > Globals.MaxAngle))
            {
                isDead = true;
            }
            //Globals update
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
            for (var k = 0; k < synapses.Count; k++)
            {
                neuralNetwork = Sigma(Matrix.Dot(neuralNetwork, synapses[k]));
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
            if (isDead)
            {
                return float.NegativeInfinity;
            }
            else
            {
                var x = 0f;
                if (useRotation)
                {
                    x = (GetAngle() > Globals.MaxAngle) ? 1 : 0;
                }
                return GetAveragePosition() + x * Globals.BadAngleImpact + GetPower() * Globals.EnergyImpact;
            }
        }

        public int GetGenome()
        {
            return genome;
        }

        public int GetSpecies()
        {
            return species;
        }

        public int GetParent()
        {
            return parent;
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
            if (rotationNode == -1)
            {
                return 0;
            }
            else
            {
                return Mathf.Abs(world.BodyList[rotationNode].Rotation - initialRotation);
            }
        }

        public int GetGeneration()
        {
            return generation;
        }

        public CreatureSaveStruct GetSave()
        {
            return new CreatureSaveStruct(initialPositions, distanceJointStructs, revoluteJointStructs, rotationNode, synapses);
        }
    }
}
