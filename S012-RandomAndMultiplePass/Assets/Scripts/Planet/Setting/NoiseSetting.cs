using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Planet.Setting
{

    public enum NoiseType
    {
        Simplex,
        Rigidbody,
        Erosion,
        Uber,
    }
    [System.Serializable]
    public class NoiseSetting
    {
        public NoiseType noiseType;

        public SimpleNoise simpleNoise;
        public UberNoise uberNoise;
        
        public NoiseSettingBuffer ToBuffer(RandomData randomSetting)
        {
            NoiseSettingBuffer buffer = new NoiseSettingBuffer();

            buffer.noiseType = (int)this.noiseType;
            if (this.noiseType == NoiseType.Uber)
            {
                buffer.layer = uberNoise.layer;
                buffer.layerRoughness = uberNoise.layerRoughness;
                buffer.layerMultiple = uberNoise.layerMultiple;
                buffer.strength = uberNoise.strength;
                buffer.roughness = uberNoise.roughness;
                buffer.offset = uberNoise.offset;
                buffer.minValue = uberNoise.minValue;

                buffer.lfPerturbFeatures = uberNoise.lfPerturbFeatures;
                buffer.lfSharpness = uberNoise.lfSharpness;
                buffer.lfAltitudeErosion = uberNoise.lfAltitudeErosion;
                buffer.lfRidgeErosion = uberNoise.lfRidgeErosion;
                buffer.lfSlopeErosion = uberNoise.lfSlopeErosion;
                buffer.lfLacunarity = uberNoise.lfLacunarity;
            }
            else
            {
                buffer.layer = simpleNoise.layer;
                buffer.layerRoughness = simpleNoise.layerRoughness*(1+randomSetting.frequencyAddPercent);
                buffer.layerMultiple = simpleNoise.layerMultiple*(1+randomSetting.amplitudeAddPercent);
                buffer.strength = simpleNoise.strength*(1+randomSetting.amplitudeAddPercent);
                buffer.roughness = simpleNoise.roughness*(1+randomSetting.frequencyAddPercent);

                buffer.offset = simpleNoise.offset + randomSetting.offsetRange;
                buffer.minValue = simpleNoise.minValue;
            }
            
            return buffer;
        }
        
    }



    [System.Serializable]
    public class SimpleNoise
    {
        [Range(1,20)]
        public int layer=1;
        public float layerRoughness = 2;
        public float layerMultiple = 0.5f;
        public float strength = 1;
        public float roughness = 1;
        public Vector3 offset = Vector3.zero;
        public float minValue = 0.0f;
    }
    
    [System.Serializable]
    public class UberNoise:SimpleNoise
    {
        [Tooltip("方向扰动")]
        public float lfPerturbFeatures;
        [Tooltip("锐化")]
        [Range(-1,1)]
        public float lfSharpness;
        [Tooltip("高地侵蚀")]
        [Range(0,1)]
        public float lfAltitudeErosion;
        [Tooltip("山脊侵蚀")]
        [Range(0,100)]
        public float lfRidgeErosion;
        [Tooltip("斜坡侵蚀")]
        [Range(0,100)]
        public float lfSlopeErosion;
        [Tooltip("缺顶")]
        public float lfLacunarity;
    }

    
    public struct NoiseSettingBuffer
    {
        public int layer;
        public float layerRoughness;
        public float layerMultiple ;
        public float strength ;
        public float roughness ;
        public Vector3 offset ;
        //public NoiseType noiseType =  NoiseType.SIMPLE;
        public float minValue ;
        public int noiseType;
        
        
        public float lfPerturbFeatures;
        public float lfSharpness;
        public float lfAltitudeErosion;
        public float lfRidgeErosion;
        public float lfSlopeErosion;
        public float lfLacunarity;
    }

}