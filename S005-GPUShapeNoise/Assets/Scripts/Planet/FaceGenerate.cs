using System;
using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class FaceGenerate
    {
        public MeshFilter MeshFilter;
        public Vector3 Normal;
        public int Resolution;

        private Vector3 axisA;
        private Vector3 axisB;

        private Vector3[] vertices;
        private int[] triangles;
        public FaceGenerate(
            MeshFilter meshFilter,Vector3 normal)
        {
            MeshFilter = meshFilter;
            Normal = normal.normalized;
            //unity 左手坐标系
            axisA = new Vector3(normal.y,normal.z,normal.x);
            axisA = axisA.normalized;
            axisB = Vector3.Cross(normal, axisA);
            axisB = axisB.normalized;
        }

        public void Update(int resolution,ShapeGenerate shapeGenerate,GPUShapeGenerate gpuShapeGenerate,ColorGenerate colorGenerate)
        {
            // var _computeShader = shapeGenerate.ShapeSettting.computeShader;

            if (Resolution != resolution)
            {
                Resolution = resolution;
                vertices = new Vector3[(Resolution ) * (Resolution )];
                var multiple = (Resolution - 1) * (Resolution - 1);
                triangles = new int[multiple*2*3];
            }

            UpdateShape(shapeGenerate,gpuShapeGenerate);
            UpdateColor(colorGenerate);
        }

        public void UpdateShape(ShapeGenerate shapeGenerate,GPUShapeGenerate gpuShapeGenerate)
        {
            if (!MeshFilter.gameObject.activeInHierarchy)
            {
                return;
            }
            var start = System.DateTime.Now;
            if (shapeGenerate.shapeSettting.GPU)
            {
                gpuShapeGenerate.UpdateShape(shapeGenerate,vertices,triangles,Resolution,
                    Normal,axisA,axisB);
            }    
            else
            {

                int indicIndex = 0;
                for (int y = 0; y < Resolution; y++)
                {
                    for (int x = 0; x < Resolution; x++)
                    {
                        var index = x + y * (Resolution);
                        Vector2 percent = new Vector2(x, y) / (Resolution - 1);
                        var pos = Normal + 2 * axisB * (percent.x - 0.5f) + 2 * axisA * (percent.y - 0.5f);
                        vertices[index] = shapeGenerate.Execulate(pos.normalized);
                        // vertices[index] = (2 * axisB * (percent.x - 0.5f) + 2 * axisA * (percent.y - 0.5f))*shapeGenerate.shapeSettting.radius;
                        if (x < Resolution - 1 && y < Resolution - 1)
                        {
                            
                            //逆时针
                            // triangles[indicIndex++] = index;
                            // triangles[indicIndex++] = index + 1 + Resolution;
                            // triangles[indicIndex++] = index + 1;
                            //
                            // triangles[indicIndex++] = index + 1 + Resolution;
                            // triangles[indicIndex++] = index ;
                            // triangles[indicIndex++] = index + Resolution;
                            
                            //
                            // //逆时针
                            triangles[indicIndex++] = index;
                            triangles[indicIndex++] = index + Resolution;
                            triangles[indicIndex++] = index + 1 + Resolution;
                            
                            triangles[indicIndex++] = index + 1 + Resolution;
                            triangles[indicIndex++] = index + 1;
                            triangles[indicIndex++] = index;
                        }
                    }
                }

            }
            var end = System.DateTime.Now;
            // Debug.LogWarning($"cost time {(end-start).TotalMilliseconds}ms");
            
            var mesh = MeshFilter.sharedMesh;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);
            
        }

        public void UpdateColor(ColorGenerate colorGenerate)
        {
            // this.colorGenerate = _colorGenerate;
            var sharedMaterial = MeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
            sharedMaterial.SetColor("_BaseColor",colorGenerate.Execute());
        }
        
    }
}