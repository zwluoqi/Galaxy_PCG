
Shader "Shader/RayMarchCloud"
{
    Properties
    {
//        _MainTex ("Texture", 2D) = "white" {}
//        shapeNoise ("shapeNoise", 3D) = "white" {}


//        boxmin("boxmin",vector)=(0,0,0,0)
//        boxmax("boxmax",vector)=(1,1,1,0)        
//
//        samplerScale("samplerScale",float) = 1
//        samplerOffset("samplerOffset",vector) = (0,0,0,0)
//        densityMultipler("densityMultipler",float) = 1
//        densityThreshold("densityThreshold",float) = 1
//        numberStepCloud("numberStepCloud",int) = 1
//        
//        lightPhaseValue("lightPhaseValue",float) = 1
//        lightAbsorptionThroughCloud("lightAbsorptionThroughCloud",float) = 1
//        
//        numberStepLight("numberStepLight",int) = 1
//        lightAbsorptionTowardSun("lightAbsorptionTowardSun",float) = 1
//        darknessThreshold("darknessThreshold",float) = 1
//
//        globalCoverage("globalCoverage",float) = 1
//        debug_shape_z("debug_shape_z",float) = 1
//        debug_rgba("debug_rgba",vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Assets/ShaderLabs/Shaders/RayMarchingIntersection.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Clouds.hlsl"

        #pragma multi_compile _ DEBUG_SHAPE_NOSE DEBUG_DETAIL_NOSE
        #pragma multi_compile SHAPE_BOX SHAPE_SPHERE 
            
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
                
            half4 FragCloud(Varyings input) : SV_Target
            {

                
                float4 ndcPos = (input.screenPos / input.screenPos.w);
                float deviceDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, ndcPos.xy);                
                float3 colorWorldPos = ComputeWorldSpacePosition(ndcPos.xy, deviceDepth, UNITY_MATRIX_I_VP);

                #ifdef DEBUG_SHAPE_NOSE
                    float4 rgba = SAMPLE_TEXTURE3D_LOD(shapeNoise, sampler_shapeNoise, float3(ndcPos.xy,debug_shape_z),0);
                    if (debug_rgba<4){
                        return rgba[debug_rgba];
                    }else
                    {
                        return rgba;
                    }
                #elif DEBUG_DETAIL_NOSE
                    float4 rgba = SAMPLE_TEXTURE3D_LOD(detailNoise, sampler_detailNoise, float3(ndcPos.xy,debug_shape_z),0);
                    if (debug_rgba<4){
                        return rgba[debug_rgba];
                    }else
                    {
                        return rgba;
                    }
                #endif
                
            
                float3 cameraToPosDir = colorWorldPos - _WorldSpaceCameraPos.xyz;
                float3 rayDir = normalize(cameraToPosDir);


                #ifdef SHAPE_BOX
                    float2 distToBox = RayBoxIntersection(_WorldSpaceCameraPos.xyz,rayDir,boxmin,boxmax);
  
                    float distToBoxHit = distToBox.x;
                    float rayDst = min(distToBox.y,length(cameraToPosDir)-distToBoxHit);
                #elif  SHAPE_SPHERE
                    //optimize
                    //float2 radius = GetOptimizeRadius(boxmin.x,boxmax.x);
                    float2 radius = float2(boxmin.x,boxmax.x);
                    float2 distToOuter = RaySphereIntersection(sphereCenter.xyz,radius.y,_WorldSpaceCameraPos.xyz,rayDir);
                    float2 distToInner = RaySphereIntersection(sphereCenter.xyz,radius.x,_WorldSpaceCameraPos.xyz,rayDir);
  
  
                    float distToBoxHit = min(distToOuter.x,distToInner.x);
                    float rayDst = min(distToInner.x- distToOuter.x,distToOuter.y-distToInner.y);
                    rayDst = min(rayDst,length(cameraToPosDir)-distToBoxHit);
                #else
                    float rayDst  = 0;
                    float distToBoxHit = 0;
                #endif

                float3 dirToLight = normalize(_MainLightPosition.xyz);
                float cosTheta = dot(rayDir,dirToLight);
                // return cosTheta;
                float lightPhaseValue = lightPhase(cosTheta)*lightPhaseStrength;
                // return lightPhaseValue;
  
                float totalLightTransmittance = 0;
                float lightEnergy = 0;
                float transmittance = 1;
                float cloudHeight = 1;
                float totalDensity = 0;
                #ifdef SHAPE_BOX
                    cloudHeight = (boxmax.y-boxmin.y);
                #elif SHAPE_SPHERE
                    cloudHeight = (boxmax.x-boxmin.x);
                #endif
                if(rayDst>0.001f){
                    float stepDst = rayDst/numberStepCloud;
                    float4 rayMarchOffset = SAMPLE_TEXTURE2D_LOD(rayMarchOffsetMap, sampler_rayMarchOffsetMap,.1*float2(rayDir.x+rayDir.y,rayDir.y+rayDir.z),0);
                    float startOffset = 1.0*stepDst*rayMarchOffset;
                    rayDst -= startOffset;
                    stepDst = rayDst/numberStepCloud;
                    // return rayMarchOffset;
                    float3 hitPoint = _WorldSpaceCameraPos.xyz + rayDir*(distToBoxHit+startOffset*0.5f);
                    for (int step = 0;step <numberStepCloud;step++)
                    {
                        float3 rayPos = hitPoint + rayDir*(stepDst)*(step);
  
                        float density = sampleDensity(rayPos);
                        // totalDensity+=density;
                        float lightEnergyFactor = 1;
                        if(density > 0 )
                        {
                            float lengthToLightCould = 0;
                            #ifdef SHAPE_BOX
                                  lengthToLightCould = RayBoxIntersection(rayPos,dirToLight,boxmin,boxmax).y;
                            #elif SHAPE_SPHERE
                                  lengthToLightCould = RaySphereIntersection(sphereCenter.xyz,radius.y,rayPos,dirToLight).y;
                            #endif
                            
                        
                            float lightDensity = lightMarchingDensity(rayPos,dirToLight,lengthToLightCould);
                            float lightTransmittance = exp(-lightDensity*lightAbsorptionTowardSun/cloudHeight)*(1-darknessThreshold)+darknessThreshold;
                            totalLightTransmittance += lightTransmittance;
  
                              #ifdef SHAPE_SPHERE
                                  float ditFacotr = lengthToLightCould/(2*radius.x);
                                  lightEnergyFactor = 1-ditFacotr*ditFacotr*ditFacotr;
                              #endif                   
                            
                            lightEnergy += (density * stepDst * transmittance * lightTransmittance*lightPhaseValue)*lightEnergyFactor;
                            transmittance *= (exp(-density*stepDst*lightAbsorptionThroughCloud/cloudHeight));
                            if(transmittance < 0.001)
                            {
                                break;
                            }
                        }
                    }
               }


              float4 cloudCol = lightEnergy*_MainLightColor;

              return float4(cloudCol.rgb,transmittance);
            }

            half4 FragBlend(Varyings input) : SV_Target{
                float4 ndcPos = (input.screenPos / input.screenPos.w);
                float4 backgroundCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,ndcPos.xy);
                float4 cloudCol = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex,ndcPos.xy);
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
            #pragma fragment FragCloud           
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
