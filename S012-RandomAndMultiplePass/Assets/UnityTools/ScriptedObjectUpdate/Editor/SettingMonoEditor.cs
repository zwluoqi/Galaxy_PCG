using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;
using Object = UnityEngine.Object;



internal class SettingEditor<T> where T : ISettingUpdate
{
    private Editor[] _editors;
    private ScriptableObject[] _setting;
    private bool[] _toggle;
    public void OnEnable(Editor editor)
    {
        var type = typeof(T);
        var scriptableObject = typeof(ScriptableObject);
        var scrObjs =  type.FindMembers(MemberTypes.Field, BindingFlags.Public | BindingFlags.Instance, (info, criteria) =>
        {
            var memType = info as FieldInfo;
            if (memType != null && memType.FieldType.IsSubclassOf(scriptableObject))
            {
                return true;
            }

            return false;
        }, editor.target);
        _editors = new Editor[scrObjs.Length];
        _setting = new ScriptableObject[scrObjs.Length];
        _toggle = new bool[scrObjs.Length];
        _toggle[0] = true;
        
        for (int i = 0; i < scrObjs.Length; i++)
        {
            var scrObj = scrObjs[i];
            var fieldInfo = type.GetField(scrObj.Name);
            var obj = fieldInfo.GetValue(editor.target) as ScriptableObject;
            _setting[i] = obj;
        }
    }

    public void OnInspectorGUI(Editor editor)
    {
        for (int i = 0; i < _setting.Length; i++)
        {
            // _editors[i]=
            var obj = _setting[i];
            if (obj != null)
            {
                DrawSettingEditor(obj,((ISettingUpdate) editor.target).UpdateSetting,ref _toggle[i],ref _editors[i]);
            }
        }
        
    }

    private void DrawSettingEditor(ScriptableObject planetMeshShapeSettting, Action<ScriptableObject> onShapeSetttingUpdated, ref bool planetMeshShpaeSetttingsFoldOut, ref Editor editor)
    {
        if (planetMeshShapeSettting != null)
        {
            planetMeshShpaeSetttingsFoldOut =
                EditorGUILayout.InspectorTitlebar(planetMeshShpaeSetttingsFoldOut, planetMeshShapeSettting);
            if (planetMeshShpaeSetttingsFoldOut)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    Editor.CreateCachedEditor(planetMeshShapeSettting, null, ref editor);
                    editor.OnInspectorGUI();
                    if (check.changed)
                    {
                        onShapeSetttingUpdated?.Invoke(planetMeshShapeSettting);
                    }
                }
            }
        }
    }

}
