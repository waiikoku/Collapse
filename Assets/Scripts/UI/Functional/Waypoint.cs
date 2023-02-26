using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    [Header("Info")]
    public int waypointID;
    public string waypointName;
    public Sprite waypointIcon;
    public Vector3 offset;
    public bool displayMeter = false;
}
