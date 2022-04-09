using System;
using Planet.Setting;
using Unity.Mathematics;
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
        private Vector2[] uvs;
        private int[] triangles;
        public MinMax top = new MinMax();
        public MinMax depth = new MinMax();
        public void Init(
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

        public void Update(int resolution,VertexGenerate vertexGenerate,GPUShapeGenerate gpuShapeGenerate)
        {
            // var _computeShader = shapeGenerate.ShapeSettting.computeShader;

            if (Resolution != resolution)
            {
                Resolution = resolution;
                vertices = new Vector3[(Resolution ) * (Resolution )];
                uvs = new Vector2[(Resolution ) * (Resolution )];
                var multiple = (Resolution - 1) * (Resolution - 1);
                triangles = new int[multiple*2*3];
            }

            UpdateShape(vertexGenerate,gpuShapeGenerate);
        }

        public void UpdateShape(VertexGenerate vertexGenerate,GPUShapeGenerate gpuShapeGenerate)
        {
            if (!MeshFilter.gameObject.activeInHierarchy)
            {
                return;
            }
            var start = System.DateTime.Now;
            if (vertexGenerate.shapeSettting.GPU)
            {
                gpuShapeGenerate.UpdateShape(vertexGenerate,vertices,triangles,uvs,
                    Resolution,
                    Normal,axisA,axisB);

                for (int i = 0; i < uvs.Length; i++)
                {
                    top.AddValue(uvs[i].x);
                    depth.AddValue(uvs[i].y);
                }
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
                        vertices[index] = vertexGenerate.Execulate(pos.normalized);
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
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);
            
        }

        public void UpdateMaterial(Material sharedMaterial)
        {
            this.MeshFilter.GetComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
        }
    }
}