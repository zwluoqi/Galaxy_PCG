using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class TerrainGenerate:System.IDisposable
    {
        private GPUShapeGenerate gpuShapeGenerate;
        private Texture2D texture2D;
        private Material sharedMaterial;

        private FaceGenerate[] faceGenerates;

        readonly Vector3[] faceNormal = {Vector3.up,Vector3.forward, Vector3.left, Vector3.back, Vector3.right,  Vector3.down};


        
        public TerrainGenerate()
        {
            faceGenerates = new FaceGenerate[6];
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i] = new FaceGenerate(faceNormal[i]);
            }
            gpuShapeGenerate = new GPUShapeGenerate();
        }

        public void UpdateMeshFilter(MeshFilter[] meshFilterss,int resolution)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMeshFilter(meshFilterss[i],resolution);
            }
        }

        public void UpdateMesh(int resolution, VertexGenerate vertexGenerate, PlanetSettingData planetSettingData, ColorGenerate colorGenerate)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].Update(resolution,vertexGenerate,planetSettingData,gpuShapeGenerate);
            }
            UpdateColor(colorGenerate,planetSettingData);
        }
        
        public void UpdateShape(VertexGenerate vertexGenerate,  PlanetSettingData planetSettingData,ColorGenerate colorGenerate)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateShape(vertexGenerate,planetSettingData,gpuShapeGenerate);
            }
            UpdateColor(colorGenerate,planetSettingData);
        }
        
        public void UpdateColor(ColorGenerate colorGenerate, PlanetSettingData planetSettingData)
        {
            // Material sharedMaterial;
            InitShareMaterial(colorGenerate.ColorSettting,planetSettingData);
            colorGenerate.GenerateTexture2D(ref texture2D,planetSettingData);
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].FormatHeight(planetSettingData,gpuShapeGenerate,colorGenerate);
            }

            UpdateMaterialProperty(colorGenerate.ColorSettting,colorGenerate.WaterRenderSetting,planetSettingData);
        }

        private void InitShareMaterial(ColorSettting colorSettting,PlanetSettingData planetSettingData)
        {
            if (sharedMaterial == null)
            {
                if (planetSettingData.ocean)
                {
                    sharedMaterial = Object.Instantiate(colorSettting.oceanMaterial);
                }
                else
                {
                    sharedMaterial = Object.Instantiate(colorSettting.material);
                }

            }
            
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMaterial(sharedMaterial);
            }
        }

        public void UpdateMaterialProperty(ColorSettting colorSettting,WaterRenderSetting waterRenderSetting, PlanetSettingData planetSettingData)
        {
            #if UNITY_EDITOR
            InitShareMaterial(colorSettting,planetSettingData);
            sharedMaterial.color = colorSettting.tinyColor;
            sharedMaterial.mainTexture = texture2D;
            sharedMaterial.SetFloat("radius",planetSettingData.radius);
            
            MinMax depth = new MinMax();
            for (int i = 0; i < 6; i++)
            {
                depth.AddValue(faceGenerates[i].depth);
            }
            sharedMaterial.SetVector("_minmax",new Vector4(depth.min,depth.max,0,0));

            UpdateWaterRender(waterRenderSetting);
            #endif
        }
        


        public void UpdateWaterRender(WaterRenderSetting waterRenderSettting)
        {
            if (waterRenderSettting.waterLayers.Length != 0)
            {
                sharedMaterial.SetVectorArray("waves",waterRenderSettting.ToWaveVec4s());    
            }
            sharedMaterial.SetInt("waveLen",waterRenderSettting.waterLayers.Length);
            sharedMaterial.SetFloat("_alphaMultiplier",waterRenderSettting.alphaMultiplier);
            sharedMaterial.SetFloat("_waterSmoothness",waterRenderSettting.waterSmoothness);
        }
        
        
        
        public void Dispose()
        {
            gpuShapeGenerate?.Dispose();
            Object.Destroy(texture2D);
        }

        public void OnDrawGizmos(float radius)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].OnDrawGizmos(radius);
            }
        }
    }
}