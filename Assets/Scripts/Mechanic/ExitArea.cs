using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitArea : MonoBehaviour
{
    [SerializeField] private Waypoint profile;
    [SerializeField] private Transform target;
    private bool destroyed = false;
    private void Start()
    {
        WaypointManager.Instance.AddWaypoint(target, profile);
        LevelManager.Instance.beginCallback += Exit;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.beginCallback -= Exit;
    }

    public void Exit()
    {
        print("Remove Waypoint!");
        try
        {
            WaypointManager.Instance.RemoveWaypoint(target);
            destroyed = true;
        }
        catch (System.Exception e)
        {
            print(e.Message);
            throw;
        }

    }
}
