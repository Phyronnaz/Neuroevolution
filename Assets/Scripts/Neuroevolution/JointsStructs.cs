using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Neuroevolution
{
    [Serializable]
    [XmlRoot(ElementName = "DistanceJoint")]
    public struct DistanceJointStruct
    {
        public int a;
        public int b;

        public DistanceJointStruct(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "RevoluteJoint")]
    public struct RevoluteJointStruct
    {
        public int a;
        public int b;
        public int anchor;
        public float lowerLimit;
        public float upperLimit;

        public RevoluteJointStruct(int a, int b, int anchor, float lowerLimit, float upperLimit)
        {
            this.a = a;
            this.b = b;
            this.anchor = anchor;
            this.lowerLimit = lowerLimit;
            this.upperLimit = upperLimit;
        }
    }
}
