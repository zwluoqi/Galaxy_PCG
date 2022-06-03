#ifndef QINGZHU_CLOUDS
#define QINGZHU_CLOUDS
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#include "Assets/ShaderLabs/Shaders/RayMarchingIntersection.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

TEXTURE2D(_SkyTex);
SAMPLER(sampler_SkyTex);


// CBUFFER_START(UnityPerMaterial) // Required to be compatible with SRP Batcher
float4 _MainTex_ST;
float4 sphereCenter;
float radiusTerrain;
float radiusAtoms;


float atomDensityFalloff;
int numberStepSky;
float lightPhaseValue;

int numberStepLight;
float lightAbsorptionTowardSun;
float darknessThreshold;

float4 waveRGBScatteringCoefficients;
float sunSmoothness;
// CBUFFER_END


// Henyey-Greenstein
float hg(float cosTheta, float g) {
    float g2 =  g*g;
    return (1-g2) / (4*3.1415*pow(1+g2-2*g*(cosTheta), 1.5));
}

float sampleDensity(float3 worldpos)
{
    float3 ditToCenter = worldpos - sphereCenter;
    float size = (radiusAtoms-radiusTerrain);
    float height01 = saturate( (length(ditToCenter) - radiusTerrain)/size);
    float density = exp(-height01*atomDensityFalloff)*(1-height01);
    return density;
}


float4 marchingDensity(float3 rayPos,float3 rayDir,float rayLength)
{
    float stepSize = rayLength/numberStepLight;
    float totalDensity = 0;
    for (int step = 0;step <numberStepLight;step++)
    {
        rayPos += rayDir*stepSize;
        float density = sampleDensity(rayPos);
        totalDensity += density*stepSize;
    }
    return totalDensity;
}




#endif