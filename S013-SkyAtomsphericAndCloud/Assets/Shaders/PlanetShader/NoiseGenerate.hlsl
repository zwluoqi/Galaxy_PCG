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
    float minValue ;
    int noiseType;
    
    
    float lfPerturbFeatures;
    float lfSharpness;
    float lfAltitudeErosion;
    float lfRidgeErosion;
    float lfSlopeErosion;
    float lfLacunarity;
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
    value = 1-abs(sin(value));
    value *=value;
    return value;
}

float3 ExecuteErosionImp(float3 inputPos)
{
    float3 value;
    value = snoise_grad(inputPos).xyz;
    //value = (value + float3(1,1,1)) * 0.5f;
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

float2 NoiseBaseGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
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
    
    float source =  value - _noiseSettting.minValue;
    value = max(0, source);
    float noise = (value) *_noiseSettting.strength;
    source *= _noiseSettting.strength;
    return float2(noise,source);
}

float2 NoiseRigidGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
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
    
    float source =  value - _noiseSettting.minValue;
    value = max(0, source);
    float noise = (value) *_noiseSettting.strength;
    source *= _noiseSettting.strength;
    return float2(noise,source);
}


float2 NoiseErosionGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
    float value = 0;
    float roughness = _noiseSettting.roughness;
    float layerStrength = 1;
    float2 dsum = 0;
    for (int i = 0; i < _noiseSettting.layer; i++)
    {
        float3 v = ExecuteErosionImp(normalPos*roughness+_noiseSettting.offset);               
        dsum += v.xz;
        
        value += v.x/(1+dot(dsum,dsum)) *layerStrength;
        layerStrength *= _noiseSettting.layerMultiple;
        roughness *= _noiseSettting.layerRoughness;
    }
    
    float source =  value - _noiseSettting.minValue;
    value = max(0, source);
    float noise = (value) *_noiseSettting.strength;
    source *= _noiseSettting.strength;
    return float2(noise,source);
}



float2 NoiseUberGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
    float value = 0;
    float roughness = _noiseSettting.roughness;
    float layerStrength = 1;
    float layerGain = 1;    
    float dampedLayerStrength = 0;
    float2 lfSlopeErosionSum = 0;
    float2 lfRidgeErosionSum = 0;    
    float3 lfPerturbDerivativeSum =0;

    for (int i = 0; i < _noiseSettting.layer; i++)
    {
        float3 v = ExecuteErosionImp(normalPos*roughness+_noiseSettting.offset + lfPerturbDerivativeSum);                       
        float lfFeatureNoise = v.x;
        
        //shapeness
        float lfRidgeNoise = (1-abs(lfFeatureNoise));
        float lfBillowNoise = lfFeatureNoise*lfFeatureNoise;
        lfFeatureNoise = lerp(lfFeatureNoise,lfBillowNoise,max(0.0,_noiseSettting.lfSharpness));
        lfFeatureNoise = lerp(lfFeatureNoise,lfRidgeNoise,abs(min(0.0,_noiseSettting.lfSharpness)));
        
        //slope erosion
        lfSlopeErosionSum += v.xz*_noiseSettting.lfSlopeErosion;
        
        //ridge erosion
        lfRidgeErosionSum += v.xz*_noiseSettting.lfRidgeErosion;
        
        //perturb
        lfPerturbDerivativeSum += float3(v.xz*_noiseSettting.lfPerturbFeatures,0);
                  
        value += layerStrength * lfFeatureNoise/(1+dot(lfSlopeErosionSum,lfSlopeErosionSum));
        value += dampedLayerStrength * lfFeatureNoise/(1+dot(lfSlopeErosionSum,lfSlopeErosionSum));
        
                                              
        layerStrength *= lerp(layerGain,layerGain*smoothstep(0.0,1.0,value),_noiseSettting.lfAltitudeErosion);   
        dampedLayerStrength = layerStrength*(1-(1-_noiseSettting.lfRidgeErosion/ (1.0f+dot(lfRidgeErosionSum,lfRidgeErosionSum)) ));
                            
        layerGain *= _noiseSettting.layerMultiple;                            
        roughness *= _noiseSettting.layerRoughness;        
    }
    
    float source =  value - _noiseSettting.minValue;
    value = max(0, source);
    float noise = (value) *_noiseSettting.strength;
    source *= _noiseSettting.strength;
    return float2(noise,source);
}         

float2 NoiseGenerateExecute(NoiseSetting _noiseSettting, float3 normalPos){
    if(_noiseSettting.noiseType==3){
        return NoiseUberGenerateExecute(_noiseSettting,normalPos);
    }
    else if(_noiseSettting.noiseType==2){
        return NoiseErosionGenerateExecute(_noiseSettting,normalPos);
    }
    else if(_noiseSettting.noiseType==1){
        return NoiseRigidGenerateExecute(_noiseSettting,normalPos);
    }else{
        return NoiseBaseGenerateExecute(_noiseSettting,normalPos);
    }
}


float2 NoiseLayerExcute( float3 normalPos,NoiseLayer _noiseLayer,StructuredBuffer<NoiseLayer> noiseLayerSettings,int count){

    float2 noise = 0;
    float2 baseNoise = 0;
    if (_noiseLayer.enable > 0)
    {
        noise = NoiseGenerateExecute(_noiseLayer.noiseSetting,normalPos);
        baseNoise = noise;
    }
    
    for (int i = 0; i < count; i++)
    {
        if (noiseLayerSettings[i].enable > 0)
        {
            float2 mask = lerp(float2(1,1),baseNoise,noiseLayerSettings[i].useMask);
            noise +=  NoiseGenerateExecute(noiseLayerSettings[i].noiseSetting,normalPos)*mask;
        }
    }
    return noise;
}

#endif