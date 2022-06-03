using System;
using Clouds.Settings;
using UnityEngine;

namespace Clouds
{
    public class ShowTexture:MonoBehaviour,ISettingUpdate
    {

        public TextureSetting textureSetting;
        public Material material;
        MeshRenderer meshRenderer;


        public void GenerateTexture()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            var texture2D = Clouds.GenerateTexture.Generate(textureSetting);
            meshRenderer.sharedMaterial = material;
            material.mainTexture = texture2D;
        }

        public void UpdateSetting(ScriptableObject scriptableObject)
        {
            GenerateTexture();
        }

        private void OnValidate()
        {
            GenerateTexture();
        }
    }
}