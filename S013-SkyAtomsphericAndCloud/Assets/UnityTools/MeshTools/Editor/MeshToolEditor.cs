using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityTools.Physics.Obb;

namespace UnityTools.MeshTools
{
    [CustomEditor(typeof(MeshToolMono))]
    public class MeshToolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
            if (GUILayout.Button("保存网格"))
            {
                var mono = target as MeshToolMono;
                var meshFilter = mono.GetComponent<MeshFilter>();
                var sharedMesh = meshFilter.sharedMesh;
                var mesh = Object.Instantiate(sharedMesh);
                mesh.hideFlags = HideFlags.None;
                AssetDatabase.CreateAsset(mesh, "Assets/UnityTools/MeshTools/" + mono.name+".asset");
            }
            
        }
    }
}