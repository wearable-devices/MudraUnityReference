using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;


[CustomEditor(typeof(MudraSettings))]
public class MudraSettingsEditor : Editor
{
    [SerializeField]
    string URL = "https://jitpack.io/w/user";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MudraSettings myScript = (MudraSettings) target;
        if (GUILayout.Button("Set Token"))
        {
            FixGradleTemplate(myScript.authToken);
        }

        if (GUILayout.Button("get Token"))
        {
            Application.OpenURL(URL);
        }

      }

    [MenuItem("GameObject/Mudra",false,0)]   
    private static void AddMudra()
    {

        GameObject MudraGameObject = new GameObject();
        MudraGameObject.name = "Mudra Manager";        
        MudraGameObject.AddComponent<MudraSettings>();
        MudraGameObject.AddComponent<MudraManager>();
    }

    public void FixGradleTemplate(string authToken)
    {
        if (authToken != null)
        {
            var gradleFile = Application.dataPath + "/Plugins/Android/mainTemplate.gradle";
            var gradleBackupFile = Application.dataPath + "/Plugins/Android/mainTemplate.source";
            
            var gradleData = File.ReadAllText(gradleBackupFile);   
            gradleData = gradleData.Replace("**TOKEN**","'"+ authToken+"'");
            File.WriteAllText(gradleFile, gradleData);
        }
    }

}
