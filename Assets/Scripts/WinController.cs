using System.Collections;
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

    [Tooltip("胜利时需要播放的若干 LineController（可在 Inspector 中列出多个）")]
    public LineController[] lineControllers;

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
        StartCoroutine(HandleWinSequence());
    }

    private IEnumerator HandleWinSequence()
    {
        Debug.Log("WIN!");

        // 先解锁下一个关卡（立即持久化）
        if (!string.IsNullOrWhiteSpace(unlockLevelID) && LevelProgress.Instance != null)
        {
            LevelProgress.Instance.UnlockLevel(unlockLevelID);
        }

        string sceneName = returnScene != null ? returnScene.SceneName : null;

        // 1) 并行触发所有 LineController 的动画并等待它们全部完成
        if (lineControllers != null && lineControllers.Length > 0)
        {
            int n = lineControllers.Length;
            bool[] finished = new bool[n];

            for (int i = 0; i < n; i++)
            {
                var lc = lineControllers[i];
                if (lc == null)
                {
                    finished[i] = true;
                    continue;
                }
                StartCoroutine(RunLineControllerAndFlag(lc, i, finished));
            }

            // 等待所有 finished 都为 true
            bool allDone = false;
            while (!allDone)
            {
                allDone = true;
                for (int i = 0; i < n; i++)
                {
                    if (!finished[i])
                    {
                        allDone = false;
                        break;
                    }
                }
                if (!allDone) yield return null;
            }
        }

        // 2) 播放胜利音并等待其播放结束
        if (winClip != null && AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
        {
            AudioManager.Instance.PlaySFX(winClip);
            yield return new WaitForSecondsRealtime(Mathf.Max(0.01f, winClip.length));
        }

        // 3) 跳转场景（如果配置了）
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator RunLineControllerAndFlag(LineController lc, int index, bool[] finished)
    {
        if (lc == null)
        {
            finished[index] = true;
            yield break;
        }

        // 启动并等待该 LineController 的协程完成
        yield return StartCoroutine(lc.PlayVictoryCoroutine());
        finished[index] = true;
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
