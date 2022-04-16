using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Planet.Setting
{
    [CreateAssetMenu()]
    public class ColorSettting:ScriptableObject
    {
        // public bool GPU = false;
        public ComputeShader computeShader;
        public int resolution = 128;
        public Color tinyColor;
        public Material material;
        public Material oceanMaterial;
        public Gradient oceanMainLand;
        public Gradient ocean;
        
        public LatitudeSetting[] LatitudeSettings = new LatitudeSetting[1];

        public bool noiseEnable;
        public NoiseSetting noiseSetting;
        public NoiseLayer[] noiseLayers;
        [Range(0,0.1f)]
        public float blendRange = 0.03f;


        public ColorSettingBuffer[] GetBaseBuffer()
        {
            ColorSettingBuffer[] colorSettingBuffer = new ColorSettingBuffer[1];
            colorSettingBuffer[0].noiseLayer = new NoiseLayerBuffer(this.noiseEnable?1.0f:0.0f,noiseSetting.ToBuffer());
            colorSettingBuffer[0].blendRange = blendRange;
            return colorSettingBuffer;
        }
        
        public NoiseLayerBuffer[] GetNoiseLayersBuffer()
        {
            NoiseLayerBuffer[] noiseLayerBuffers = new NoiseLayerBuffer[this.noiseLayers.Length];
            for (int i = 0; i < this.noiseLayers.Length; i++)
            {
                noiseLayerBuffers[i] = this.noiseLayers[i].ToBuffer();
            }

            return noiseLayerBuffers;
        }

        public LatitudeSettingBuffer[] GetLatitudeSettingsBuffer()
        {
            LatitudeSettingBuffer[] latitudeSettingBuffers = new LatitudeSettingBuffer[this.LatitudeSettings.Length];
            for (int i = 0; i < this.LatitudeSettings.Length; i++)
            {
                latitudeSettingBuffers[i] = this.LatitudeSettings[i].ToBuffer();
            }

            return latitudeSettingBuffers;
        }


    }

    [System.Serializable]
    public class LatitudeSetting
    {
        public Gradient gradient;
        public Color tinyColor;
        [Range(0,1)]
        public float tinyPercent;
        [Range(0,1)]
        public float startHeight;

        public LatitudeSettingBuffer ToBuffer()
        {
            LatitudeSettingBuffer buffer  = new LatitudeSettingBuffer();
            buffer.startHeight = startHeight;
            return buffer;
        }
    }

    public struct ColorSettingBuffer
    {
        public NoiseLayerBuffer noiseLayer;
        public float blendRange;
    }

    public struct LatitudeSettingBuffer
    {
        public float startHeight;
    }
}