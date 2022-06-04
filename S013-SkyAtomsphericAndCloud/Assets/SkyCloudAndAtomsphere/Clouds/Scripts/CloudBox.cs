using System;
using UnityEngine;

namespace Clouds
{
    public class CloudBox:MonoBehaviour
    {
        private void Start()
        {
            
        }

        public enum CloudShape
        {
            BOX,
            SPHERE,
        }

        public CloudShape cloudShape = CloudShape.BOX;
        
        
        [Header ( "Density" )]
        [Min(0.001f)]
        public float samplerScale = 1;
        [Min(0.001f)]
        public float samplerHeightScale = 1;
        public Vector3 samplerOffset = Vector3.zero;
        
        
        [Header ( "----Shape Noise" )]
        public Texture3D textureShape;
        public Texture3D detailShape;
        public Texture2D weatherMap;
        public Texture2D rayMarchOffsetMap;
        
        [Header ( "----Global Value" )]
        [Range(0,1)]
        public float globalCoverage = 1;
        [Min(0.01f)]
        public float globalDensity = 1;
        [Range(0.0f, 1f)] 
        public float globalStarHeight = 0.2f;
        [Range(0.2f,1f)]
        public float globalThickness = 0.2f;
        
        [Header ( "----Box Value" )]
        [Range(0.001f,1.0f)]
        public float densityThreshold;
        [Min(0.011f)]
        public float densityMultipler = 1;


        [Header ( "Lighting" )]
        [Range(1,256)]
        [Tooltip("云层光线步进数量")]
        public int numberStepCloud = 100;
        
        [Header ( "----Henyey-Greenstein’s" )]
        [Min(0.01f)]
        [Tooltip("光能强度")]
        public float lightPhaseStrength = 1;
        [Range(0,1)]
        [Tooltip("前向散射")]
        public float lightPhaseIns = 0.5f;
        [Range(0,1)]
        [Tooltip("后向散射")]
        public float lightPhaseOuts = 0.5f;
        [Range(0,1)]
        public float lightPhaseBlend = 0.5f;
        [Header ( "----Henyey-Greenstein a secondary term" )]
        [Range(0,100)]
        public float lightPhaseCsi = 10;
        [Range(0,100)]
        public float lightPhaseCse = 20;
        
        [Header ( "----light transmittance" )]
        [Min(0.01f)]
        [Tooltip("摄像机射向云层使对光线吸收率")]
        public float lightAbsorptionThroughCloud = 1;

        
        [Header ( "----light sun marching" )]
        [Tooltip("光源射向云层使对光线吸收率")]
        [Min(0.01f)]
        public float lightAbsorptionTowardSun = 1;
        [Range(0.01f,1.0f)]
        [Tooltip("最低光线穿透阀值")]
        public float darknessThreshold = 0.01f;
        [Range(1,128)]
        [Tooltip("太阳光线步进数量")]
        public int numberStepLight = 100;
        
        [Header ( "Debug" )]
        public DEBUG_SHAPE debug_shape_noise;
        
        [Range(0,1)]
        public float debug_shape_z;

        public RGBA_DEBUG debug_rgba = RGBA_DEBUG.ALL;
    }

    public enum DEBUG_SHAPE
    {
        NONE,
        SHAPE,
        DETAIL,
    }
    public enum RGBA_DEBUG
    {
        R,
        G,
        B,
        A,
        ALL,
    }
}