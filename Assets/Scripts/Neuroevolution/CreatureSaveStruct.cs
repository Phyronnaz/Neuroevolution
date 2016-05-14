using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Assets.Scripts.Neuroevolution
{
    [Serializable]
    [XmlRoot(ElementName = "Creature")]
    public struct CreatureSaveStruct
    {
        public readonly List<Vector2> InitialPositions;
        public readonly List<DistanceJointStruct> DistanceJoints;
        public readonly List<RevoluteJointStruct> RevoluteJoints;
        public readonly List<Matrix> Synapses;

        public CreatureSaveStruct(List<Vector2> initialPositions, List<DistanceJointStruct> distanceJoints,
            List<RevoluteJointStruct> revoluteJoints, List<Matrix> synapses)
        {
            InitialPositions = initialPositions;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;
            Synapses = synapses;
        }

        public CreatureSaveStruct(Creature creature)
        {
            InitialPositions = creature.InitialPositions;
            DistanceJoints = creature.DistanceJoints;
            RevoluteJoints = creature.RevoluteJoints;
            Synapses = creature.Synapses;
        }

        public Creature ToCreature()
        {
            return new Creature(InitialPositions, DistanceJoints, RevoluteJoints, Synapses);
        }
    }
}
