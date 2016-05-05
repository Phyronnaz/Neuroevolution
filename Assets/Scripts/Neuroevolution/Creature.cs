using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Assets.Scripts.Neuroevolution
{
    class Creature
    {
        World world;
        List<RevoluteJoint> revoluteJoints;
        public Creature (List<FVector2> positions)
        {
            world = new World(new FVector2(0, -9.8f));
            revoluteJoints = new List<RevoluteJoint>();
            foreach(var p in positions)
            {
                world.AddBody(BodyFactory.CreateBody(world, p));
            }
        }

        public void Update (float dt)
        {
            world.Step(dt);
            revoluteJoints[0].
        }

        public void AddDistanceJoint (int a, int b)
        {
            world.AddJoint(JointFactory.CreateDistanceJoint(world, world.BodyList[a], world.BodyList[b], FVector2.Zero, FVector2.Zero));
        }

        public void AddRevoluteJoint(int a, int b, int anchor, float lowerLimit, float upperLimit)
        {
            var j = JointFactory.CreateRevoluteJoint(world.BodyList[a], world.BodyList[b], world.BodyList[anchor].Position);
            j.SetLimits(lowerLimit, upperLimit);
            world.AddJoint(j);
            revoluteJoints.Add(j);
        }

        public List<Body> GetBodies ()
        {
            return world.BodyList;
        }

        public List<FarseerJoint> GetJoints ()
        {
            return world.JointList;
        }
    }
}
