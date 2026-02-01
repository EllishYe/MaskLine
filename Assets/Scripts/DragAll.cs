using UnityEngine;

public class DragAll : MonoBehaviour
{
    private Transform dragging = null; 
    private Vector3 offset;

    [SerializeField] private LayerMask movableLayers;

    public WinController winController;

    // 可配置：视口内边距（0-0.5），默认 5% 的内边距
    [Tooltip("视口内边距 (0..0.5)，用于避免物体中心靠边，值越大范围越小")]
    [Range(0f, 0.3f)]
    public float viewportPadding = 0.05f;


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
                    // 吸附逻辑（略，此处保留原有实现）
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

        // Dragging with viewport clamp
        if (dragging != null) {
            Vector3 targetWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            // preserve original z
            targetWorld.z = dragging.position.z;

            // convert to viewport and clamp with padding
            Vector3 vp = Camera.main.WorldToViewportPoint(targetWorld);
            vp.x = Mathf.Clamp(vp.x, viewportPadding, 1f - viewportPadding);
            vp.y = Mathf.Clamp(vp.y, viewportPadding, 1f - viewportPadding);

            Vector3 clampedWorld = Camera.main.ViewportToWorldPoint(vp);
            clampedWorld.z = dragging.position.z;

            dragging.position = clampedWorld;
        }
    }
}
