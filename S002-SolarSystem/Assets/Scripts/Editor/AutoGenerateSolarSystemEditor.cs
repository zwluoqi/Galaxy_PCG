using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoGenerateSolarSystem))]
public class AutoGenerateSolarSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
            
        base.OnInspectorGUI();
        if (GUILayout.Button("生成恒星系统"))
        {
            AutoGenerateSolarSystem.Inst.Generator();
        }
        
    }
}