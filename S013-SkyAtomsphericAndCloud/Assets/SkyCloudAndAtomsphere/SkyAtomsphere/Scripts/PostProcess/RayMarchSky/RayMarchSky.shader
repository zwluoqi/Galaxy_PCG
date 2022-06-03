
Shader "Shader/RayMarchSky"
{
    Properties
    {

    }
    
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Assets/ShaderLabs/Shaders/RayMarchingIntersection.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Sky.hlsl"


            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 screenPos :TEXCOORD1;
            };
            
            Varyings FullscreenVert(Attributes input)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(input.vertex.xyz);
                o.uv = TRANSFORM_TEX(input.uv, _MainTex);
                o.color = input.color;
                               //vertex
                o.screenPos = ComputeScreenPos(o.vertex);//o.vertex是裁剪空间的顶点
                return o;
            }
            
            half4 DepthShow(Varyings input) : SV_Target
            {
              float4 ndcPos = (input.screenPos / input.screenPos.w);

              float deviceDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, ndcPos.xy);                

              float sceneDepth =  LinearEyeDepth(deviceDepth,_ZBufferParams)*0.001;
              return float4(sceneDepth,sceneDepth,sceneDepth,1);
            }
                
            half4 FragSky(Varyings input) : SV_Target
            {

                
              float4 ndcPos = (input.screenPos / input.screenPos.w);
              float deviceDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, ndcPos.xy);                
              float3 colorWorldPos = ComputeWorldSpacePosition(ndcPos.xy, deviceDepth, UNITY_MATRIX_I_VP);
              // return deviceDepth;

             
              float3 cameraToPosDir = colorWorldPos - _WorldSpaceCameraPos.xyz;
              float3 rayDir = normalize(cameraToPosDir);

              float2 distToOuter = RaySphereIntersection(sphereCenter.xyz,radiusAtoms,_WorldSpaceCameraPos.xyz,rayDir);
              float2 distToInner = RaySphereIntersection(sphereCenter.xyz,radiusTerrain,_WorldSpaceCameraPos.xyz,rayDir);


              float distToBoxHit = min(distToOuter.x,distToInner.x);
              float rayDst = min(distToInner.x- distToOuter.x,distToOuter.y-distToInner.y);
              rayDst = min(rayDst,length(cameraToPosDir)-distToBoxHit);

                
                float3 dirToLight = normalize(_MainLightPosition.xyz);
                float cosTheta = dot(rayDir,dirToLight);
                // float lightPhaseValue = lightPhase(cosTheta);
                // float lightPhaseValue= 1;


              float4 lightEnergy = 0;
              float totalDensity = 0;
              float viewTransmit = 1;
              if(rayDst>0.001f){
                  // return rayDst/(radiusAtoms*2);

                  const float thickness = 0.001f;
                  float stepDst = (rayDst-2*thickness)/numberStepSky;
                  
                  float3 hitPoint = _WorldSpaceCameraPos.xyz + rayDir*(distToBoxHit+thickness);
                  float4 viewDensity;
                  for (int step = 0;step <numberStepSky;step++)
                  {
                      float3 rayPos = hitPoint + rayDir*(stepDst)*(step);
                      float density = sampleDensity(rayPos);
                      totalDensity+=density;
                      
                      float2 hitToAtoms = RaySphereIntersection(sphereCenter,radiusAtoms,rayPos,dirToLight);
                      float4 lightDensity = marchingDensity(rayPos,dirToLight,hitToAtoms.y);
                      viewDensity = marchingDensity(rayPos,-rayDir,stepDst*step);                                          
                        //密度越大,穿透率越低
                      float4 transmittance = exp(-(lightDensity+viewDensity)*lightAbsorptionTowardSun*waveRGBScatteringCoefficients/radiusTerrain);

                      lightEnergy += (density * stepDst * waveRGBScatteringCoefficients * transmittance * lightPhaseValue);
                  }
                  lightEnergy /= radiusTerrain;
                  viewTransmit = exp(-viewDensity/radiusTerrain);
                  // return viewTransmit;
               }

              // float4 skyCol = lightEnergy;

             //add sun
             float focusedEyeCos = pow(saturate(cosTheta), sunSmoothness);
             float sun = saturate(hg(focusedEyeCos, .9995)) * viewTransmit*(1-saturate(distToInner.y) ) * step(deviceDepth,0.001);
             // return sun;
              return float4(lightEnergy.xyz*(1-sun)+sun*_MainLightColor,viewTransmit);
            }

            half4 FragBlend(Varyings input) : SV_Target{
                float4 ndcPos = (input.screenPos / input.screenPos.w);
                float4 backgroundCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,ndcPos.xy);
                float4 cloudCol = SAMPLE_TEXTURE2D(_SkyTex, sampler_SkyTex,ndcPos.xy);
                return float4(backgroundCol.xyz*cloudCol.a+cloudCol.xyz,1);
            }
            
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="TransParent" "Queue" = "TransParent"}
        LOD 100

        ZWrite Off
        ZTest LEqual
        Cull Off
        
        Pass
        {        
            Blend One Zero
    
            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment FragSky           
            ENDHLSL
        }

        Pass
        {        
            Blend One Zero
    
            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment FragBlend           
            ENDHLSL
        }
    }
}
