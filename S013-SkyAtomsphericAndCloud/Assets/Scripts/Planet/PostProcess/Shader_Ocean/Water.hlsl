#ifndef QINGZHU_TOOLS_SIMPLE_WATER
#define QINGZHU_TOOLS_SIMPLE_WATER
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float invLerp(float from, float to, float value){
    return clamp((value - from) / (to - from),0,1);
}

#define SPEEDTREE_PI 3.14159265359

float2 GetDirection(float angle){
    float ddx = cos(angle);
    float ddy = sin(angle);
    return float2(ddx,ddy);
}

float GetWavePosition(float3 tangentPos,float4 wave,float radius,out float3 normal,out float3 tangent){
    float t = _Time.y;
    
    float W = SPEEDTREE_PI * 2.0 / wave.x / radius;
    float A = wave.y;
    float F = wave.z * W * radius;    
    float angle = SPEEDTREE_PI * wave.w /180.0;
    float2 Di = GetDirection(angle);
    
    float2 D = Di * tangentPos.xy;
    float Dvalue = Di.x* tangentPos.x+Di.y*tangentPos.y;
    
    float H = radius*A*sin(Dvalue*W+F*t);
    float dx = W*Di.x*A*cos(Dvalue*W+F*t);
    float dy = W*Di.y*A*cos(Dvalue*W+F*t);
    tangent = float3(1,0,dy);
    normal = float3(-dx,-dy,1);
    return H;
}
#endif