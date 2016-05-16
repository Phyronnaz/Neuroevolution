using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    public static class Globals
    {
        public const float CycleDuration = 3;
        public const float WorldYGravity = -9.8f;
        public const float BodyFriction = 100000;
        public const float BodyDensity = 1;
        public const float MotorTorque = 1000;
        public const bool Debug = false;
        public const float MaxMotorTorque = 1000;
        public const float MaxYPosition = 30;
        public static float MaxAngle = Mathf.PI / 3;
        public static float BadAngleImpact = -100000;
        public static float EnergyImpact = -1;
        public static float DeltaTime = 0.01f;
    }
}
