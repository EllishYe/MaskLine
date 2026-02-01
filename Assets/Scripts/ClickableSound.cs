using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableSound : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    public AudioClip buttonDownClip;   // clip1
    public AudioClip maskDownClip;     // clip2
    public AudioClip maskUpClip;       // clip3

    private Button button;
    private MaskID maskID;

    private void Awake()
    {
        button = GetComponent<Button>();
        maskID = GetComponent<MaskID>();
    }

    // 鼠标 / 手指 按下
    public void OnPointerDown(PointerEventData eventData)
    {
        // 先记录事件触发，便于排查 OnPointerDown 是否被调用
        Debug.Log($"[ClickableSound] OnPointerDown on '{gameObject.name}' button={(button!=null)} mask={(maskID!=null)}");

        // 情况 1：这是一个 Button
        if (button != null)
        {
            if (buttonDownClip != null)
                AudioManager.Instance?.PlaySFX(buttonDownClip);
        }

        // 情况 2：这是一个 MaskID
        if (maskID != null)
        {
            Debug.Log("[ClickableSound] Mask down detected");
            if (maskDownClip != null)
                AudioManager.Instance?.PlaySFX(maskDownClip);
        }
    }

    // 鼠标 / 手指 抬起
    public void OnPointerUp(PointerEventData eventData)
    {
        if (maskID != null)
        {
            if (maskUpClip != null)
                AudioManager.Instance?.PlaySFX(maskUpClip);
        }
    }
}
