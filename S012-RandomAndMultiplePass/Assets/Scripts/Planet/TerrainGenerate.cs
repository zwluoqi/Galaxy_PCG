using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class TerrainGenerate:System.IDisposable
    {
        private GPUShapeGenerate gpuShapeGenerate = new GPUShapeGenerate();
        private Texture2D mainTex;
        private Texture2D oceanTex;
        private Material sharedMaterial;

        private FaceGenerate[] faceGenerates;

        readonly Vector3[] faceNormal = {Vector3.up,Vector3.forward, Vector3.left, Vector3.back, Vector3.right,  Vector3.down};
        private VertexGenerate vertexGenerate;
        private ColorGenerate colorGenerate;


        public TerrainGenerate(VertexGenerate _vertexGenerate,ColorGenerate _colorGenerate)
        {
            this.vertexGenerate = _vertexGenerate;
            this.colorGenerate = _colorGenerate;
            faceGenerates = new FaceGenerate[6];
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i] = new FaceGenerate(faceNormal[i]);
            }
        }

        public void UpdateMeshFilter(MeshFilter[] meshFilterss,int resolution)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMeshFilter(meshFilterss[i],resolution);
            }
        }
        
        public bool UpdateLod(Vector3 cameraPos)
        {
#if UNITY_EDITOR
            int nearestIndex = -1;
            int farestIndex = -1;
            float nearestDistance = float.MaxValue;
            float farestDistance = float.MinValue;
                
            for (int i = 0; i < 6; i++)
            {
                var pos = faceGenerates[i].GetPlanePos(vertexGenerate);
                var dir = cameraPos - pos;
                if (dir.magnitude < nearestDistance)
                {
                    nearestDistance = dir.magnitude;
                    nearestIndex = i;
                }

                if (dir.magnitude > farestDistance)
                {
                    farestDistance = dir.magnitude;
                    farestIndex = i;
                }
            }

            bool refreshShape = false;
            for (int i = 0; i < 6; i++)
            {
                if (nearestIndex == i)
                {
                    refreshShape |=faceGenerates[i].UpdateLODLevel(0);
                }
                else if (farestIndex == i)
                {
                    refreshShape |=faceGenerates[i].UpdateLODLevel(4);
                }
                else
                {
                    refreshShape |=faceGenerates[i].UpdateLODLevel(2);
                }
            }

            return refreshShape;
#endif
        }

        public void UpdateMesh(int resolution ,PlanetSettingData planetSettingData)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].Update(resolution,vertexGenerate,planetSettingData,gpuShapeGenerate);
            }
            UpdateColor(planetSettingData);
        }
        
        public void UpdateShape( PlanetSettingData planetSettingData)
        {
            System.DateTime start = System.DateTime.Now;
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateShape(vertexGenerate,planetSettingData,gpuShapeGenerate);
            }

            var start2 = System.DateTime.Now;
            var spaceShape = start2 - start;
            Debug.LogWarning("shape:"+spaceShape.TotalMilliseconds+"ms");
            UpdateColor(planetSettingData);
            var spaceColor = System.DateTime.Now - start2;
            Debug.LogWarning("color:"+spaceColor.TotalMilliseconds+"ms");
        }
        
        public void UpdateColor( PlanetSettingData planetSettingData)
        {
            // Material sharedMaterial;
            InitShareMaterial(planetSettingData);
            colorGenerate.GenerateTexture2D(ref mainTex);
            colorGenerate.GenerateOceanTexture2D(ref oceanTex);
            Debug.LogWarning("Start UpdateColor:"+colorGenerate.colorSettting.LatitudeSettings.Length);
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].FormatHeight(planetSettingData,colorGenerate,gpuShapeGenerate);
            }
            Debug.LogWarning("End UpdateColor:"+colorGenerate.colorSettting.LatitudeSettings.Length);

            UpdateMaterialProperty(planetSettingData);
        }

        private void InitShareMaterial(PlanetSettingData planetSettingData)
        {
            if (sharedMaterial == null)
            {
                sharedMaterial = Object.Instantiate(colorGenerate.colorSettting.material);
            }

            if (sharedMaterial.name != colorGenerate.colorSettting.material.name)
            {
                Object.DestroyImmediate(sharedMaterial);
                sharedMaterial = Object.Instantiate(colorGenerate.colorSettting.material);
            }
            
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMaterial(sharedMaterial);
            }
        }

        public void UpdateMaterialProperty( PlanetSettingData planetSettingData)
        {
            #if UNITY_EDITOR
            InitShareMaterial(planetSettingData);
            sharedMaterial.color = colorGenerate.colorSettting.tinyColor;
            sharedMaterial.mainTexture = mainTex;
            sharedMaterial.SetTexture("_OceanMap",oceanTex);
            sharedMaterial.SetFloat("radius",planetSettingData.radius);
            
            MinMax depth = new MinMax();
            for (int i = 0; i < 6; i++)
            {
                depth.AddValue(faceGenerates[i].depth);
            }
            sharedMaterial.SetVector("_minmax",new Vector4(depth.min,depth.max,0,0));

            UpdateWaterRender();
            #endif
        }
        


        public void UpdateWaterRender()
        {
            if (colorGenerate.waterRenderSetting.waterLayers.Length != 0)
            {
                sharedMaterial.SetVectorArray("waves",colorGenerate.waterRenderSetting.ToWaveVec4s());    
            }
            sharedMaterial.SetInt("waveLen",colorGenerate.waterRenderSetting.waterLayers.Length);
            sharedMaterial.SetFloat("_alphaMultiplier",colorGenerate.waterRenderSetting.alphaMultiplier);
            sharedMaterial.SetFloat("_waterSmoothness",colorGenerate.waterRenderSetting.waterSmoothness);
        }
        
        
        
        public void Dispose()
        {
            gpuShapeGenerate.Dispose();
            Object.Destroy(mainTex);
            Object.Destroy(oceanTex);
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