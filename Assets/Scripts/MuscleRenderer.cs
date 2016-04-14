using UnityEngine;

namespace Evolution
{
    public class MuscleRenderer : MonoBehaviour
    {
        public float MaxLength;
        public float MinLength;
        public Color Color;

        Material material;
        LineRenderer lineRenderer;
        LineRenderer max;
        LineRenderer min;
        bool isContracting;

        public void Initialize()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            material = new Material(Shader.Find("Diffuse"));
            material.color = Color;
            lineRenderer.material = material;

            max = new GameObject().AddComponent<LineRenderer>();
            min = new GameObject().AddComponent<LineRenderer>();

            max.transform.parent = transform;
            min.transform.parent = transform;

            max.SetWidth(0.25f, 0.25f);
            min.SetWidth(0.25f, 0.25f);

            max.material = new Material(Shader.Find("Transparent/Diffuse"));
            max.material.color = new Color(0, 1, 0, 0.5f);
            min.material = new Material(Shader.Find("Transparent/Diffuse"));
            min.material.color = new Color(0, 1, 0, 0.5f);
        }

        public void SetWidthAndColor(float width, bool contracting)
        {
            lineRenderer.SetWidth(width, width);
            if (Constants.MuscleDebug)
            {
                if (isContracting != contracting)
                {
                    isContracting = contracting;
                    if (contracting)
                    {
                        max.material.color = new Color(1, 0, 0, 0.5f);
                        min.material.color = new Color(1, 0, 0, 0.5f);
                    }
                    else
                    {
                        max.material.color = new Color(0, 1, 0, 0.5f);
                        min.material.color = new Color(0, 1, 0, 0.5f);
                    }
                }
            }
        }

        public void SetPosition(Vector2 left, Vector2 right)
        {
            lineRenderer.SetPosition(0, (Vector3)left + Vector3.forward);
            lineRenderer.SetPosition(1, (Vector3)right + Vector3.forward);
            if (Constants.MuscleDebug)
            {
                max.enabled = true;
                min.enabled = true;
                var center = (left + right) / 2;
                max.SetPosition(0, (Vector3)(center - left).normalized * MaxLength / 2 + (Vector3)center + Vector3.back);
                max.SetPosition(1, (Vector3)(center - right).normalized * MaxLength / 2 + (Vector3)center + Vector3.back);

                min.SetPosition(0, (Vector3)(center - left).normalized * MinLength / 2 + (Vector3)center + Vector3.back);
                min.SetPosition(1, (Vector3)(center - right).normalized * MinLength / 2 + (Vector3)center + Vector3.back);
            }
            else
            {
                max.enabled = false;
                min.enabled = false;
            }
        }
    }
}