using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Neuroevolution
{
    class GlobalsUI : MonoBehaviour
    {
        public Transform Ground;


        public void Start()
        {
            var panel = GameObject.Find("GlobalsPanel");
            foreach (var f in panel.GetComponentsInChildren<InputField>())
            {
                var s = f.name.Substring(0, f.name.Length - 5);
                f.text = typeof(Globals).GetField(s).GetValue(null).ToString();
            }
            foreach (var t in panel.GetComponentsInChildren<Toggle>())
            {
                var s = t.name.Substring(0, t.name.Length - 6);
                t.isOn = (bool)typeof(Globals).GetField(s).GetValue(null); ;
            }
        }

        public void SetMaxAngle(string angle)
        {
            Globals.MaxAngle = float.Parse(angle);
        }

        public void SetAngleImpact(string impact)
        {
            Globals.AngleImpact = float.Parse(impact);
        }

        public void SetEnergyImpact(string impact)
        {
            Globals.EnergyImpact = float.Parse(impact);
        }

        public void SetDT(string dt)
        {
            Globals.DeltaTime = float.Parse(dt);
            GetComponent<Main>().Awake();
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

        public void SetHiddenLayersCount(string s)
        {
            Globals.HiddenLayersCount = int.Parse(s);
        }
        public void SetHiddenSize(string s)
        {
            Globals.HiddenSize = int.Parse(s);
        }

        public void SetUseThreads(bool b)
        {
            Globals.UseThreads = b;
        }

        public void SetNoImpulse(bool b)
        {
            Globals.NoImpulse = b;
        }

        public void SetStable(bool b)
        {
            Globals.Stable = b;
        }
        public void SetDistanceImpact(string s)
        {
            Globals.DistanceImpact = float.Parse(s);
        }

        public void SetLog(bool b)
        {
            Globals.Log = b;
        }
    }
}
