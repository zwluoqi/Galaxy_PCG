using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SolarSystemSimulater))]
public class SolarSystemSimulaterEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GlobalDefine.deltaTime = EditorGUILayout.FloatField("deltaTime", GlobalDefine.deltaTime);
        if (Math.Abs(GlobalDefine.deltaTime - Time.fixedDeltaTime) > 0.0001f)
        {
            Time.fixedDeltaTime = GlobalDefine.deltaTime;
        }
    }
}