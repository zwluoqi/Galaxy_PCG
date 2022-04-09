using UnityEngine;

namespace Planet
{
    public class TerrainGenerate
    {
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
        }

        public void Init(MeshFilter[] meshFilterss)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].Init(meshFilterss[i], faceNormal[i]);
            }
        }

        public void UpdateMesh(int resolution, VertexGenerate vertexGenerate, GPUShapeGenerate gpuShapeGenerate, ColorGenerate colorGenerate)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].Update(resolution,vertexGenerate,gpuShapeGenerate);
            }
            UpdateColor(colorGenerate);
        }
        
        public void UpdateShape(VertexGenerate vertexGenerate, GPUShapeGenerate gpuShapeGenerate)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateShape(vertexGenerate,gpuShapeGenerate);
            }
        }
        
        public void UpdateColor(ColorGenerate colorGenerate)
        {
            if (sharedMaterial == null)
            {
                sharedMaterial = Object.Instantiate( colorGenerate.ColorSettting.material);
            }
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMaterial(sharedMaterial);
            }

            sharedMaterial.color = colorGenerate.Execute();
            sharedMaterial.mainTexture = colorGenerate.GenerateTexture2D();
            
            MinMax top = new MinMax();
            MinMax depth = new MinMax();
            for (int i = 0; i < 6; i++)
            {
                top.AddValue(faceGenerates[i].top);
                depth.AddValue(faceGenerates[i].depth);
            }
            sharedMaterial.SetVector("_minmax",new Vector4(depth.min,depth.max,0,0));
        }

        
    }
}