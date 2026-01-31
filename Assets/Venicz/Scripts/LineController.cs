using System.Collections;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public LineRenderer line;
    public GameObject StartPrefab;
    public GameObject EndPrefab;
    public Transform[] lineSlot;
    public int pointNums;
    public Transform[] targetPos;
    public float moveSpeed = 5f;
    Vector3 moveVel;
    int currentIndex = 0;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        //当胜利时，先在线条的起始位置生成一个点，留在原地，
        line.positionCount = 1;
        line.SetPosition(0, lineSlot[0].position);
        currentIndex = 1;

        StartCoroutine(DrawLineStepByStep());
    }

    IEnumerator DrawLineStepByStep()
    {
        //在在起始位置生成一个点，往下一个lineSlot移动，当到达位置后，在新的位置生成一个新的点，直到达到终点。
        while (currentIndex < lineSlot.Length)
        {
            line.positionCount = currentIndex;
            int pointIndex = line.positionCount - 1;

            Vector3 startPos = lineSlot[currentIndex-1].position;//上一点的位置
            Vector3 targetPos = lineSlot[currentIndex].position;//目标位置

            line.SetPosition(pointIndex, startPos);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime*moveSpeed;
                line.SetPosition(pointIndex,Vector3.Lerp(startPos, targetPos, t));
                //line.SetPosition(pointIndex, Vector3.SmoothDamp(startPos, targetPos,ref moveVel, 1f));
                yield return null;
            }

            currentIndex++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
