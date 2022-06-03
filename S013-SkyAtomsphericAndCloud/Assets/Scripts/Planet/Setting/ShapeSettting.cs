using UnityEngine;

namespace Planet.Setting
{
    [SerializeField]
    public enum GenerateType
    {
        NormalizeCube,//四面体划分
        UVSphere,//经纬度迭代
        IcosahedronSphere,//二十面体，迭代细分三角形
        QuadSphere,//类似标准经纬度迭代
        GoldbergPloyhedra,//十二面体对偶迭代
    }

    [CreateAssetMenu()]
    public class ShapeSettting:ScriptableObject
    {
        public GenerateType  generateType;
        public ComputeShader computeShader;
        [Range(0.1f,10000)]
        public float radius=1;
        public bool _noiseEnable = true;
        public NoiseSetting _noiseSetting;
        
        public NoiseLayer[] _noiseLayers;

        public ShapeSettingBuffer[] ToBaseBuffer(RandomData randomSetting)
        {
            ShapeSettingBuffer[] settingBuffer = new ShapeSettingBuffer[1];
            settingBuffer[0].radius = this.radius;
            settingBuffer[0].noiseLayer = new NoiseLayerBuffer(this._noiseEnable?1.0f:0.0f,_noiseSetting.ToBuffer(randomSetting));
            return settingBuffer;
        }

        public NoiseLayerBuffer[] ToLayerBuffer(RandomData randomSetting)
        {
            NoiseLayerBuffer[] noiseLayerBuffers = new NoiseLayerBuffer[this._noiseLayers.Length];
            for (int i = 0; i < this._noiseLayers.Length; i++)
            {
                noiseLayerBuffers[i] = this._noiseLayers[i].ToBuffer(randomSetting);
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

        public NoiseLayerBuffer ToBuffer(RandomData randomSetting)
        {
            NoiseLayerBuffer buffer = new NoiseLayerBuffer();
            buffer.enable = this.enable?1.0f:0.0f;
            buffer.useMask = this.useMask?1.0f:0.0f;
            buffer.noiseSetting = this.noiseSetting.ToBuffer(randomSetting);
            return buffer;
        }
    }

    public struct NoiseLayerBuffer
    {
        public float enable;
        public float useMask;
        public NoiseSettingBuffer noiseSetting;

        public NoiseLayerBuffer(float _enable, NoiseSettingBuffer _noiseSetting)
        {
            this.enable = _enable;
            this.noiseSetting = _noiseSetting;
            this.useMask = 0;
        }
    }

    public struct ShapeSettingBuffer
    {
        public float radius;

        public NoiseLayerBuffer noiseLayer;
    }
}