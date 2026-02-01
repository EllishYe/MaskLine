using UnityEngine;

public class WinController_V : MonoBehaviour
{
    public TargetSlot[] targets;

    public bool hasWon = false; //公开给后续的UI和胜利动画
    public LineController[] line;



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
        if (!hasWon) return; 
        Debug.Log("WIN333!");
        // Animation,Sound,UI,etc.
        line[0].VictoryAnim();
        line[1].VictoryAnim();

    }
}
