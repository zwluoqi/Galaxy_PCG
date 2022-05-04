using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTools.TextureTools
{
    public class CombineTextureWindow : EditorWindow
    {
        [MenuItem("Assets/TextureTools/贴图合并窗口")]
        private static void ShowWindow()
        {
            var window = GetWindow<CombineTextureWindow>();
            window.titleContent = new GUIContent("贴图合并窗口");
            window.Show();
        }

        public Texture2D[] rgba = new Texture2D[4];
        public int[] rgbaSample = new int[4];
        public string[] RGBA = {"R", "G", "B", "A"};
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            rgba[0] = (Texture2D)EditorGUILayout.ObjectField("R通道",rgba[0], typeof(Texture2D),false);
            rgbaSample[0] = EditorGUILayout.Popup(rgbaSample[0], RGBA);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            rgba[1] = (Texture2D)EditorGUILayout.ObjectField("G通道",rgba[1], typeof(Texture2D),false);
            rgbaSample[1] = EditorGUILayout.Popup(rgbaSample[1], RGBA);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            rgba[2] = (Texture2D)EditorGUILayout.ObjectField("B通道",rgba[2], typeof(Texture2D),false);
            rgbaSample[2] = EditorGUILayout.Popup(rgbaSample[2], RGBA);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            rgba[3] = (Texture2D)EditorGUILayout.ObjectField("A通道",rgba[3], typeof(Texture2D),false);
            rgbaSample[3] = EditorGUILayout.Popup(rgbaSample[3], RGBA);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("合并"))
            {
                var path = AssetDatabase.GetAssetPath(rgba[0]);
                var dir = Path.GetDirectoryName(path);
                TextureFunc.CombineTexByRGBA(rgba, rgbaSample,dir + "/" + rgba[0].name + "_combine.png");
            }
        }
    }
}