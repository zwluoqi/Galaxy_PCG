#ifndef UNIVERSAL_CUSTOM_MULTIPLE_UNLIT_PASS_INCLUDED
#define UNIVERSAL_CUSTOM_MULTIPLE_UNLIT_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

//https://developer.nvidia.com/sites/all/modules/custom/gpugems/books/GPUGems/gpugems_ch01.html

struct Attributes
{
    float4 positionOS       : POSITION;
    //float3 normalOS       : NORMAL;
    //float4 tangentOS       : TANGENT;
    float2 uv               : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv        : TEXCOORD0;
    float4 fogFactorAndVertexLight  : TEXCOORD1;
    float3 normalWS  : TEXCOORD2;
    float3 viewDirWS  : TEXCOORD3;
    float3 tangentWS  : TEXCOORD4;   
    float3 normalOS  : TEXCOORD5;
    float4 vertex : SV_POSITION;
    float4 oceanData : COLOR;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

float invLerp(float from, float to, float value){
    return clamp((value - from) / (to - from),0,1);
}

#define SPEEDTREE_PI 3.14159265359

float2 GetDirection(float angle){
    float ddx = cos(angle);
    float ddy = sin(angle);
    return float2(ddx,ddy);
}

float GetWavePosition(float3 tangentPos,float4 wave,out float3 normal,out float3 tangent){
    float t = _Time.y;
    
    float W = SPEEDTREE_PI * 2.0 / wave.x / radius;
    float A = wave.y;
    float F = wave.z * W * radius;    
    float angle = SPEEDTREE_PI * wave.w /180.0;
    float2 Di = GetDirection(angle);
    
    float2 D = Di * tangentPos.xy;
    float Dvalue = Di.x* tangentPos.x+Di.y*tangentPos.y;
    
    float H = radius*A*sin(Dvalue*W+F*t);
    float dx = radius*W*Di.x*A*cos(Dvalue*W+F*t);
    float dy = radius*W*Di.y*A*cos(Dvalue*W+F*t);
    tangent = float3(1,0,-dy);
    normal = float3(-dx,-dy,1);
    return H;
}

float3 MultipleWavePositoin(float4x4 vertexTangentToObj,float3 pos,out float3 normal,out float3 tangent){
    float3 newPos = pos;
    normal = 0;
    tangent = 0;

    float3 tmpNormal;
    float3 tmpTangent;
    
    float tmpOffset;
    for(int i=0;i<waveLen;i+=1){
        tmpOffset = GetWavePosition(pos,waves[i],tmpNormal,tmpTangent);
        
        half3 normalOS = mul(vertexTangentToObj,float4(tmpNormal,.0)).xyz;
        half3 tangentOS = mul(vertexTangentToObj,float4(tmpTangent,.0)).xyz;   
        half3 offsetPos = normalOS*tmpOffset;
    
        newPos += offsetPos;
        normal += normalOS;
        tangent += tangentOS;
    }
    normal = normalize(normal);
    tangent = normalize(tangent);
    
    return newPos;
}



Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);



    float3 input_positionOS = (radius)*normalize(input.positionOS.xyz);
    float3 input_normalOS = normalize(input_positionOS);
    float4 input_tangentOS = float4(cross(input_normalOS,float3(0,1,0)),-1.0f);

    real sign = input_tangentOS.w * GetOddNegativeScale();     
    
    half crossSign = (sign > 0.0 ? 1.0 : -1.0); // we do not need to multiple GetOddNegativeScale() here, as it is done in vertex shader
    half3 bitang = crossSign * cross(normalize(input_normalOS.xyz), normalize(input_tangentOS.xyz));
    half4x4 vertexTangentToObj = half4x4(half4(input_tangentOS.x,bitang.x,input_normalOS.x,0),
                                    half4(input_tangentOS.y,bitang.y,input_normalOS.y,0),
                                    half4(input_tangentOS.z,bitang.z,input_normalOS.z,0),
                                    half4(0,0,0,1)
                                    );

    half3 normalOS = input_normalOS;
    half4 tangentOS = input_tangentOS;
    float3 positionOS = input_positionOS;
    
    if(waveLen>0){
        half3 tangentOS3;
        positionOS = MultipleWavePositoin(vertexTangentToObj,input_positionOS.xyz,normalOS,tangentOS3);
        tangentOS = half4(tangentOS3,input_tangentOS.w);
    }
    
    //positionOS = (length(positionOS)+2)*normalize(positionOS);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS.xyz);
    output.vertex = vertexInput.positionCS;

    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex lighting and SH evaluation
    VertexNormalInputs normalInput = GetVertexNormalInputs(normalOS, tangentOS);

    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);   
    
    float2 sourceUV = TRANSFORM_TEX(input.uv, _OceanMap);       
    float oceanDepth = invLerp(min(_minmax.x,-0.01f),0,sourceUV.y);

    output.uv.x = oceanDepth;
    output.uv.y = 0.5f;
    //smoothness
    //output.color.x = (1.0f-floor(ocean));
    output.oceanData.x = 1.0-oceanDepth;
    
    float fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
    output.fogFactorAndVertexLight = float4(fogCoord,vertexLight);
    output.viewDirWS = viewDirWS;
    output.normalWS = normalInput.normalWS;
    output.tangentWS = normalInput.tangentWS;
    // GetWavePosition(input_positionOS.xyz,waves[0],normalOS,tangentOS.xyz);
    output.normalOS = normalOS;//input_normalOS;//normalTS;//mul(vertexObjToTangent,input_normalOS);

    
    return output;
}

half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    // return float4(0.5*(1.0+input.normalOS.xyz),1);
    half2 uv = input.uv;
    half4 texColor = SAMPLE_TEXTURE2D(_OceanMap,sampler_OceanMap, uv);
    half3 color = texColor.rgb * _OceanColor.rgb;
    half alpha = texColor.a * _OceanColor.a;
    AlphaDiscard(alpha, _Cutoff);
    
    half oceanDepth = input.oceanData.x;
    half depthAlpha = 1 - exp(-oceanDepth*_alphaMultiplier)+0.01f;
    alpha *= depthAlpha;
    
    half3 oceanNormal = normalize(input.normalWS);
    Light mainLight = GetMainLight();
    
    half3 halfVec = normalize(normalize(input.viewDirWS) + normalize(mainLight.direction));
    half specularAngle = acos(dot(oceanNormal,halfVec));
    half specularExponent = specularAngle/_waterSmoothness;
    half kspec = exp(-specularExponent*specularExponent);
    color += kspec;
    //return float4 (kspec,0,0,1);
    
#ifdef _ALPHAPREMULTIPLY_ON
    //color *= alpha;
#endif

    //color = MixFog(color, input.fogCoord);
    //alpha = OutputAlpha(alpha, _Surface);

    return half4(color, alpha);
}
#endif