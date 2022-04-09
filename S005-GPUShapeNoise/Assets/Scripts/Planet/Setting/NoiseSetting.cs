using System;
using UnityEngine;

namespace Planet.Setting
{

    public enum NoiseType
    {
        Simplex,
        Rigidbody,
    }
    [System.Serializable]
    public class NoiseSetting
    {
        [Range(1,20)]
        public int layer=1;
        public float layerRoughness = 2;
        public float layerMultiple = 0.5f;
        public float strength = 1;
        public float roughness = 1;
        public Vector3 offset = Vector3.zero;
        //public NoiseType noiseType =  NoiseType.SIMPLE;
        public float minValue = 0.0f;
        public NoiseType noiseType;

        public NoiseSettingBuffer ToBuffer()
        {
            NoiseSettingBuffer buffer = new NoiseSettingBuffer();
            buffer.layer = this.layer;
            buffer.layerRoughness = this.layerRoughness;
            buffer.layerMultiple = this.layerMultiple;
            buffer.strength = this.strength;
            buffer.roughness = this.roughness;
            buffer.offset = this.offset;
            buffer.minValue = this.minValue;
            buffer.noiseType = (int)this.noiseType;
            return buffer;
        }
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
    }
}