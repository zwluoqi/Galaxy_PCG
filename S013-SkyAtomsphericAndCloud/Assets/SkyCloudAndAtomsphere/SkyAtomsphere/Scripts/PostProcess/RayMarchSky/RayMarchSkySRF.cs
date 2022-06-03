using System;
using Clouds;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

public class RayMarchSkySRF : ScriptableRendererFeature
{
    class RayMarchSkyPass : ScriptableRenderPass
    {

        public static string k_RenderTag = "RayMarchSky";
        static string shaderName = "Shader/RayMarchSky";

        private RenderTargetIdentifier _renderTargetIdentifier;
        private Material _material;

        private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        private static readonly int TmpTexId = Shader.PropertyToID("_SkyTex");
        private static readonly int TmpTexId2 = Shader.PropertyToID("_SkyTex2");
        

        public RayMarchSkyPass()
        {
            
        }
        
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {

        }

        public void SetUp(RenderTargetIdentifier targetIdentifier)
        {
            this._renderTargetIdentifier = targetIdentifier;
            
            //需要存储法线
            ConfigureInput(ScriptableRenderPassInput.Normal|ScriptableRenderPassInput.Depth|ScriptableRenderPassInput.Color);

        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            SkySphere[] skySpheres = Object.FindObjectsOfType<SkySphere>();
            if (skySpheres.Length == 0)
            {
                return;
            }
            
            
            if (_material == null)
            {
                CreateMaterial(shaderName);
            }

            var cmd = CommandBufferPool.Get(k_RenderTag);

            var w = renderingData.cameraData.camera.scaledPixelWidth;
            var h = renderingData.cameraData.camera.scaledPixelHeight;
            
            var soruce = _renderTargetIdentifier;
            cmd.GetTemporaryRT(TmpTexId,w/2,h/2,0,FilterMode.Point, RenderTextureFormat.Default);
            cmd.GetTemporaryRT(TmpTexId2,w,h,0,FilterMode.Point, RenderTextureFormat.Default);
            // cmd.GetTemporaryRT(TmpTexId3,w,h,0,FilterMode.Point, RenderTextureFormat.Default);
            
            for (int i = 0; i < skySpheres.Length; i++)
            {
                var sky = skySpheres[i];
                if (!sky.enabled)
                {
                    return;
                }
                var transform = sky.transform;
                Vector3 boxcenter = transform.position;
                var localScale = transform.localScale;


                cmd.SetGlobalFloat("atomDensityFalloff",sky.atomDensityFalloff);
                cmd.SetGlobalFloat("lightPhaseValue",sky.lightPhaseValue);
                
                cmd.SetGlobalInt("numberStepSky",sky.numberStepSky);
                
                
                cmd.SetGlobalFloat("lightAbsorptionTowardSun",sky.lightAbsorptionTowardSun);
                cmd.SetGlobalFloat("darknessThreshold",sky.darknessThreshold);
                cmd.SetGlobalInt("numberStepLight",sky.numberStepLight);
                
                
                float raidu0 = localScale.x*0.5f;
                float raidu1 = raidu0 * 1.2f*sky.atomScale;
                    
                cmd.SetGlobalFloat("radiusTerrain", raidu0);
                cmd.SetGlobalFloat("radiusAtoms", raidu1);
                cmd.SetGlobalVector("sphereCenter", boxcenter);

                
                var waveRGBScatteringCoefficients = math.pow((new float3(400.0f))/sky.rgbWaveLengths, 4);
                cmd.SetGlobalVector("waveRGBScatteringCoefficients", new Vector4(waveRGBScatteringCoefficients.x,
                    waveRGBScatteringCoefficients.y,
                    waveRGBScatteringCoefficients.z,
                    1));
                cmd.SetGlobalFloat("sunSmoothness",sky.sunSmoothness);
                

                cmd.Blit(null,TmpTexId,_material,0);
                cmd.SetGlobalTexture(MainTexId,soruce);
                cmd.SetGlobalTexture("_SkyTex",TmpTexId);
                cmd.Blit(soruce,TmpTexId2,_material,1);
                cmd.Blit(TmpTexId2, soruce);
            }
            context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);

        }

        private void EnableDebugShapeKeyWord(CommandBuffer cmd, string debugShapeNose)
        {
            cmd.DisableShaderKeyword("DEBUG_SHAPE_NOSE");
            cmd.DisableShaderKeyword("DEBUG_DETAIL_NOSE");
            if (!string.IsNullOrEmpty(debugShapeNose))
            {
                cmd.EnableShaderKeyword(debugShapeNose);    
            }
            
        }

        private void EnableShapeKeyWord(CommandBuffer cmd,string shapeBox)
        {
            cmd.DisableShaderKeyword("SHAPE_BOX");
            cmd.DisableShaderKeyword("SHAPE_SPHERE");
            cmd.EnableShaderKeyword(shapeBox);
        }


        private void CreateMaterial(string shaderName)
        {
            if (_material != null)
            {
                CoreUtils.Destroy(_material);
            }
        
            var shader = Shader.Find(shaderName);
            _material = CoreUtils.CreateEngineMaterial(shader);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    RayMarchSkyPass _mScriptableCloudPass;

    /// <inheritdoc/>
    public override void Create()
    {
        _mScriptableCloudPass = new RayMarchSkyPass();

        // Configures where the render pass should be injected.
        _mScriptableCloudPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents - 20;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        _mScriptableCloudPass.SetUp(renderer.cameraColorTarget);
        renderer.EnqueuePass(_mScriptableCloudPass);
    }
}


