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
    bool lineSpawnStart = false;
    bool lineSpawnEnd = false;
    [Header("StartPoint&EndPoint")]
    public GameObject startPrefab;
    public SpriteMask startPointMask;
    bool startPointSpawned;
    public GameObject endPrefab;
    public SpriteMask endPointMask;
    bool endPointSpawned;
    bool win;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        StartCoroutine(MoveMask(startPointMask, lineSlot[0].position, startPointSpawned));//播放开始点动画
        //当胜利时，先在线条的起始位置生成一个点，留在原地，
        line.positionCount = 1;
        line.SetPosition(0, lineSlot[0].position);
        currentIndex = 1;
        StartCoroutine(DrawLineStepByStep());
    }


    IEnumerator MoveMask(SpriteMask mask, Vector3 targetPos, bool conditon)
    {
        var target = targetPos;

        while (Vector3.Distance(mask.transform.position, target) > 0.01f)
        {
            mask.transform.position = Vector3.Lerp(mask.transform.position, target, 10f * Time.deltaTime);
            yield return null;
        }

        mask.transform.position = target;
        conditon = true;

    }
    IEnumerator DrawLineStepByStep()
    {
        //在在起始位置生成一个点，往下一个lineSlot移动，当到达位置后，在新的位置生成一个新的点，直到达到终点。
        while (currentIndex < lineSlot.Length)
        {
            line.positionCount++;
            int pointIndex = line.positionCount - 1;

            Vector3 startPos = lineSlot[currentIndex-1].position;//上一点的位置
            Vector3 targetPos = lineSlot[currentIndex].position;//目标位置

            float t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime * moveSpeed;
                line.SetPosition(pointIndex,Vector3.Lerp(startPos, targetPos, t));
                //line.SetPosition(pointIndex, Vector3.SmoothDamp(startPos, targetPos,ref moveVel, 1f));
                yield return null;
            }

            line.SetPosition(pointIndex, targetPos);

            currentIndex++;
        }
        //lineSpawnEnd = true;
        StartCoroutine(MoveMask(endPointMask, lineSlot[lineSlot.Length-1].position,endPointSpawned));//播放结束点动画
    }

    // Update is called once per frame
    void Update()
    {
        //if (lineSpawnEnd && !endPointSpawned) 
        //{
        //    Instantiate(endPrefab, lineSlot[lineSlot.Length - 1].position, Quaternion.identity);
        //    endPointSpawned = true;
        //}
        
    }
}
