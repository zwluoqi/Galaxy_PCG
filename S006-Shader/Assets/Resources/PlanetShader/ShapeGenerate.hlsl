
#ifndef SHAPE_GENERATE_INCLUDED
#define SHAPE_GENERATE_INCLUDED

#include "NoiseGenerate.hlsl"


struct ShapeSettting{
    float radius;
    float _noiseEnable;
    NoiseSetting _noiseSetting;       
};


float3 ShapeGenerateExeculate(ShapeSettting shapeSettting, float3 normalPos){
    float noise = 0;
    float baseNoise = 0;
    if (shapeSettting._noiseEnable)
    {
        noise = NoiseGenerateExecute(shapeSettting._noiseSetting,normalPos);
        baseNoise = noise;
    }
    /*
    for (int i = 0; i < this._addNoiseGenerate.Length; i++)
    {
        if (this.shapeSettting._noiseLayers[i].enable)
        {
            var mask = this.shapeSettting._noiseLayers[i].useMask ? baseNoise : 1;
            noise += this._addNoiseGenerate[i].Execute(normalPos)*mask;
        }
    }
    */
    return (normalPos+noise*normalPos)*shapeSettting.radius;    
}

float3 ShapeGenerateExeculate1(ShapeSettting shapeSettting, float3 normalPos,StructuredBuffer<NoiseLayer> noiseLayerSettings,int count,out float2 noiseValue){
    float2 noise = 0;
    float2 baseNoise = 0;
    if (shapeSettting._noiseEnable)
    {
        noise = NoiseGenerateExecute(shapeSettting._noiseSetting,normalPos);
        baseNoise = noise;
    }
    
    for (int i = 0; i < count; i++)
    {
        if (noiseLayerSettings[i].enable)
        {
            float2 mask = lerp(float2(1,1),baseNoise,noiseLayerSettings[i].useMask);
            noise +=  NoiseGenerateExecute(noiseLayerSettings[i].noiseSetting,normalPos)*mask;
        }
    }
    noiseValue = noise;
    
    return normalPos*(1+noise.x)*shapeSettting.radius;
}

#endif