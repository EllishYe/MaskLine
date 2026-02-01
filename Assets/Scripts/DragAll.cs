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

                // 若开始拖拽的是一个 Mask，立即清除它在任何 target 上的占位
                var maskStart = dragging.GetComponent<MaskID>();
                if (maskStart != null && winController != null)
                {
                    winController.ClearOccupancyForMask(maskStart);
                }
            }
        }
        // Stop Dragging and Evaluate win condition
        else if (Input.GetMouseButtonUp(0)) {
            if (dragging != null)
            {
                var mask = dragging.GetComponent<MaskID>();
                if (mask != null)
                {
                    // basic safety checks
                    if (winController == null)
                    {
                        Debug.LogWarning("[DragAll] winController is not assigned!");
                    }
                    else if (winController.targets == null)
                    {
                        Debug.LogWarning("[DragAll] winController.targets is null!");
                    }

                    // 吸附：在所有目标中找与 mask ID 匹配且在 acceptRadius 内的最近目标
                    TargetSlot bestTarget = null;
                    float bestDist = float.MaxValue;
                    if (winController != null && winController.targets != null)
                    {
                        foreach (var target in winController.targets)
                        {
                            if (target == null) continue;
                            if (target.requiredMaskID != mask.tileID) continue;

                            float dist = Vector2.Distance(mask.transform.position, target.transform.position);
                            if (dist <= target.acceptRadius && dist < bestDist)
                            {
                                bestDist = dist;
                                bestTarget = target;
                            }
                        }
                    }

                    if (bestTarget != null)
                    {
                        bestTarget.TrySnap(mask, true);
                    }
                    else
                    {
                        Debug.Log($"[DragAll] No snap target found for '{dragging.name}' (id='{mask.tileID}')");
                    }

                    // 评估（吸附后再评估）
                    if (winController != null)
                        winController.EvaluateMaskOnTargets(mask);
                }
                else
                {
                    if (winController != null)
                        winController.CheckWinCondition();
                }
            }

            dragging = null;
        }

        // Dragging
        if (dragging != null) {
            dragging.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }
}
