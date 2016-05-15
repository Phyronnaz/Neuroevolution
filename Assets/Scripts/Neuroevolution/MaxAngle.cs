using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    class MaxAngle : MonoBehaviour
    {
        public void SetMaxAngle(string angle)
        {
            Globals.MaxAngle = float.Parse(angle);
        }
    }
}
