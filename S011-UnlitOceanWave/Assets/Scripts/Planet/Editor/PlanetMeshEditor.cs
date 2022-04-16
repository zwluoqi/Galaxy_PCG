using System;
using Planet.Setting;
using UnityEditor;
using UnityEngine;

namespace Planet
{
    [CustomEditor(typeof(PlanetMesh))]
    public class PlanetMeshEditor : Editor
    {
        private PlanetMesh _planetMesh;
        private Editor shapeEditor;
        private Editor colorEditor;
        private Editor waterRenderEditor;
        
        private void OnEnable()
        {
            _planetMesh = target as PlanetMesh;
            ;
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

            _planetMesh.showNormalAndTangent = GUILayout.Toggle(_planetMesh.showNormalAndTangent, "Normal");
            
            
            DrawSettingEditor(_planetMesh.ShapeSettting, _planetMesh.OnShapeSetttingUpdated,
                ref _planetMesh.shapeSetttingsFoldOut, ref shapeEditor);
            DrawSettingEditor(_planetMesh.ColorSettting, _planetMesh.OnColorSetttingUpdated,
                ref _planetMesh.colorSetttingsFoldOut, ref colorEditor);
            DrawSettingEditor(_planetMesh.WaterRenderSettting, _planetMesh.OnWaterRenderSetttingUpdated,
                ref _planetMesh.waterRenderSetttingsFoldOut, ref waterRenderEditor);


            _planetMesh.UpdateMaterialProperty();
            if (GUILayout.Button("Mesh存储"))
            {
                SaveMesh(1);
            }
            
            
        }

        private void DrawSettingEditor(ScriptableObject planetMeshShapeSettting, Action onShapeSetttingUpdated, ref bool planetMeshShpaeSetttingsFoldOut, ref Editor editor)
        {
            if (planetMeshShapeSettting != null)
            {
                planetMeshShpaeSetttingsFoldOut =
                    EditorGUILayout.InspectorTitlebar(planetMeshShpaeSetttingsFoldOut, planetMeshShapeSettting);
                if (planetMeshShpaeSetttingsFoldOut)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        CreateCachedEditor(planetMeshShapeSettting, null, ref editor);
                        editor.OnInspectorGUI();
                        if (check.changed)
                        {
                            if (onShapeSetttingUpdated != null)
                            {
                                onShapeSetttingUpdated();
                            }
                        }
                    }
                }
            }
        }
    }
}