using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static float MaxAngle = Mathf.PI / 3;
        public const float BadAngleImpact = 100000;
        public const float MaxMotorTorque = 1000;
        public const float MaxYPosition = 30;
    }
}
