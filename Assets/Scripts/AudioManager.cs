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
