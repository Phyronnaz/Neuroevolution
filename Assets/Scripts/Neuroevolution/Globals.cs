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
        public const float BodyFriction = 100;
        public const float BodyDensity = 1;
        public const float MotorTorque = 1000;
        public const bool Debug = false;
        public const float MaxAngle = Mathf.PI / 4;
        public const float BadAngleImpact = 100000;
        public const float MaxMotorTorque = 1000;
    }
}
