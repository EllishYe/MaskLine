using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableSound : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    public AudioClip buttonDownClip;
    public AudioClip maskDownClip;
    public AudioClip maskUpClip;

    private Button button;
    private MaskID maskID;

    private void Awake()
    {
        button = GetComponent<Button>();
        maskID = GetComponent<MaskID>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[ClickableSound] OnPointerDown on '{gameObject.name}' button={(button!=null)} mask={(maskID!=null)}");

        bool hasAudioMgr = AudioManager.Instance != null;
        var sfx = AudioManager.Instance != null ? AudioManager.Instance.sfxSource : null;
        Debug.Log($"[ClickableSound] AudioManager.Instance={(hasAudioMgr)} sfxSource={(sfx!=null)} buttonDownClip={(buttonDownClip!=null)} maskDownClip={(maskDownClip!=null)}");

        if (button != null && buttonDownClip != null)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
                AudioManager.Instance.PlaySFX(buttonDownClip);
            else
                Debug.LogWarning("[ClickableSound] Cannot play buttonDownClip: AudioManager or sfxSource is missing.");
        }

        if (maskID != null && maskDownClip != null)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
                AudioManager.Instance.PlaySFX(maskDownClip);
            else
                Debug.LogWarning("[ClickableSound] Cannot play maskDownClip: AudioManager or sfxSource is missing.");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (maskID != null && maskUpClip != null)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
                AudioManager.Instance.PlaySFX(maskUpClip);
            else
                Debug.LogWarning("[ClickableSound] Cannot play maskUpClip: AudioManager or sfxSource is missing.");
        }
    }
}
