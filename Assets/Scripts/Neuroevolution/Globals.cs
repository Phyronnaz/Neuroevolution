using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    public static class Globals
    {
        public const float BodyDensity = 1;
        public static float CycleDuration = 3;
        public static float WorldYGravity = -9.8f;
        public static float BodyFriction = 100000;
        public static float MotorTorque = 1000;
        public static bool Debug = false;
        public static float MaxMotorTorque = 1000;
        public static float MaxYPosition = 30;
        public static float MaxAngle = Mathf.PI / 3;
        public static float BadAngleImpact = -100000;
        public static float EnergyImpact = -1;
        public static float DeltaTime = 0.01f;
        public static int RandomCount = 5;
        public static bool UseSpecies = true;
        public static int TrainCycle = 1;
        public static float Restitution = 0.1f;
        public static bool KillFallen;
        public static float GroundRotation = 0;
    }
}
