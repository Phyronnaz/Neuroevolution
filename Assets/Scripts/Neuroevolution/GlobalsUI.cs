using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    class GlobalsUI : MonoBehaviour
    {
        public void SetMaxAngle(string angle)
        {
            Globals.MaxAngle = float.Parse(angle);
        }

        public void SetAngleImpact(string impact)
        {
            Globals.BadAngleImpact = float.Parse(impact);
        }

        public void SetEnergyImpact(string impact)
        {
            Globals.EnergyImpact = float.Parse(impact);
        }

        public void SetDT(string dt)
        {
            Globals.DeltaTime = float.Parse(dt);
        }

        public void SetCycleDuration(string c)
        {
            Globals.CycleDuration = float.Parse(c);
        }

        public void SetGravity(string g)
        {
            Globals.WorldYGravity = float.Parse(g);
        }

        public void SetFriction(string f)
        {
            Globals.BodyFriction = float.Parse(f);
        }

        public void SetMotorTorque(string f)
        {
            Globals.MotorTorque = float.Parse(f);
        }

        public void SetDebug(bool b)
        {
            Globals.Debug = b;
        }

        public void SetMaxMotorTorque(string f)
        {
            Globals.MaxMotorTorque = float.Parse(f);
        }

        public void SetMaxPositon(string f)
        {
            Globals.MaxYPosition = float.Parse(f);
        }

        public void SetRandomCount(string f)
        {
            Globals.RandomCount = int.Parse(f);
        }
    }
}
