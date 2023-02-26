using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonTextWithFrame))]
public class ButtonTextWithFrameEditor : Editor
{
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ButtonTextWithFrame btwf = target as ButtonTextWithFrame;
        if(GUILayout.Button("Get Components"))
        {
            btwf.Setup();
        }
    }
#endif
}
