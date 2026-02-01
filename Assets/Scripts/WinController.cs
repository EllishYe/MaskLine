using UnityEngine;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    public TargetSlot[] targets;

    private bool hasWon = false;

    [Tooltip("在当前关卡胜利后要解锁的 LevelID（可留空）")]
    public string unlockLevelID;

    [Tooltip("胜利时播放的音效（可留空）")]
    public AudioClip winClip;

    [Tooltip("胜利后要跳转到的场景（可拖入 Scene Asset）")]
    public SceneField returnScene;

    // 供DragAll在拖拽结束时调用
    public void EvaluateMaskOnTargets(MaskID mask)
    {
        if (mask == null)
        {
            Debug.LogWarning("[WinController] EvaluateMaskOnTargets called with null mask");
            return;
        }

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

        // 先解锁下一个关卡（立即持久化）
        if (!string.IsNullOrWhiteSpace(unlockLevelID) && LevelProgress.Instance != null)
        {
            LevelProgress.Instance.UnlockLevel(unlockLevelID);
        }

        // 再播放胜利音，播放完毕后跳转
        string sceneName = returnScene != null ? returnScene.SceneName : null;

        if (winClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(winClip, () =>
            {
                if (!string.IsNullOrWhiteSpace(sceneName))
                    SceneManager.LoadScene(sceneName);
            });
        }
        else
        {
            // 若没配置音或AudioManager不存在则直接跳转
            if (!string.IsNullOrWhiteSpace(sceneName))
                SceneManager.LoadScene(sceneName);
        }

        // 你也可以在这里播放胜利 UI/特效
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
