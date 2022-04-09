#ifndef NOISE_GENERATE_INCLUDED
#define NOISE_GENERATE_INCLUDED

#include "../NoiseShader/SimplexNoise3D.hlsl"

struct NoiseSetting{
    int layer;
    float layerRoughness;
    float layerMultiple;
    float strength ;
    float roughness;
    float3 offset ;
    //NoiseType noiseType =  NoiseType.SIMPLE;
    float minValue ;
    int noiseType;
};

struct NoiseLayer
{
    float enable;
    float useMask;
    NoiseSetting noiseSetting;
};


float ExecuteBaseImp(float3 inputPos)
{
    float value = 0;
    value = snoise(inputPos);
    value = (value + 1) * 0.5f;
    return value;
}

float ExecuteSinImp(float3 inputPos)
{
    float value = 0;
    value = snoise(inputPos);
    value = abs(1-sin(value));
    value *=value;
    return value;
}

float ExecuteImp(int noiseType ,float3 inputPos)
{
    if(noiseType==1){
        return ExecuteSinImp(inputPos);
    }else{
        return ExecuteBaseImp(inputPos);
    }
}

float3 NoiseBaseGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
    float value = 0;
    float roughness = _noiseSettting.roughness;
    float layerStrength = 1;

    for (int i = 0; i < _noiseSettting.layer; i++)
    {
        float v = ExecuteImp(_noiseSettting.noiseType,normalPos*roughness+_noiseSettting.offset);
        
        value += v*layerStrength;
        layerStrength *= _noiseSettting.layerMultiple;
        roughness *= _noiseSettting.layerRoughness;
    }
    
    value = max(0, value - _noiseSettting.minValue);
    float noise = (value) *_noiseSettting.strength;
    return noise;
}

float3 NoiseRigidGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
    float value = 0;
    float roughness = _noiseSettting.roughness;
    float layerStrength = 1;
    float weigth = 1; 
    for (int i = 0; i < _noiseSettting.layer; i++)
    {
        float v = ExecuteImp(_noiseSettting.noiseType,normalPos*roughness+_noiseSettting.offset);
        v *= weigth;
        weigth = v;
        
        value += v*layerStrength;
        layerStrength *= _noiseSettting.layerMultiple;
        roughness *= _noiseSettting.layerRoughness;
    }
    
    value = max(0, value - _noiseSettting.minValue);
    float noise = (value) *_noiseSettting.strength;
    return noise;
}
     

float3 NoiseGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
    if(_noiseSettting.noiseType==1){
        return NoiseRigidGenerateExecute(_noiseSettting,normalPos);
    }else{
        return NoiseBaseGenerateExecute(_noiseSettting,normalPos);
    }
}
#endif