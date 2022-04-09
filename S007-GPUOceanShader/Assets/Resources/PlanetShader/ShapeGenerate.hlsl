
#ifndef SHAPE_GENERATE_INCLUDED
#define SHAPE_GENERATE_INCLUDED

#include "NoiseGenerate.hlsl"


struct ShapeSettting{
    float radius;
    NoiseLayer noiseLayer;       
};



float3 ShapeGenerateExeculate(ShapeSettting shapeSettting, float3 normalPos,StructuredBuffer<NoiseLayer> noiseLayerSettings,int count,out float2 noiseValue){
    
    noiseValue = NoiseLayerExcute(normalPos,shapeSettting.noiseLayer,noiseLayerSettings,count);
    
    return normalPos*(1+noiseValue.x)*shapeSettting.radius;
}

#endif