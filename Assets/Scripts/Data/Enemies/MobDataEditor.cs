using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
[CustomEditor(typeof(MobData))]
public class MobDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MobData md = target as MobData;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        if (md.MobIcon != null)
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUILayout.Label("Preview Icon", style, GUILayout.ExpandWidth(true));
            int lIndex = md.iconPath.LastIndexOf("/");
            string iconFilename = md.iconPath.Substring(lIndex);
            string onlyFilename = Path.GetFileNameWithoutExtension(iconFilename);
            Sprite loadIcon = Resources.Load<Sprite>($"Sprite/{onlyFilename}");
            if(loadIcon != null)
            {
                GUILayout.Label(loadIcon.texture, GUILayout.Height(128), GUILayout.Width(128));
            }
            //GUILayout.Label(md.MobIcon.texture, GUILayout.Height(128), GUILayout.Width(128));
            if(GUILayout.Button("Copy to Resource"))
            {
                string withoutExtension = md.iconPath.Substring(0, lIndex) + "/" + onlyFilename;
                bool isSuccess = AssetDatabase.CopyAsset(md.iconPath, $"Assets/Resources/Sprite/{iconFilename}");
                if (isSuccess)
                {
                    Debug.Log($"Copy {onlyFilename} To: Assets/Resources/Sprite{iconFilename}");
                }
                else
                {
                    Debug.Log("Copy Failed!");
                }
            }
        }
        GUILayout.EndVertical(); 
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    #region DirectRead
    /*
    if (md.MobIcon != null)
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUILayout.Label("Preview Icon", style, GUILayout.ExpandWidth(true));
            if (string.IsNullOrEmpty(md.iconPath) == false)
            {
                int lIndex = md.iconPath.LastIndexOf("/");
                string iconFilename = md.iconPath.Substring(lIndex);
                iconFilename = Path.GetFileNameWithoutExtension(iconFilename);
                iconFilename = md.iconPath.Substring(0, lIndex) + "/" + iconFilename;
                GUILayout.Label($"Path: {iconFilename}", style, GUILayout.ExpandWidth(true));
                byte[] image = File.ReadAllBytes(md.iconPath);
                Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                texture.LoadImage(image);
                if (texture != null)
                {
                    loadTexture = texture;
                    loadSprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f), 100);
                }
            }
            if (loadSprite != null)
            {
                GUILayout.Label(loadSprite.texture, GUILayout.Height(128), GUILayout.Width(128));
            }

        }
     */
    #endregion
}
#endif