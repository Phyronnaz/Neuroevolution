using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Neuroevolution
{
    public static class Globals
    {
        public const float BodyDensity = 1;
        public static float CycleDuration = 3;
        public static float WorldYGravity = -9.8f;
        public static float BodyFriction = 50;
        public static float MotorTorque = 10;
        public static bool Debug = false;
        public static float MaxMotorTorque = 50000;
        public static float MaxYPosition = 30;
        public static float MaxAngle = Mathf.PI / 3;
        public static float AngleImpact = 0;
        public static float EnergyImpact = 0;
        public static float DistanceImpact = 1;
        public static float DeltaTime = 0.01f;
        public static int RandomCount = 5;
        public static bool UseSpecies = true;
        public static int TrainCycle = 1;
        public static float Restitution = 0;
        public static bool KillFallen = true;
        public static float GroundRotation = 0;
        public static int HiddenLayersCount = 2;
        public static int HiddenSize = 0;
        public static bool UseThreads = true;
        public static bool NoImpulse = false;
        public static bool Stable = true;
        public static bool Log = true;
        public static List<int> SpeciesSizes = new List<int>() { 5, 4, 3, 2, 1 };
    }
}
