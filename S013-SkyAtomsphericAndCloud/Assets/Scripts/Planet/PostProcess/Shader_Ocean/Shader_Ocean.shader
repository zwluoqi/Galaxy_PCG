
Shader "Shader/Shader_Ocean"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        waveNormalA ("waveNormalA", 2D) = "white" {}
        waveNormalB ("waveNormalA", 2D) = "white" {}
    }
    
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Assets/ShaderLabs/Shaders/RayMarchingIntersection.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    // #include "Water.hlsl"
    #include "Triplanar.hlsl"

            sampler2D _MainTex;
            sampler2D waveNormalA;
            sampler2D waveNormalB;

            CBUFFER_START(UnityPerMaterial) // Required to be compatible with SRP Batcher
            float4 _MainTex_ST;
            float3 centerPos;
            float radius;
            float _alphaMultiplier;
            float _colorMultiplier;
            float _fogMultiplier;
    
            float4 depthColor;
            float4 surfaceColor;
            float _waterSmoothness;
            float waveLen;
            float4 waves[12];
            CBUFFER_END
            
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

            float4 GetOceanColor(float oceanViewDepth,float3 oceanWorldPos,float3 normalWS)
            {
                Light mainLight = GetMainLight();
                float alpha=1;
                float distToOcean = length(_WorldSpaceCameraPos.xyz - oceanWorldPos);
                float3 color;
                half depthAlpha = 1 - exp(-oceanViewDepth/radius *_alphaMultiplier);
                alpha *= depthAlpha;
                float opticalDepth01 = 1-exp(-(oceanViewDepth/radius)*_colorMultiplier);
                //diffuse
                float3 oceanToCenter = normalize(oceanWorldPos-centerPos);
                float diffuseLighting = saturate(0.5*dot(oceanToCenter, mainLight.direction)+0.5);
                color = lerp(surfaceColor,depthColor,opticalDepth01)*mainLight.color*pow(diffuseLighting,0.8);

                //specular
                half3 viewDirWS = _WorldSpaceCameraPos.xyz - oceanWorldPos;
                half3 oceanNormal = normalize(normalWS);
                
                half3 halfVec = normalize(normalize(viewDirWS) + normalize(mainLight.direction));
                half specularAngle = acos(dot(oceanNormal,halfVec));
                half specularExponent = specularAngle/_waterSmoothness;
                half kspec = exp(-specularExponent*specularExponent);
                color += kspec;
                
                return float4(color,alpha);
            }

                
            half4 FragBlurH(Varyings input) : SV_Target
            {
              float4 ndcPos = (input.screenPos / input.screenPos.w);
              float deviceDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, ndcPos.xy);                
              float3 colorWorldPos = ComputeWorldSpacePosition(ndcPos.xy, deviceDepth, UNITY_MATRIX_I_VP);

              float3 dirCameraToCenter = centerPos -  _WorldSpaceCameraPos.xyz;
              float distCameraToCenter = length(dirCameraToCenter);


                float3 cameraToPosDir = colorWorldPos - _WorldSpaceCameraPos.xyz;
                        float3 rayDir = normalize(cameraToPosDir);      
                        float2 distToSphere = RaySphereIntersection(centerPos,radius,_WorldSpaceCameraPos.xyz,rayDir);

                
                if(distCameraToCenter<radius)
                {
                    //水下
                    // TODO
                    float3 posToCameraDir = colorWorldPos - _WorldSpaceCameraPos.xyz;
                    float dist = length(posToCameraDir);
                    float viewDepth = min(dist,distToSphere.y);
                    float distFog = viewDepth/radius;
                    distFog = 1-exp(-distFog*_fogMultiplier);
                    // float fog = dist/200;//可见距离
                    return float4(surfaceColor.rgb,distFog);
                }else{
                        
                      //撞到球distToSphere.y>0,
                      //如果length(worldDir)-distToSphere.x>0海洋,否则,陆地
                      
                      float oceanViewDepth = min(distToSphere.y,length(cameraToPosDir)-distToSphere.x);
                      if(oceanViewDepth>0)
                      {
                            float3 hitSpherePos = _WorldSpaceCameraPos.xyz+ rayDir*distToSphere.x;
                            half3 normalWS = hitSpherePos - centerPos;
                            normalWS = normalize(normalWS);

                            if(waveLen>0){
                                //https://bgolus.medium.com/normal-mapping-for-a-triplanar-shader-10bf39dca05a
                                float waveSpeed = waves[0].z;
                                float Frenque = waves[0].x;
                                float waveStrength = waves[0].y;
                          	    float2 waveOffsetA = float2(_Time.x * waveSpeed, _Time.x * waveSpeed * 0.8);
					            float2 waveOffsetB = float2(_Time.x * waveSpeed * - 0.8, _Time.x * waveSpeed * -0.3);
                                
					            float3 waveNormal = triplanarNormal(hitSpherePos, normalWS, Frenque / radius, waveOffsetA, waveNormalA);
					            waveNormal = triplanarNormal(hitSpherePos, waveNormal, Frenque / radius, waveOffsetB, waveNormalB);
					            waveNormal = normalize(lerp(normalWS, waveNormal, waveStrength));
                                normalWS = waveNormal;
                            }
                            float4 oceanColor = GetOceanColor(oceanViewDepth,hitSpherePos,normalWS);
                            return float4(oceanColor.rgb,oceanColor.a);
                      }
             }
              
                
              return 0;
            }

    
            
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="TransParent" "Queue" = "TransParent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Off
        
        Pass
        {            
            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment FragBlurH           
            ENDHLSL
        }
    }
}
