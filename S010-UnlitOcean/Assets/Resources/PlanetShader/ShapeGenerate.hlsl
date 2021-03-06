
#ifndef SHAPE_GENERATE_INCLUDED
#define SHAPE_GENERATE_INCLUDED

#include "NoiseGenerate.hlsl"


struct ShapeSettting{
    float radius;
    NoiseLayer noiseLayer;       
};



float3 ShapeGenerateExeculate(ShapeSettting shapeSettting, float3 normalPos,StructuredBuffer<NoiseLayer> noiseLayerSettings,int count,float ocean,out float2 noiseValue){
    
    noiseValue = NoiseLayerExcute(normalPos,shapeSettting.noiseLayer,noiseLayerSettings,count);
    float mainland = 1-ocean;
    return normalPos*(1+noiseValue.y*mainland)*shapeSettting.radius;
}

#endif