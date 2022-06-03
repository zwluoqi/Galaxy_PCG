#ifndef UNIVERSAL_CUSTOM_MULTIPLE_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_CUSTOM_MULTIPLE_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


TEXTURE2D(_OceanMap);       SAMPLER(sampler_OceanMap);

CBUFFER_START(UnityPerMaterial)
    float4 _OceanMap_ST;
    half4 _OceanColor;
    half _Cutoff;
    half _Surface;
    half2 _minmax;
    float _alphaMultiplier;
    float _waterSmoothness;
    float radius;
    float waveLen;
    float4 waves[12];
CBUFFER_END



#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float4, _OceanColor)
    UNITY_DOTS_INSTANCED_PROP(float , _Cutoff)
    UNITY_DOTS_INSTANCED_PROP(float , _Surface)
    UNITY_DOTS_INSTANCED_PROP(float2 , _minmax)
    UNITY_DOTS_INSTANCED_PROP(float , _alphaMultiplier)
    UNITY_DOTS_INSTANCED_PROP(float , _waterSmoothness)
    UNITY_DOTS_INSTANCED_PROP(float , radius)  
    UNITY_DOTS_INSTANCED_PROP(float , waveLen)  
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _OceanColor          UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata__OceanColor)
#define _Cutoff             UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__Cutoff)
#define _Surface            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__Surface)
#define _minmax            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float2  , Metadata__minmax)
#define _alphaMultiplier            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__alphaMultiplier)
#define _waterSmoothness            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__waterSmoothness)
#define radius            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4x4  , Metadata_radius)
#define waveLen            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_waveLen)
#endif




#endif
