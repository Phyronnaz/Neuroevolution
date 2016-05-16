using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Assets.Scripts.Neuroevolution
{
    public struct CreatureStruct
    {
        public List<Vector2> Positions;
        public List<DistanceJointStruct> DistanceJoints;
        public List<RevoluteJointStruct> RevoluteJoints;
        public int RotationNode;

        public CreatureStruct(List<Vector2> positions, List<DistanceJointStruct> distanceJoints, List<RevoluteJointStruct> revoluteJoints, int rotationNode)
        {
            Positions = positions;
            DistanceJoints = distanceJoints;
            RevoluteJoints = revoluteJoints;
            RotationNode = rotationNode;
        }
    }
}
