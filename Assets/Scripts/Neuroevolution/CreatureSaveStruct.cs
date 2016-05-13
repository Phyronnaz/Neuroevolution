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
        public readonly int Generation;

        public CreatureSaveStruct(List<Vector2> initialPositions, List<DistanceJointStruct> distanceJoints,
            List<RevoluteJointStruct> revoluteJoints, List<Matrix> synapses, int generation)
        {
            InitialPositions = initialPositions;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;
            Synapses = synapses;
            Generation = generation;
        }

        public CreatureSaveStruct(Creature creature)
        {
            InitialPositions = creature.InitialPositions;
            DistanceJoints = creature.DistanceJoints;
            RevoluteJoints = creature.RevoluteJoints;
            Synapses = creature.Synapses;
            Generation = creature.Generation;
        }

        public Creature ToCreature()
        {
            return new Creature(InitialPositions, DistanceJoints, RevoluteJoints, Synapses, Generation);
        }
    }
}
