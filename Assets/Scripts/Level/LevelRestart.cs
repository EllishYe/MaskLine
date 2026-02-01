using System.Collections.Generic;
using UnityEngine;

public class LevelRestart : MonoBehaviour
{
    [Tooltip("场景中的 WinController (用于重置目标状态)")]
    public WinController winController;

    // 所有场景中可拖拽的 Mask 列表与其初始世界位置
    private MaskID[] _masks;
    private Dictionary<MaskID, Vector3> _initialPositions = new Dictionary<MaskID, Vector3>();

    void Start()
    {
        CacheInitialMaskPositions();
    }

    // 在 Start 时缓存所有 Mask 的初始世界位置
    public void CacheInitialMaskPositions()
    {
        _initialPositions.Clear();
        _masks = FindObjectsByType<MaskID>(FindObjectsSortMode.None);
        foreach (var m in _masks)
        {
            if (m == null) continue;
            _initialPositions[m] = m.transform.position;
        }
    }

    // 供 Button 绑定：恢复所有 Mask 的位置并重置目标/胜利状态
    public void RestartLevel()
    {
        // 恢复位置
        foreach (var kv in _initialPositions)
        {
            var mask = kv.Key;
            var pos = kv.Value;
            if (mask == null) continue;

            mask.transform.position = pos;
        }

        // 重置目标占位与胜利状态
        if (winController != null)
        {
            winController.ResetState();
        }
        else
        {
            Debug.LogWarning("[LevelRestartManager] winController not assigned");
        }
    }
}
