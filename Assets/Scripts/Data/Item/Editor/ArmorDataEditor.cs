using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArmorData))]
public class ArmorDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ArmorData data = target as ArmorData;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Calculate Percentage", GUILayout.Width(160), GUILayout.Height(32)))
        {
            //Base On Time
            ArmorData.ArmorStat armorStat = data.Stat;
            armorStat.RechargeRate = armorStat.ArmorPoint / armorStat.RechargeTime;
            armorStat.RechargePercentage = armorStat.RechargeRate / armorStat.ArmorPoint * 100;
            data.Stat = armorStat;
        }
        if (GUILayout.Button("Calculate Time", GUILayout.Width(160), GUILayout.Height(32)))
        {
            //Base On Percentage
            ArmorData.ArmorStat armorStat = data.Stat;
            armorStat.RechargeTime = 100 / armorStat.RechargePercentage;
            armorStat.RechargeRate = armorStat.ArmorPoint / armorStat.RechargeTime;
            data.Stat = armorStat;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
}
