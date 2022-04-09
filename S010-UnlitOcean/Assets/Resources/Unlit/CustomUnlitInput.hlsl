#ifndef UNIVERSAL_CUSTOM_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_CUSTOM_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"



CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    half4 _BaseColor;
    half _Cutoff;
    half _Surface;
    half2 _minmax;
    half _alphaMultiplier;
    half _waterSmoothness;
    half _speed;
CBUFFER_END

TEXTURE2D(FlowMapS);        SAMPLER(sampler_FlowMapS);
TEXTURE2D(NoiseMapS);       SAMPLER(sampler_NoiseMapS);
TEXTURE2D(WaveMapS0);         SAMPLER(sampler_WaveMapS0);
TEXTURE2D(WaveMapS1);         SAMPLER(sampler_WaveMapS1);


#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DOTS_INSTANCED_PROP(float , _Cutoff)
    UNITY_DOTS_INSTANCED_PROP(float , _Surface)
    UNITY_DOTS_INSTANCED_PROP(float2 , _minmax)
    UNITY_DOTS_INSTANCED_PROP(float , _alphaMultiplier)
    UNITY_DOTS_INSTANCED_PROP(float , _waterSmoothness)
    UNITY_DOTS_INSTANCED_PROP(float , _speed)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _BaseColor          UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata__BaseColor)
#define _Cutoff             UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__Cutoff)
#define _Surface            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__Surface)
#define _minmax            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float2  , Metadata__minmax)
#define _alphaMultiplier            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__alphaMultiplier)
#define _waterSmoothness            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__waterSmoothness)
#define _speed            UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata__speed)
#endif




#endif
