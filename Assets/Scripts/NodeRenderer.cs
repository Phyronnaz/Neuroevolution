using UnityEngine;

namespace Evolution
{
    public class NodeRenderer : MonoBehaviour
    {
        public int Id;

        public void SetPosition(Vector2 position)
        {
            transform.position = (Vector3)position;
        }
    }
}