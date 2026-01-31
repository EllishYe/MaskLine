using UnityEngine;

public class DragAll : MonoBehaviour
{
    private Transform dragging = null; 
    private Vector3 offset;

    [SerializeField] private LayerMask movableLayers;

    public WinController winController;


    void Update()
    {
        // Start Dragging
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity, movableLayers);
            if (hit)
            {
                dragging = hit.transform;
                offset = dragging.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        // Stop Dragging and Evaluate win condition
        else if (Input.GetMouseButtonUp(0)) {
            if (dragging != null)
            {
                var mask = dragging.GetComponent<MaskID>();
                if (mask != null)
                {
                    winController.EvaluateMaskOnTargets(mask);
                }
                else
                {
                    winController.CheckWinCondition();
                }
            }
            else
            {
                Debug.Log("[DragAll] MouseUp but no dragging object");
            }

            dragging = null;
        }

        // Dragging
        if (dragging != null) {
            dragging.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }
}
