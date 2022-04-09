using UnityEngine;

namespace Planet
{
    public class TerrainGenerate:System.IDisposable
    {
        private GPUShapeGenerate gpuShapeGenerate;
        private Texture2D texture2D;
        private Material sharedMaterial;

        private FaceGenerate[] faceGenerates;
        readonly Vector3[] faceNormal = {Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down};


        
        public TerrainGenerate()
        {
            faceGenerates = new FaceGenerate[6];
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i] = new FaceGenerate();
            }
            gpuShapeGenerate = new GPUShapeGenerate();
        }

        public void Init(MeshFilter[] meshFilterss)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].Init(meshFilterss[i],faceNormal[i]);
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
            if (sharedMaterial == null)
            {
                if (planetSettingData.ocean)
                {
                    sharedMaterial = Object.Instantiate(colorGenerate.ColorSettting.oceanMaterial);
                }
                else
                {
                    sharedMaterial = Object.Instantiate(colorGenerate.ColorSettting.material);
                }
            }

            sharedMaterial.color = colorGenerate.Execute();
            colorGenerate.GenerateTexture2D(ref texture2D,planetSettingData);
            sharedMaterial.mainTexture = texture2D;
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMaterial(sharedMaterial);
                faceGenerates[i].FormatHeight(planetSettingData,gpuShapeGenerate,colorGenerate);
            }
            // MinMax objectHeight = new MinMax();
            MinMax depth = new MinMax();
            for (int i = 0; i < 6; i++)
            {
                // objectHeight.AddValue(faceGenerates[i].objectHeight);
                depth.AddValue(faceGenerates[i].depth);
            }
            sharedMaterial.SetVector("_minmax",new Vector4(depth.min,depth.max,0,0));
        }


        public void Dispose()
        {
            gpuShapeGenerate?.Dispose();
            Object.Destroy(texture2D);
        }
    }
}