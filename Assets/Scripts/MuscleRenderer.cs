using UnityEngine;

namespace Evolution
{
    public class MuscleRenderer : MonoBehaviour
    {
        public Color Color;

        public void Initialize(float length)
        {
           var  material = new Material(Shader.Find("Diffuse"));
            material.color = Color;
            .material = material;
        }

        public void SetPosition(Vector2 left, Vector2 right)
        {
            lineRenderer.SetPosition(0, (Vector3)left + Vector3.forward);
            lineRenderer.SetPosition(1, (Vector3)right + Vector3.forward);
        }
    }
}