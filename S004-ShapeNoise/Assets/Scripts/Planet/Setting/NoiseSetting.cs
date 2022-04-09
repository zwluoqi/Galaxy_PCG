using System;
using UnityEngine;

namespace Planet.Setting
{

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
        public NoiseType noiseType = 0;
    }
}