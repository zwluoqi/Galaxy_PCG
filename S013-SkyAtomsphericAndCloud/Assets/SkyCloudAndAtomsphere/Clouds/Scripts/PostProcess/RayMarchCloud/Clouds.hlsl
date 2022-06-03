#ifndef QINGZHU_CLOUDS
#define QINGZHU_CLOUDS
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#include "Assets/ShaderLabs/Shaders/RayMarchingIntersection.hlsl"


TEXTURE3D(shapeNoise);
SAMPLER(sampler_shapeNoise);


TEXTURE3D(detailNoise);
SAMPLER(sampler_detailNoise);


TEXTURE2D(weatherMap);
SAMPLER(sampler_weatherMap);

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

TEXTURE2D(_CloudTex);
SAMPLER(sampler_CloudTex);


// CBUFFER_START(UnityPerMaterial) // Required to be compatible with SRP Batcher
float4 _MainTex_ST;
float4 sphereCenter;
float4 boxmin;
float4 boxmax;

float samplerScale;
float samplerHeightScale;
float4 samplerOffset;
float densityMultipler;
float densityThreshold;//云层密度阀值
int numberStepCloud;//云层密度检测步进

float lightPhaseStrength;//光线穿透能力
float lightPhaseIns;//[0,1]
float lightPhaseOuts;//[0,1]
float lightPhaseBlend;//[0,1]

float lightPhaseCsi;//[0,max]
float lightPhaseCse;//[0,max]

float lightAbsorptionThroughCloud;//云层对光的吸收率


int numberStepLight;//管线传播步进
float lightAbsorptionTowardSun;//云层对光的吸收率
float darknessThreshold;//最低光线穿透阀值

float globalCoverage;// [0, 1]
float globalDensity;// [0, max]
float globalStarHeight;//
float globalThickness;// [0.2, 0.4]云层厚度



float debug_shape_z;
float debug_rgba;
// CBUFFER_END


float SAT(float v)
{
    return saturate(v);
}

float remap(float v,float l0,float h0,float ln,float hn)
{
    return ln+(v-l0)*(hn-ln)/(h0-l0);
}

// Henyey-Greenstein
float hg(float cosTheta, float g) {
    float g2 =  g*g;
    return (1-g2) / (4*3.1415*pow(1+g2-2*g*(cosTheta), 1.5));
}

float ISextra(float cosTheta)
{
    return  lightPhaseCsi*pow(SAT(cosTheta),lightPhaseCse);
}

float lightPhase(float cosTheta)
{
    return lerp( max(hg(cosTheta,lightPhaseIns) ,ISextra(cosTheta)), hg(cosTheta,-lightPhaseOuts),lightPhaseBlend);
}

float sampleDensityBox(float3 worldPos)
{
    float3 samplePos = worldPos*samplerScale*0.001+samplerOffset*0.01;
    float4 rgba = SAMPLE_TEXTURE3D_LOD(shapeNoise, sampler_shapeNoise, samplePos,0);

    float3 size = boxmax - boxmin;
    float wc0=rgba.r;
    // return wc0;
    float wc1=rgba.g;
    float wh=rgba.b;
    float wd=rgba.a;
    
    
    float gMin = .2;
    float gMax = .7;
    float heightPercent = (worldPos.y - boxmin.y) / size.y;
    // return heightPercent;
    
    float heightGradient = saturate(remap(heightPercent, 0.0, gMin, 0, 1))
    * saturate(remap(heightPercent, 1, gMax, 0, 1));


    float density = max(0,wc0-densityThreshold)*densityMultipler;

    
    return density*heightGradient;
}


float sampleDensitySphere(float3 worldPos)
{
    float3 samplePos = worldPos*samplerScale*0.001+samplerOffset*0.01;
    // float3 samplePos = worldPos;
    
    //height gridiend
    float3 size = boxmax - boxmin;
    // float halfHeight = size.x*0.5f;
    float3 dir = worldPos-sphereCenter;
    float height = length(dir);    
    float heightPercent = saturate( (height - boxmin.x) / (size.x) );
    

    float3 normal = normalize(dir);
    float u = samplerScale*0.01*atan(normal.z/normal.x) / 3.1415*2.0 + samplerOffset.x*0.01;
    float v = samplerScale*0.01*asin(normal.y) / 3.1415*2.0 + 0.5 + samplerOffset.y*0.01;
    
    float3 textureCoord = float3(u,v,heightPercent*samplerHeightScale*0.01);
    float4 rgba = SAMPLE_TEXTURE3D_LOD(shapeNoise, sampler_shapeNoise, textureCoord,0);
    
    float snr=rgba.r;

    float sng=rgba.g;
    float snb=rgba.b;
    float sna=rgba.a;
    

    float4 detail_rgba = SAMPLE_TEXTURE3D_LOD(detailNoise, sampler_detailNoise, textureCoord,0);

    float dnr=detail_rgba.r;
    float dng=detail_rgba.g;
    float dnb=detail_rgba.b;
    float dna=detail_rgba.a;
    
    //weather map

    // float4 weatherColor = SAMPLE_TEXTURE2D_LOD(weatherMap, sampler_weatherMap,float2(weatherU,weatherV),0);
    // return weatherColor.r;
    // float wh = weatherColor.z;//blue channel
    float wh = 1;
    //
    //4 WM = max(wc0,STA(gc-0.5)*wc1*2) 云出现率<br>
    float WMc = 1;
    
//     5 SRb = SAT(R(ph, 0, 0.07, 0, 1)) 向下映射<br>
//     6 SRt = SAT(R(ph, wh ×0.2, wh, 1, 0)) 向上映射<br>
//     7 SA = SRb × SRt <br>
    float SRb = SAT(remap(heightPercent, 0, 0.07+globalStarHeight, 0, 1));
    float SRt = SAT(remap(heightPercent, wh*globalThickness, wh, 1, 0));
    float SA = SRb*SRt;//[0,1]

    
    // float density2 = max(0,snr-densityThreshold)*densityMultipler;
    // return density2*SA;
    
    // 8 DRb = ph ×SAT(R(ph, 0, 0.15, 0, 1)) 向底部降低密度<br>
    // 9 DRt = SAT(R(ph, 0.9, 1.0, 1, 0))) 向顶部的更柔和的过渡降低密度<br>
    // 10 DA = gd × DRb × DRt × wd × 2 密度融合<br>
    
    float DRb = SAT(remap(heightPercent, 0,  0.15, 0, 1));
    float DRt = SAT(remap(heightPercent, 0.9, 1, 1, 0));
    float DA = globalDensity*DRb*DRt*1*0.25;//[0,max]

    // 11 SNsample = R(snr, (sng ×0.625+snb ×0.25+sna ×0.125)−1, 1, 0, 1)  FBM gba <br>
    // 12 SN = SAT(R(SNsample ×SA, 1−gc ×WMc, 1, 0, 1))×DA 
    float SNsample = remap(snr, (sng *0.625+snb *0.25+sna *0.125)-1.0f, 1, 0, 1);
    float SN =  SAT(remap(SNsample *SA, 1 - globalCoverage * WMc, 1, 0, 1))*DA;
    // return SN;
    
    // return SN;
    //13 DNfbm = dnr ×0.625+dng ×0.25+dnb ×0.125
    //14 DNmod = 0.35×e−gc×0.75 ×Li(DNfbm, 1−DNfbm, SAT(ph ×5))
    //15 SNnd = SAT(R(SNsample ×SA, 1−gc ×WMc, 1, 0, 1))
    //16 d = SAT(R(SNnd, DNmod, 1, 0, 1)))×DA
    float DNfbm = dnr *0.625+dng *0.25+dnb *0.125;
    float DNmod = 0.35*exp(-globalCoverage*0.75f)*lerp(DNfbm, 1-DNfbm, SAT(heightPercent *5));
    float SNnd = SAT(remap(SNsample *SA, 1 - globalCoverage * WMc, 1, 0, 1));
    float d = SAT(remap(SNnd, DNmod, 1, 0, 1))*DA;
    
    float density = d;

    return density;
}



float sampleDensity(float3 worldpos)
{
    #if SHAPE_BOX
    return sampleDensityBox(worldpos);
    #elif SHAPE_SPHERE
    return sampleDensitySphere(worldpos);
    #else
    return 0;
    #endif
}


float lightMarchingDensity(float3 rayPos,float3 dirToLight,float rayLength)
{
    // return 0.0f;
    
    float stepSize = rayLength/numberStepLight;
    float totalDensity = 0;
    for (int step = 0;step <numberStepLight;step++)
    {
        rayPos += dirToLight*stepSize;
        float density = sampleDensity(rayPos);
        totalDensity += density*stepSize;
    }
    return totalDensity;

    
    // return darknessThreshold + transmittance*(1-darknessThreshold);
}




#endif