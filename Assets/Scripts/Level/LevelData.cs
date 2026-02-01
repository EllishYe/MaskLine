using UnityEngine;

[CreateAssetMenu(menuName = "Leve Data/Levels", fileName = "New Level")]
public class LevelData : ScriptableObject
{
    [Header("Level Stats")]
    public string LevelID;

    [Tooltip("For Starting Levels")] public bool ISUnlockedByDefault;
    public SceneField Scene;

    [Header("Level Display Information")]
    public string LevelName;

    public GameObject LevelButtonObj { get; set; }

}
