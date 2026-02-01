using UnityEngine;

public class WinController : MonoBehaviour
{
    public TargetSlot[] targets;

    private bool hasWon = false;

    // 可在 Inspector 中填写（胜利后解锁的下一个 LevelID）
    [Tooltip("在当前关卡胜利后要解锁的 LevelID（可留空）")]
    public string unlockLevelID;

    // 供DragAll在拖拽结束时调用
    public void EvaluateMaskOnTargets(MaskID mask)
    {
        if (mask == null)
        {
            Debug.LogWarning("[WinController] EvaluateMaskOnTargets called with null mask");
            return;
        }

        // 仅调用各 target 的检查，不做冗余日志输出（TargetSlot 内保留必要日志）
        if (targets != null)
        {
            foreach (var target in targets)
            {
                if (target == null) continue;
                target.CheckMask(mask);
            }
        }

        CheckWinCondition();
    }

    /// <summary>
    /// 遍历所有 target，若某个 target 的 occupyingMask 恰好是传入的 mask，则让该 target 清除占位。
    /// 在拾起 mask（开始拖拽）时调用。
    /// </summary>
    public void ClearOccupancyForMask(MaskID mask)
    {
        if (mask == null) return;
        if (targets == null) return;

        foreach (var target in targets)
        {
            if (target == null) continue;
            target.ClearOccupyingIfMatches(mask);
        }
    }

    public void CheckWinCondition()
    {
        if (hasWon) return;

        if (targets == null) return;

        foreach (var target in targets)
        {
            if (target == null || !target.IsSatisfied)
            {
                // 不再输出每次未完成的冗余日志，保留沉默检查
                return;
            }
        }

        hasWon = true;
        OnWin();
    }

    private void OnWin()
    {
        Debug.Log("WIN!");
        // 解锁下一个关卡（如果设置了）
        if (!string.IsNullOrWhiteSpace(unlockLevelID) && LevelProgress.Instance != null)
        {
            LevelProgress.Instance.UnlockLevel(unlockLevelID);
        }
        // Animation,Sound,UI,etc.
    }

    /// <summary>
    /// 在 Restart 场景时调用：重置 hasWon 并清除所有 target 的占位/状态 
    /// </summary>
    public void ResetState()
    {
        hasWon = false;
        if (targets == null) return;
        foreach (var t in targets)
        {
            if (t == null) continue;
            t.ResetSlot();
        }
    }
}
