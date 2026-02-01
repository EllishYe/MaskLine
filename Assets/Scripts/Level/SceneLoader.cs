using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public SceneField scene;

    // 按钮 OnClick 指向此方法（同步加载）
    public void LoadScene()
    {
        string name = scene.SceneName;
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogWarning("[SceneLoader] scene is not set on " + gameObject.name);
            return;
        }

        SceneManager.LoadScene(name);
    }
}
