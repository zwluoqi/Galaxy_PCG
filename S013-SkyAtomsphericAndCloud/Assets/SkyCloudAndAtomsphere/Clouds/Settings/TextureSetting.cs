using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clouds.Settings
{
    
    [CreateAssetMenu()]
    public class TextureSetting : ScriptableObject
    {

        [Range(8,128)]
        public int resolution = 8;
        public bool thirdTexture = false;
        public NoiseLayer[] noiseLayers = new NoiseLayer[1];
        
    }

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enable = true;
        public int mask;
        public NoiseType noiseType;
        public Vector3 offset;
        [Min(0.0001f)]
        public float frequency = 1;
        [Min(0.0001f)]
        public float amplify = 1;
        [Range(1,6)]
        public int layerCount = 1;
        [Min(0.0001f)]
        public float layerWeigthMultiper = 1;
        [Min(0.0001f)]
        public float layerAmplifyMultiper = 1;
        [Min(0.0001f)]
        public float layerFrequencyMultiper = 1;
        
        [Min(0.0001f)]
        public float layerOffsetMultiper = 1;
    }

    public enum NoiseType
    {
        Perlin,
        Simplex,
        CustomSimplex,
        Worley,
        // CustomSimplex_Worley
    }
    
}

