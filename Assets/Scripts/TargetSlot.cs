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
        if (mask == null)
        {
            return;
        }

        // 如果 tileID 不匹配
        if (mask.tileID != requiredMaskID)
        {
            // 只有当当前占位恰好是这个 mask 时，才清除（表示该 mask 被移开或放错）
            if (occupyingMask == mask)
            {
                IsSatisfied = false;
                occupyingMask = null;
                Debug.Log($"[TargetSlot] {name}: occupancy cleared for '{mask.gameObject.name}'");
            }
            return;
        }

        // ID is matching,check distance
        float distance = Vector2.Distance(mask.transform.position, transform.position);
        bool satisfied = distance <= acceptRadius;

        if (satisfied)
        {
            IsSatisfied = true;
            occupyingMask = mask;
            Debug.Log($"[TargetSlot] {name}: alignment SUCCESS with '{mask.gameObject.name}'");
        }
        else
        {
            // 如果当前占位的是这个 mask，但现在不在范围内，则清除
            if (occupyingMask == mask)
            {
                IsSatisfied = false;
                occupyingMask = null;
                Debug.Log($"[TargetSlot] {name}: occupying '{mask.gameObject.name}' moved out of range, cleared");
            }
        }
    }

    /// <summary>
    /// 如果当前占位正是传入的 mask，则清除占位与状态。
    /// 拾取（OnMouseDown）时应调用此函数以立即释放占位。
    /// </summary>
    public void ClearOccupyingIfMatches(MaskID mask)
    {
        if (mask == null) return;
        if (occupyingMask == mask)
        {
            occupyingMask = null;
            IsSatisfied = false;
            Debug.Log($"[TargetSlot] {name}: ClearOccupyingIfMatches -> cleared occupancy for '{mask.gameObject.name}'");
        }
    }

    /// <summary>
    /// 尝试让这个 target 接受并（可选）吸附传入的 mask。
    /// 返回 true 表示接受并设置占位（并在 snap=true 时把 mask 的 transform 设为 target 位置）。
    /// </summary>
    public bool TrySnap(MaskID mask, bool snapTransform = true)
    {
        if (mask == null) return false;
        if (mask.tileID != requiredMaskID) return false;

        float distance = Vector2.Distance(mask.transform.position, transform.position);
        if (distance <= acceptRadius)
        {
            occupyingMask = mask;
            IsSatisfied = true;
            if (snapTransform)
            {
                mask.transform.position = transform.position;
            }
            Debug.Log($"[TargetSlot] {name}: TrySnap -> snapped '{mask.gameObject.name}'");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 重置此 target 的占位与满足状态（Restart 时调用）
    /// </summary>
    public void ResetSlot()
    {
        occupyingMask = null;
        IsSatisfied = false;
    }

    /// <summary>
    /// Scene 视图可视化判定范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, acceptRadius);
    }
}
