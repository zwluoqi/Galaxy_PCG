using System.IO;
using UnityEditor;
using UnityEngine;
using UnityTools.MeshTools;
using UnityTools.TextureTools;

namespace Clouds
{
    [CustomEditor(typeof(ShowTexture))]
    public class ShowTextureEditor : Editor
    {
        private SettingEditor<ShowTexture> shapeEdirot;

        private void OnEnable()
        {
            shapeEdirot = new SettingEditor<ShowTexture>();
            shapeEdirot.OnEnable(this);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            shapeEdirot.OnInspectorGUI(this);
            
            if (GUILayout.Button("生成图片"))
            {
                var textureSetting= (target as ShowTexture).textureSetting;
                var texture = GenerateTexture.Generate(textureSetting);
                File.WriteAllBytes("Assets/"+textureSetting.name+".png",texture.EncodeToPNG());
                AssetDatabase.Refresh();
            }
        }
        
    }
}