using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TextMeshProUGUI AreaHeaderText;
    public TextMeshProUGUI LevelHeaderText;
    public AreaData CurrentArea;
    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();
    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonLocations = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        AssignAreaText();
        LoadUnlockedLevels();
        CreateLevelButton();
    }

    private void AssignAreaText() {
        AreaHeaderText.SetText(CurrentArea.AreaName);
    }

    private void LoadUnlockedLevels() {
        UnlockedLevelIDs.Clear();
        if (CurrentArea == null || CurrentArea.Levels == null) return;

        // 先把在 LevelData 中默认解锁的加入
        foreach (var Level in CurrentArea.Levels)
        {
            if (Level == null) continue;
            if (Level.ISUnlockedByDefault) UnlockedLevelIDs.Add(Level.LevelID);
        }

        // 再从持久化中读取解锁记录（LevelProgress）
        if (LevelProgress.Instance != null)
        {
            foreach (var level in CurrentArea.Levels)
            {
                if (level == null) continue;
                if (LevelProgress.Instance.IsUnlocked(level.LevelID))
                    UnlockedLevelIDs.Add(level.LevelID);
            }
        }
    }

    private void CreateLevelButton() {
        if (CurrentArea == null || CurrentArea.Levels == null) return;
        if (LevelButtonPrefab == null || LevelParent == null) return;

        for (int i = 0; i < CurrentArea.Levels.Count; i++) {
            var levelData = CurrentArea.Levels[i];
            if (levelData == null) continue;

            GameObject buttonGO = Instantiate(LevelButtonPrefab, LevelParent);
            _buttonObjects.Add(buttonGO);

            buttonGO.name = levelData.LevelID ?? $"Level_{i}";
            levelData.LevelButtonObj = buttonGO;

            LevelButton levelButton = buttonGO.GetComponentInChildren<LevelButton>();
            if (levelButton == null) continue;

            levelButton.Setup(levelData, UnlockedLevelIDs.Contains(levelData.LevelID));
        }
    }
}
