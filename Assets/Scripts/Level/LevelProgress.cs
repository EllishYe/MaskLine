using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    private const string PlayerPrefsKey = "UnlockedLevels";
    public static LevelProgress Instance { get; private set; }

    private HashSet<string> _unlocked = new HashSet<string>(StringComparer.Ordinal);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    private void Load()
    {
        _unlocked.Clear();
        string csv = PlayerPrefs.GetString(PlayerPrefsKey, "");
        if (!string.IsNullOrEmpty(csv))
        {
            var parts = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts) _unlocked.Add(p.Trim());
        }
    }

    private void Save()
    {
        var csv = string.Join(",", _unlocked.OrderBy(s => s));
        PlayerPrefs.SetString(PlayerPrefsKey, csv);
        PlayerPrefs.Save();
    }

    public bool IsUnlocked(string levelID)
    {
        if (string.IsNullOrWhiteSpace(levelID)) return false;
        return _unlocked.Contains(levelID);
    }

    public void UnlockLevel(string levelID)
    {
        if (string.IsNullOrWhiteSpace(levelID)) return;
        if (_unlocked.Add(levelID))
        {
            Save();
            Debug.Log($"[LevelProgress] Unlocked level '{levelID}'");
        }
    }

    // 可选：解锁并返回是否新解锁
    public bool UnlockLevelIfNeeded(string levelID)
    {
        if (string.IsNullOrWhiteSpace(levelID)) return false;
        if (!_unlocked.Contains(levelID))
        {
            _unlocked.Add(levelID);
            Save();
            Debug.Log($"[LevelProgress] Unlocked level '{levelID}'");
            return true;
        }
        return false;
    }
}
