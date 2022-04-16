#ifndef COLOR_GENERATE_INCLUDED
#define COLOR_GENERATE_INCLUDED

#include "NoiseGenerate.hlsl"


struct LatitudeSettingBuffer{
    float startHeight;
};

struct ColorSettting{
    NoiseLayer noiseLayer;    
    float blendRange;   
};

float invLerp(float from, float to, float value){
  return clamp((value - from) / (to - from),0,1);
}

float ColorGenerateExeculate(ColorSettting colorSetting,
StructuredBuffer<NoiseLayer> noiseLayerSettings,int layerCount,
StructuredBuffer<LatitudeSettingBuffer> latitudes,int latitudeCount,
float3 vertex,float height){

    height = (height + 1) * 0.5f;
    float latitudeIndex = 0;
    float blendRange = colorSetting.blendRange + 0.001f;
    float2 noise = NoiseLayerExcute(vertex,colorSetting.noiseLayer,noiseLayerSettings,layerCount);
    float noiseHeight = noise.y + height;
    for (int i = 0; i < latitudeCount; i++)
    {
        float dist = noiseHeight - latitudes[i].startHeight;              
        float weight = invLerp(-blendRange,blendRange,dist);
        latitudeIndex *= (1- weight);
        latitudeIndex += i*weight;
    }

    return (latitudeIndex*1.0f) / max(1,(latitudeCount-1));            
}


#endif