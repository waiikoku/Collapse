using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCoordinate : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private RectTransform centerRT;
    //[SerializeField] private Transform[] targets;
    //[SerializeField] private Vector3[] directions;
    //[SerializeField] private RectTransform[] targetRT;

    [SerializeField] private Vector2 coordinateRatio = new Vector2(1, 10);
    [SerializeField] private float multipier = 1f;
    public float radius = 1f;
    public Camera orthoCam;
    private float squareRadius;
    public Transform[] debugWorldObjs;
    public RectTransform debugVisualRadius;
    [Header("UI")]
    public GameObject iconPrefab;
    public Transform container;
    public List<RectTransform> targetRT;
    public List<Transform> targets;
    public List<Vector3> directions;

    private Vector3 targetForward;
    private Vector3 targetRight;
    private Vector3 euan;
    private float centerY;

    private void Start()
    {
        if (centerRT == null)
        {
            centerRT = Instantiate(iconPrefab, container).GetComponent<RectTransform>();
        }
        foreach (var obj in debugWorldObjs)
        {
            AddTarget(obj);
        }
    }

    private void FixedUpdate()
    {
        euan = center.eulerAngles;
        centerY = -euan.y;
        /*
        targetForward = center.forward;
        targetRight = center.right;
        targetForward.y = 0;
        targetRight.y = 0;
        targetForward.Normalize();
        targetRight.Normalize();
        */
    }

    private Vector3 GetIntend(Vector3 direction)
    {
        return (targetForward * direction.z) + (targetRight * direction.x);
    }

    private void LateUpdate()
    {
        float os = radius;
        squareRadius = os * os;
        debugVisualRadius.sizeDelta = os * 10 * Vector2.one;

        for (int i = 0; i < targets.Count; i++)
        {
            directions[i] = Get2DPosition(targets[i].position);
            bool outOfRange = directions[i].sqrMagnitude > squareRadius;
            targetRT[i].gameObject.SetActive(!outOfRange);
            if (outOfRange)
            {
                continue;
            }
            directions[i] = Quaternion.Euler(new Vector3(0, centerY, 0)) * directions[i];
            Vector2 WorldToPlane = new Vector2(directions[i].x, directions[i].z);
            targetRT[i].anchoredPosition = WorldToPlane * multipier;
        }
    }

    public void AddTarget(Transform target)
    {
        int index = targets.Count - 1;
        if(index < 0)
        {
            index = 0;
        }
        targets.Add(target);
        Vector3 pos = target.position;
        directions.Add(pos - center.position);
        RectTransform rt = Instantiate(iconPrefab, container).GetComponent<RectTransform>();
        targetRT.Add(rt);
        directions[index] = Get2DPosition(pos);
        directions[index] = Quaternion.Euler(new Vector3(0, -centerY, 0)) * directions[index];
        Vector2 WorldToPlane = new Vector2(directions[index].x, directions[index].z);
        targetRT[index].anchoredPosition = WorldToPlane * multipier;
    }

    private Vector3 Get2DPosition(Vector3 position)
    {
        Vector3 tempDir = position - center.position;
        tempDir.y = 0;
        return tempDir;
    }
}
