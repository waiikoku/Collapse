using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class MobWindow : EditorWindow
{
    private const string defaultPath = "Enemies";
    private Vector2 scrollPos;
    private int selectPanel;
    private Editor mobEditor;
    [MenuItem("Window/Mob")]
    public static void ShowWindow()
    {
        GetWindow<MobWindow>("Mob");
    }

    private void OnGUI()
    {
        string[] opts = new string[] { "AA", "BB", "CC"};
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        selectPanel = GUILayout.SelectionGrid(selectPanel, opts, 3,GUILayout.Width(160),GUILayout.Height(32));
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if(selectPanel > -1)
        {
            //Debug.Log("Selected: " + selectPanel + " , " + opts[selectPanel]);
            if (selectPanel == 0)
            {
                Panel1();
            }else if(selectPanel == 1)
            {
                TestBox();
            }
        }
        //Debug.Log("Rendering!");
    }
    private int startIndex;

    [System.Obsolete]
    private void Panel1()
    {
        MobData[] ResMD = Resources.LoadAll<MobData>(defaultPath);
        int stIndex = startIndex + ResMD.Length;
        GUIStyle headStyle = new GUIStyle();
        headStyle.fontSize = 36;
        headStyle.fontStyle = FontStyle.Bold;
        headStyle.normal.textColor = Color.white;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Mob Asset", headStyle);
        GUILayout.BeginVertical();
        if (GUILayout.Button("Add"))
        {
            MobData newMob = ScriptableObject.CreateInstance<MobData>();
            newMob.objName = "Mob" + stIndex;
            newMob.MobID = stIndex;
            newMob.MobName = $"Mob {stIndex}";
            AssetDatabase.CreateAsset(newMob,$"Assets/Resources/{defaultPath}/Mob{stIndex}.asset");
        }
        if (GUILayout.Button("Add"))
        {
            MobData newMob = ScriptableObject.CreateInstance<MobData>();
            AssetDatabase.CreateAsset(newMob, $"Assets/Resources/{defaultPath}/Mob{stIndex}.asset");
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        startIndex = int.Parse(GUILayout.TextField(startIndex.ToString(), GUILayout.Width(128), GUILayout.Height(24)));
        //startIndex = int.Parse(GUILayout.TextField(startIndex.ToString(), GUILayout.Width(128), GUILayout.Height(24)));
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height * 0.8f));
        GUILayout.BeginVertical();
        for (int i = 0; i < ResMD.Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            /*
            string title = $"{ResMD[i].MobName}({ResMD[i].MobID})";
            GUILayout.Label(title, EditorStyles.boldLabel);
            */
            GUILayout.BeginHorizontal();
            GUILayout.Label("File:", GUILayout.Width(48), GUILayout.Height(24)); 
            ResMD[i].objName = GUILayout.TextField(ResMD[i].objName, GUILayout.Width(68), GUILayout.Height(24));
            if (GUILayout.Button("Rename", GUILayout.Width(55), GUILayout.Height(24)))
            {
                if (string.IsNullOrEmpty(ResMD[i].objName))
                {
                    ResMD[i].objName = ResMD[i].name;
                }
                else
                {
                    AssetDatabase.RenameAsset($"Assets/Resources/{defaultPath}/{ResMD[i].name}.asset", ResMD[i].objName);
                    ResMD[i].objName = ResMD[i].name;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID:",GUILayout.Width(48),GUILayout.Height(24));
            ResMD[i].MobID = int.Parse(GUILayout.TextField(ResMD[i].MobID.ToString(), GUILayout.Width(128), GUILayout.Height(24)));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(48), GUILayout.Height(24));
            ResMD[i].MobName = GUILayout.TextField(ResMD[i].MobName, GUILayout.Width(128), GUILayout.Height(24));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon:", GUILayout.Width(48), GUILayout.Height(24));
            if(ResMD[i].MobIcon != null)
            {
                ResMD[i].iconPath = AssetDatabase.GetAssetPath(ResMD[i].MobIcon);
            }
            Sprite tempSprite = ResMD[i].MobIcon == null? null: ResMD[i].MobIcon;
            tempSprite = (Sprite)EditorGUILayout.ObjectField(tempSprite, typeof(Sprite), GUILayout.Width(128));
            ResMD[i].MobIcon = tempSprite;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Navigate", GUILayout.Width(160)))
            {
                EditorGUIUtility.PingObject(ResMD[i]);
            }
            GUILayout.EndVertical();

            if (ResMD[i].MobIcon != null)
            {
                GUILayout.Label(ResMD[i].MobIcon.texture, GUILayout.Height(128), GUILayout.Width(128));
            }
            else
            {
                GUILayout.Box("", GUILayout.Height(128), GUILayout.Width(128));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        //Debug.Log("Display Panel1");
    }
    Object source;
    Texture2D previewBackgroundTexture;
    private void TestBox()
    {
        source = EditorGUILayout.ObjectField(source, typeof(MobData), true);
        if(source != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            MobData md = source as MobData;
            string title = $"{md.MobName}({md.MobID})";
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.Label(md.MobIcon.texture, GUILayout.Height(128), GUILayout.Width(128));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            if(md.MobPrefab != null)
            {
                GUIStyle bgColor = new GUIStyle();
                bgColor.normal.background = previewBackgroundTexture;
                if (mobEditor == null)
                {
                    mobEditor = Editor.CreateEditor(md.MobPrefab);
                    mobEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 200), bgColor);
                }
            }
            if (GUILayout.Button("Navigate"))
            {
                EditorGUIUtility.PingObject(md);
            }
        }
    }
}
#endif