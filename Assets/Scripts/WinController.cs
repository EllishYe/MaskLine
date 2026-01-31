using UnityEngine;

public class WinController : MonoBehaviour
{
    public TargetSlot[] targets;

    private bool hasWon = false;

    // 供DragAll在拖拽结束时调用
    public void EvaluateMaskOnTargets(MaskID mask)
    {
        if (mask == null) {
            return;
        }

        foreach (var target in targets)
        {
            if (target == null)
            {
                continue;
            }
            target.CheckMask(mask);
        }

        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        if (hasWon) return;

        foreach (var target in targets)
        {
            if (target == null || !target.IsSatisfied)
            {
                Debug.Log("[WinController] Not all targets satisfied yet.");
                return;
            }
        }

        hasWon = true;
        OnWin();
    }

    private void OnWin()
    {
        Debug.Log("WIN!");
        // Animation,Sound,UI,etc.
    }
}
