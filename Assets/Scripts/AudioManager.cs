using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 如果 Inspector 未指派 sfxSource，尝试自动获取或创建
        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
            if (sfxSource == null)
            {
                // 创建一个子对象来承载 AudioSource，避免污染根
                var go = new GameObject("SFX_Source");
                go.transform.SetParent(transform);
                sfxSource = go.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f; // 2D sound
                Debug.LogWarning("[AudioManager] sfxSource was null; created fallback AudioSource.");
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // 新增：播放音效并在播放结束后回调
    public void PlaySFX(AudioClip clip, Action onComplete)
    {
        if (clip == null || sfxSource == null)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(PlaySfxCoroutine(clip, onComplete));
    }

    private IEnumerator PlaySfxCoroutine(AudioClip clip, Action onComplete)
    {
        sfxSource.PlayOneShot(clip);
        yield return new WaitForSecondsRealtime(Mathf.Max(0.01f, clip.length));
        onComplete?.Invoke();
    }
}
