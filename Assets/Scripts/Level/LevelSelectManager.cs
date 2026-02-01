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
    //private Camera camera;
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
        foreach (var Level in CurrentArea.Levels)
        {
            if (Level.ISUnlockedByDefault) {
                UnlockedLevelIDs.Add(Level.LevelID);
            }
        }

    }

    private void CreateLevelButton() {
        for (int i = 0; i < CurrentArea.Levels.Count; i++) {
            GameObject buttonGO = Instantiate(LevelButtonPrefab, LevelParent);
            _buttonObjects.Add(buttonGO);
            RectTransform buttonRect = buttonGO.GetComponent <RectTransform> ();

            buttonGO.name = CurrentArea.Levels[i].LevelID;
            CurrentArea.Levels[i].LevelButtonObj = buttonGO;

            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();
            levelButton.Setup(CurrentArea.Levels[i], UnlockedLevelIDs.Contains(CurrentArea.Levels[i].LevelID));

        }
    }
}
