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

    }
}
