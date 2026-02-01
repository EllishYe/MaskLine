using System.Collections;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [Header("Line")]
    public LineRenderer line;
    public Transform[] lineSlot;
    public int pointNums;
    public Transform[] targetPos;
    public float moveSpeed = 5f;
    int currentIndex = 0;

    [Header("StartPoint&EndPoint")]
    public GameObject startPrefab;
    public SpriteMask startPointMask;
    bool startPointSpawned;
    public GameObject endPrefab;
    public SpriteMask endPointMask;
    bool endPointSpawned;
    bool win;

    [Header("LineLevel")]
    public bool main;
    public LineController extraLine;
    public Transform secondLineStartPoint;

    public float startPointOffset;

    [Tooltip("是否在场景 Start 时自动播放（通常禁用，由 WinController 触发）")]
    public bool autoPlayOnStart = false;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        if (main && autoPlayOnStart)
        {
            StartCoroutine(PlayVictoryCoroutine());
        }
    }

    public void VictoryAnim()
    {
        StartCoroutine(PlayVictoryCoroutine());
    }

    public IEnumerator PlayVictoryCoroutine()
    {
        line.positionCount = 1;
        if (lineSlot != null && lineSlot.Length > 0)
            line.SetPosition(0, lineSlot[0].position);
        currentIndex = 1;

        if (startPointMask != null && lineSlot != null && lineSlot.Length > 0)
        {
            yield return StartCoroutine(MoveMask(startPointMask, lineSlot[0].position));
            startPointSpawned = true;
        }

        yield return StartCoroutine(DrawLineStepByStepCoroutine());

        if (endPointMask != null && lineSlot != null && lineSlot.Length > 0)
        {
            yield return StartCoroutine(MoveMask(endPointMask, lineSlot[lineSlot.Length - 1].position));
            endPointSpawned = true;
        }
    }

    private IEnumerator DrawLineStepByStepCoroutine()
    {
        while (currentIndex < lineSlot.Length)
        {
            Vector3 startPos = lineSlot[currentIndex - 1].position;
            Vector3 targetPos = lineSlot[currentIndex].position;

            var dir = targetPos - startPos;
            Vector3 newStartPos = startPos;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x < 0) newStartPos.x -= startPointOffset;
                else newStartPos.x += startPointOffset;
            }
            else
            {
                if (dir.y < 0) newStartPos.y -= startPointOffset;
                else newStartPos.y += startPointOffset;
            }

            line.positionCount++;
            line.SetPosition(line.positionCount - 1, newStartPos);
            int pointIndex = line.positionCount - 1;
            float t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime * moveSpeed;
                line.SetPosition(pointIndex, Vector3.Lerp(newStartPos, targetPos, t));
                yield return null;
            }

            line.SetPosition(pointIndex, targetPos);

            if (main && secondLineStartPoint != null)
            {
                if (line.GetPosition(pointIndex) == secondLineStartPoint.position)
                {
                    if (extraLine != null)
                    {
                        extraLine.VictoryAnim();
                    }
                }
            }
            currentIndex++;
        }
    }

    private IEnumerator MoveMask(SpriteMask mask, Vector3 targetPos)
    {
        if (mask == null) yield break;

        while (Vector3.Distance(mask.transform.position, targetPos) > 0.01f)
        {
            mask.transform.position = Vector3.Lerp(mask.transform.position, targetPos, 10f * Time.deltaTime);
            yield return null;
        }

        mask.transform.position = targetPos;
    }

    // 新增：停止任何正在运行的动画并重置 LineRenderer 状态（供 Reset 使用）
    public void ResetAnimation()
    {
        StopAllCoroutines();
        currentIndex = 0;
        if (line != null)
        {
            line.positionCount = 0;
        }
        startPointSpawned = false;
        endPointSpawned = false;
    }
}
