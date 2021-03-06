using UnityEngine;

namespace Planet.Setting
{
    
    [CreateAssetMenu()]
    public class ShapeSettting:ScriptableObject
    {
        
        public bool GPU = false;
        public ComputeShader computeShader;
        [Range(0.1f,10000)]
        public float radius=1;
        public bool _noiseEnable = true;
        public NoiseSetting _noiseSetting;
        
        public NoiseLayer[] _noiseLayers;

        public ShapeSettingBuffer[] ToBaseBuffer()
        {
            ShapeSettingBuffer[] settingBuffer = new ShapeSettingBuffer[1];
            settingBuffer[0].radius = this.radius;
            settingBuffer[0]._noiseEnable = this._noiseEnable?1.0f:0.0f;
            settingBuffer[0]._noiseSetting = _noiseSetting.ToBuffer();
            return settingBuffer;
        }

        public NoiseLayerBuffer[] ToLayerBuffer()
        {
            NoiseLayerBuffer[] noiseLayerBuffers = new NoiseLayerBuffer[this._noiseLayers.Length];
            for (int i = 0; i < this._noiseLayers.Length; i++)
            {
                noiseLayerBuffers[i] = this._noiseLayers[i].ToBuffer();
            }

            return noiseLayerBuffers;
        }
    }
    
    [System.Serializable]
    public class NoiseLayer
    {
        public bool enable = true;
        public bool useMask = false;
        public NoiseSetting noiseSetting = default;

        public NoiseLayerBuffer ToBuffer()
        {
            NoiseLayerBuffer buffer = new NoiseLayerBuffer();
            buffer.enable = this.enable?1.0f:0.0f;
            buffer.useMask = this.useMask?1.0f:0.0f;
            buffer.noiseSetting = this.noiseSetting.ToBuffer();
            return buffer;
        }
    }

    public struct NoiseLayerBuffer
    {
        public float enable;
        public float useMask;
        public NoiseSettingBuffer noiseSetting;
    }

    public struct ShapeSettingBuffer
    {
        public float radius;
        public float _noiseEnable;
        public NoiseSettingBuffer _noiseSetting;
    }
}