using UnityEngine;

public class TargetSlot : MonoBehaviour
{
    [Header("MaskID Requirement")]
    public string requiredMaskID;

    [Header("Radius Acceptance")]
    public float acceptRadius = 0.25f;

    [Header("Runtime State")]
    public bool IsSatisfied { get; private set; }

    // record the Mask currently occupying this Target
    private MaskID occupyingMask;

    /// <summary>
    /// 对外接口：
    /// 由 WinManager 或 Drag 系统在「拖拽结束时」调用
    /// 用于检测当前是否有正确 Mask 对位到这个 Target
    /// </summary>
    public void CheckMask(MaskID mask)
    {
        // if null mask is passed, do nothing
        if (mask == null)
        {
            Debug.Log($"[TargetSlot] {name}: CheckMask called with null -> keep IsSatisfied={IsSatisfied}");
            return;
        }

        // if ID is not matched, early return
        if (mask.tileID != requiredMaskID)
        {
            // 只有当正在评估的 mask 恰好是当前占位的 mask 时，才清除（说明占位的 mask 被移开或放错地方）
            if (occupyingMask == mask)
            {
                IsSatisfied = false;
                occupyingMask = null;
            }
            return;
        }

        // ID is matched, check distance
        float distance = Vector2.Distance(mask.transform.position, transform.position);
        Debug.Log($"[TargetSlot] {name}: CheckMask -> matched ID='{mask.tileID}'. distance={distance:F3}, acceptRadius={acceptRadius:F3} (mask object: {mask.gameObject.name})");

        bool satisfied = distance <= acceptRadius;

        if (satisfied)
        {
            IsSatisfied = true;
            occupyingMask = mask;
            Debug.Log($"[TargetSlot] {name}: CheckMask -> alignment SUCCESS. IsSatisfied={IsSatisfied}");
        }
        else
        {
            // 如果当前占位的是这个 mask，但现在不在范围内，则清除；否则不改变
            if (occupyingMask == mask)
            {
                IsSatisfied = false;
                occupyingMask = null;
            }
        }
    }

    /// <summary>
    /// 仅用于 Scene 视图中的调试显示
    /// 可视化当前 Target 的判定范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, acceptRadius);
    }
}
