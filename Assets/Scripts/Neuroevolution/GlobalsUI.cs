using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    class GlobalsUI : MonoBehaviour
    {
        public Transform Ground;

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

        public void SetUseSpecies(bool b)
        {
            Globals.UseSpecies = b;
        }

        public void SetTrainCycle(string s)
        {
            Globals.TrainCycle = int.Parse(s);
        }
        public void SetRestitution(string s)
        {
            Globals.Restitution = float.Parse(s);
        }

        public void SetKillFallen(bool b)
        {
            Globals.KillFallen = b;
        }

        public void SetGroundRotation(float f)
        {
            Globals.GroundRotation = f;
            var r = Ground.rotation;
            var e = r.eulerAngles;
            e.z = Globals.GroundRotation * Mathf.Rad2Deg;
            r.eulerAngles = e;
            Ground.rotation = r;
        }
    }
}
