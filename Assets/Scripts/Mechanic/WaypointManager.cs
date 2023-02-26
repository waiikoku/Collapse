using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : Singleton<WaypointManager>
{
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform waypointPrefab;
    public List<Transform> targets;
    private List<Waypoint> waypoints;
    private List<WaypointUI> waypointUIs;
    private List<RectTransform> targetUI;
    private List<UnityEngine.UI.Image> targetIcon;
    private Transform player;
    private Camera cam;

    protected override void Awake()
    {
        base.Awake();
        targets = new List<Transform>();
        waypoints = new List<Waypoint>();
        waypointUIs = new List<WaypointUI>();
        targetUI = new List<RectTransform>();
        targetIcon = new List<UnityEngine.UI.Image>();
    }

    private void Start()
    {
        GameManager.Instance.OnLoadPlayer += LoadCameraa;
    }

    private void LoadCameraa(PlayerAgent player,CameraManager camera)
    {
        cam = camera.myCamera;
        this.player = player.transform;
    }
    private void LateUpdate()
    {
        if (cam == null) return;
        for (int i = 0; i < targets.Count; i++)
        {
            TrackWaypoint(i);
        }
    }

    public void AddWaypoint(Transform target,Waypoint data = null)
    {
        targets.Add(target);
        RectTransform rt = Instantiate(waypointPrefab, container);
        targetUI.Add(rt);
        UnityEngine.UI.Image image = rt.GetComponent<UnityEngine.UI.Image>();
        targetIcon.Add(image);
        if (data != null)
        {
            waypoints.Add(data);
            waypointUIs.Add(rt.GetComponent<WaypointUI>());
            if(data.waypointIcon != null)
            {
                image.sprite = data.waypointIcon;
            }
        }
    }

    public void AddWaypoint(GameObject prefab,Transform target,Waypoint data = null)
    {

    }

    public void RemoveWaypoint(Transform target)
    {
        int indexOfTarget = targets.FindIndex(t => target == t);
        if (indexOfTarget == -1) return;
        try
        {
            GameObject go = targetUI[indexOfTarget].gameObject;
            targets.RemoveAt(indexOfTarget);
            targetUI.RemoveAt(indexOfTarget);
            targetIcon.RemoveAt(indexOfTarget);
            waypoints.RemoveAt(indexOfTarget);
            waypointUIs.RemoveAt(indexOfTarget);
            Destroy(go, 0.1f);
        }
        catch (System.Exception e)
        {
            print($"{e.Message} | {target.name} | {indexOfTarget}");
            throw;
        }
    }

    private void UpdateIcon(int id)
    {
        targetIcon[id].sprite = waypoints[id].waypointIcon;
    }

    private void TrackWaypoint(int id)
    {
        if (player == null) return;
        if (targets[id] == null) return;
        Vector3 pos = targets[id].position;
        float distance = Mathf.Sqrt((pos - player.position).sqrMagnitude);
        if (waypoints[id].displayMeter)
        {
            waypointUIs[id].Activate(true);
            waypointUIs[id].UpdateDistance(distance);
        }
        Vector3 screenPoint = cam.WorldToViewportPoint(pos + waypoints[id].offset);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        targetUI[id].gameObject.SetActive(onScreen);
        if (onScreen)
        {
            targetUI[id].anchoredPosition = cam.ViewportToScreenPoint(screenPoint);
        }
    }

}
