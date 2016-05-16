using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Assets.Scripts.Neuroevolution
{
    [Serializable]
    [XmlRoot(ElementName = "Creature")]
    public struct CreatureSaveStruct
    {
        public readonly List<Vector2> InitialPositions;
        public readonly List<DistanceJointStruct> DistanceJointStructs;
        public readonly List<RevoluteJointStruct> RevoluteJointStructs;
        public int RotationNode;
        public readonly List<Matrix> Synapses;

        public CreatureSaveStruct(List<Vector2> initialPositions, List<DistanceJointStruct> distanceJoints,
            List<RevoluteJointStruct> revoluteJoints, int rotationNode, List<Matrix> synapses)
        {
            InitialPositions = initialPositions;
            DistanceJointStructs = distanceJoints;
            RevoluteJointStructs = revoluteJoints;
            RotationNode = rotationNode;
            Synapses = synapses;
        }

        public Creature ToCreature()
        {
            return new Creature(new CreatureStruct(InitialPositions, DistanceJointStructs, RevoluteJointStructs, RotationNode), Synapses);
        }
    }
}
