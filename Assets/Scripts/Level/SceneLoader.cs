using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Tooltip("把场景从 Project 窗口拖到这里")]
    public SceneField scene;

    [Tooltip("按下后要播放的音效（可留空）")]
    public AudioClip playBeforeLoadClip;

    // 按钮 OnClick 指向此方法（同步加载，先播放音效）
    public void LoadScene()
    {
        string name = scene.SceneName;
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogWarning("[SceneLoader] scene is not set on " + gameObject.name);
            return;
        }

        if (playBeforeLoadClip != null && AudioManager.Instance != null)
        {
            // 播放并在回调里加载场景
            AudioManager.Instance.PlaySFX(playBeforeLoadClip, () =>
            {
                SceneManager.LoadScene(name);
            });
        }
        else
        {
            // 无音效或 AudioManager，不等待，直接加载
            SceneManager.LoadScene(name);
        }
    }

    // 可选异步版本（同样等待音效）
    public void LoadSceneAsync()
    {
        string name = scene.SceneName;
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogWarning("[SceneLoader] scene is not set on " + gameObject.name);
            return;
        }

        if (playBeforeLoadClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(playBeforeLoadClip, () =>
            {
                SceneManager.LoadSceneAsync(name);
            });
        }
        else
        {
            SceneManager.LoadSceneAsync(name);
        }
    }
}
