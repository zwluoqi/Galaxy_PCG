#ifndef UNIVERSAL_CUSTOM_UNLIT_PASS_INCLUDED
#define UNIVERSAL_CUSTOM_UNLIT_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


struct Attributes
{
    float4 positionOS       : POSITION;
    float3 normalOS       : NORMAL;
    float4 tangentOS       : TANGENT;
    float2 uv               : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv        : TEXCOORD0;
    float4 fogFactorAndVertexLight  : TEXCOORD1;
    float3 normalWS  : TEXCOORD2;
    float3 viewDirWS  : TEXCOORD3;
    float4 vertex : SV_POSITION;
    float4 oceanData : COLOR;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

float invLerp(float from, float to, float value){
    return clamp((value - from) / (to - from),0,1);
}


// float4 GetWaveColor(float2 tex0){
//     //get and uncompress the flow vector for this pixel
//     //float2 flowmap = SAMPLE_TEXTURE2D( FlowMapS,sampler_FlowMapS, tex0 ).rg * 2.0f - 1.0f;
//     float s = 0;
//     float2 flowmap = SAMPLE_TEXTURE2D( FlowMapS,sampler_FlowMapS, tex0 ).rg * 2.0f - 1.0f;               
//     float cycleOffset = SAMPLE_TEXTURE2D( NoiseMapS,sampler_NoiseMapS ,tex0 ).r;
//     
//     float FlowMapOffset0 = (_Time.y*_speed);
//     float FlowMapOffset1 = (_Time.y*_speed + .5f);
//     
//     float phase0 = frac(cycleOffset * .5f + FlowMapOffset0);
//     float phase1 = frac(cycleOffset * .5f + FlowMapOffset1);
//     
//     
//     float2 uv0 = ( tex0  ) + flowmap * phase0 ;
//     float2 uv1 =  ( tex0  ) + flowmap * phase1 ;
//     
//     // Sample normal map.
//     float4 normalT0 = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap, frac(uv0));
//     float4 normalT1 = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap, frac(uv1) );
//     
//     float HalfCycle = cycleOffset*.5f;
//     float flowLerp = ( abs( HalfCycle - phase0 ) / HalfCycle );
//     float4 offset = lerp( normalT0, normalT1, flowLerp );
//     return float4(normalT1.xyz,1);
// }

Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.vertex = vertexInput.positionCS;

    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex lighting and SH evaluation
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    

    
    float2 sourceUV = TRANSFORM_TEX(input.uv, _BaseMap);
    
    
    
    float oceanDepth = invLerp(_minmax.x,0,sourceUV.y);
    //float top = invLerp(0,_minmax.y,sourceUV.y);
    //float x = 0.5*ocean+0.5*top;
    output.uv.x = oceanDepth;
    output.uv.y = sourceUV.x;
    //smoothness
    //output.color.x = (1.0f-floor(ocean));
    output.oceanData.x = 1.0-oceanDepth;
    
    float fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
    output.fogFactorAndVertexLight = float4(fogCoord,vertexLight);
    output.viewDirWS = viewDirWS;
    output.normalWS = normalInput.normalWS;

    
    return output;
}

half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    half2 uv = input.uv;
    half4 texColor = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap, uv);
    half3 color = texColor.rgb * _BaseColor.rgb;
    half alpha = texColor.a * _BaseColor.a;
    AlphaDiscard(alpha, _Cutoff);
    
    half oceanDepth = input.oceanData.x;
    half depthAlpha = 1 - exp(-oceanDepth*_alphaMultiplier);
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
    alpha = OutputAlpha(alpha, _Surface);

    return half4(color, alpha);
}
#endif