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

        private void OnEnable()
        {
            _planetMesh = target as PlanetMesh;
            ;
        }

        public override void OnInspectorGUI()
        {
            using (var check  = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (check.changed)
                {
                    _planetMesh.Generate();
                }
            }

            DrawSettingEditor(_planetMesh.ShapeSettting, _planetMesh.OnShapeSetttingUpdated,
                ref _planetMesh.shapeSetttingsFoldOut, ref shapeEditor);
            DrawSettingEditor(_planetMesh.ColorSettting, _planetMesh.OnColorSetttingUpdated,
                ref _planetMesh.colorSetttingsFoldOut, ref colorEditor);
            if (GUILayout.Button("Mesh存储"))
            {
                _planetMesh.SaveMesh(1);
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