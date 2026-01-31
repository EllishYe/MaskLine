using UnityEngine;

public class Drag: MonoBehaviour
{
    private bool isDaragging = false;
    private Vector3 offset;


    void Update()
    {
        if (isDaragging)
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    }

    private void OnMouseDown()
    {
        isDaragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseUp()
    {
        isDaragging = false;
        
    }
}
