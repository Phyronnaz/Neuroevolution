using UnityEngine;

public class DragUI : MonoBehaviour
{
    private Vector3 deltaPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            deltaPosition = transform.position - Input.mousePosition;
        }
    }

    public void OnDrag()
    {
        transform.position = Input.mousePosition + deltaPosition;
    }
}
