using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Assets.Scripts.Neuroevolution
{
    [Serializable]
    [XmlRoot(ElementName = "Creature")]
    public struct CreatureStruct
    {
        public List<Vector2> Positions;
        public List<DistanceJointStruct> DistanceJoints;
        public List<RevoluteJointStruct> RevoluteJoints;
        public int RotationNode;
        public List<Matrix> Synapses;

        public CreatureStruct(List<Vector2> positions, List<DistanceJointStruct> distanceJoints, List<RevoluteJointStruct> revoluteJoints, int rotationNode, List<Matrix> synapses)
        {
            Positions = positions;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;
            RotationNode = rotationNode;
            Synapses = synapses;
        }

        public CreatureStruct(List<Vector2> positions, List<DistanceJointStruct> distanceJoints, List<RevoluteJointStruct> revoluteJoints, int rotationNode)
        {
            Positions = positions;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;
            RotationNode = rotationNode;
            Synapses = new List<Matrix>();
        }
    }
}
