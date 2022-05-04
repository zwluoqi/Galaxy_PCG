using System;
using Planet.Setting;
using UnityEditor;
using UnityEngine;
using UnityTools.ScriptedObjectUpdate;

namespace Planet
{
    [CustomEditor(typeof(PlanetMesh))]
    public class PlanetMeshEditor : Editor
    {
        private SettingEditor<PlanetMesh> shapeEdirot;
        private PlanetMesh _planetMesh;
        private void OnEnable()
        {
            _planetMesh = target as PlanetMesh;
            shapeEdirot = new SettingEditor<PlanetMesh>();
            shapeEdirot.OnEnable(this);
        }

        public override void OnInspectorGUI()
        {
            using (var check  = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (check.changed)
                {
                    _planetMesh.OnBaseUpdate();
                }
            }
            
            shapeEdirot.OnInspectorGUI(this);
            _planetMesh.UpdateMaterialProperty();
            // _planetMesh.UpdateLod();
            if (GUILayout.Button("Mesh存储"))
            {
                SaveMesh(1);
            }
        }

        public void SaveMesh(int indx)
        {
            var mesh = new Mesh();
            mesh.vertices = _planetMesh._meshFilterss[indx].sharedMesh.vertices;
            mesh.triangles = _planetMesh._meshFilterss[indx].sharedMesh.triangles;
            mesh.uv = _planetMesh._meshFilterss[indx].sharedMesh.uv;
            mesh.normals = _planetMesh._meshFilterss[indx].sharedMesh.normals;
            mesh.colors = _planetMesh._meshFilterss[indx].sharedMesh.colors;
            mesh.UploadMeshData(false);
            foreach (var vector3 in mesh.vertices)
            {
                Debug.LogWarning(vector3);
            }
            AssetDatabase.CreateAsset(mesh,"Assets/mesh"+_planetMesh.resolution+".asset");
        }
        
    }
}