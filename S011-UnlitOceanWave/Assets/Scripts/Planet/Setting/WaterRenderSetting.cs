using UnityEngine;

namespace Planet.Setting
{
    [System.Serializable]
    public class WaterSetting
    {
        [Tooltip("频率")]
        public float Frenque = 0.1f;
        [Tooltip("振幅")]
        public float Strength = 0.01f;
        [Tooltip("速率")]
        public float Speed = 0.1f;
        [Tooltip("方向")]
        public float Dir = 0;

        public Vector4 ToVec4()
        {
            return new Vector4(Frenque,Strength,Speed,Dir);
        }

        public WaterSetting()
        {
            Frenque = 0.1f;
            Strength = 0.01f;
            Speed = 0.1f;
            Dir = 0;
        }
    }
    
    [System.Serializable]
    public class WaterLayer
    {
        public bool enable = true;
        public bool useMask = false;
        public WaterSetting waterSetting = default;

        public Vector4 ToVec4s()
        {
            return waterSetting.ToVec4();
        }
    }

    [CreateAssetMenu()]
    public class WaterRenderSetting:ScriptableObject
    {
        public const int shaderWaveMaxSize = 12;

        public WaterLayer[] waterLayers = new WaterLayer[1];
        public float alphaMultiplier = 10;
        public float waterSmoothness = 0.1f;
        
        
        public Vector4[] ToWaveVec4s()
        {
            
            Vector4[] vector4s = new Vector4[shaderWaveMaxSize];
            for (int i = 0; i < waterLayers.Length; i++)
            {
                vector4s[i] = waterLayers[i].ToVec4s();
            }

            return vector4s;
        }
    }
}